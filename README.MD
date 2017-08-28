# Xrm-OSS-Interfacing [![Build status](https://ci.appveyor.com/api/projects/status/3qeoi45uym9dwabd/branch/master?svg=true)](https://ci.appveyor.com/project/DigitalFlow/xrm-oss-interfacing/branch/master) [![NuGet Badge](https://buildstats.info/nuget/Xrm.Oss.Interfacing.Domain)](https://www.nuget.org/packages/Xrm.Oss.Interfacing.Domain)

This is an implementation of a realtime messaging interface for Dynamics CRM/365, using RabbitMQ and MassTransit for transporting the messages.
The purpose is to give a base implementation and some working examples for setting up a realtime interface from and to Dynamics CRM/365.

## Support
This project uses the latest Dynamics 365 SDK, so there is support for CRM >=2011.
For CRM/365 online, the CRM listener will need to be reachable from outside of your network. Alternatively, a polling mechanism could be added to the CRM listener and used instead of the publisher workflows.

## Demo
### CRM event to Bus
Imagine we're on the form of a CRM contact.
We change the phone number (telephone1) of the contact and save our changes.
Our "Publish Contact Update" workflow (can be found in solution below "solutions" folder) will now be triggered asynchronously.
We set the URL of our CRM listener in the workflow definition, as well as what event just happened. 
A http request for the event is now sent to our CRM listener. It states which entity the event happened on, what event happened and the record id of the target record.

![image](https://user-images.githubusercontent.com/4287938/29791704-2b36db64-8c3e-11e7-8ee1-fb7be8174660.png)

The CRM listener now proceeds to publish this event as ICrmEvent message to the RabbitMQ bus.

RabbitMQ routes the message automatically to our CRM publisher, as we subscribed it to ICrmEvent messages.
The publisher consumes the message by invoking all ICrmPublishers, that were registered for the current scenario.

![image](https://user-images.githubusercontent.com/4287938/29792911-42be423c-8c42-11e7-9bbd-eee31e35261d.png)

The ICrmPublishers are built in a modular fashion and used for publishing messages to other consumers on the bus based on custom business logic.
Why doesn't the CRM listener do this? As stated above, the messages are built upon your business logic.
If a new attribute was added to your message, you'd have to adjust the message contract and update the interface.
As you have to stop it for updating it, you would have to stop the CrmListener for updating your messages. This is not desirable, as CRM workflows trying to reach the endpoint will fail, if the listener is not running.
Therefore the listener is as simple as possible for reaching the highest availability.
The CrmPublisher in contrast can be stopped at any point, as RabbitMQ will queue and persist all messages created by the CrmListener and we can process them right after we updated our messages and started the publisher again.

RabbitMQ now searches for subscribers to the message that our ICrmPublisher just published to the bus.
In a real world environment, the subscribers would be the other services of your system landscape, that you want to integrate.
In our example, we published an IDemoCrmContactUpdated message, which our ThirdPartyConsumer is subscribed to.
It therefore receives our message and processes it.
As this is a bus system, it doesn't matter how many subscribers exist to a message type, all of them will receive the message and be able to use it for updating their data.

As a nice benefit, RabbitMq supplies us with a management interface, where we can check the state of all connected parties and their messaging queues.
Our ThirdPartyConsumer queue now shows the message that we just published:

![image](https://user-images.githubusercontent.com/4287938/29792992-94726f4a-8c42-11e7-975c-2c09de3f5cc5.png)

Once we launch our ThirdPartyConsumer, it will consume the message:
![image](https://user-images.githubusercontent.com/4287938/29793145-209f9a7e-8c43-11e7-9f8f-22604b34597c.png)

All of the components are already designed to be executed as a service, so that they will be available all the time.

### Bus message to CRM
For consuming messages from the bus and applying the changes to the CRM, we use our CrmConsumer.
It registers itself for messages at RabbitMQ and receives them.

This project contains a ThirdPartyPublisher which is used to simulate message traffic from other systems to the CRM.
It consumes csv files and publishes each line as message to the bus.

![image](https://user-images.githubusercontent.com/4287938/29793433-19c40a54-8c44-11e7-839a-9500177dc80e.png)

Once the CRM consumer receives the messages, it processes the changes:
![image](https://user-images.githubusercontent.com/4287938/29793795-95bb5f80-8c45-11e7-9a45-900fc5987468.png)

## Using it yourself
### Installing
First of,  we'll need Erlang and RabbitMQ as infrastructure.
Download Erlang first [here](http://www.erlang.org/downloads) and install it.

IMPORTANT: Afterwards, go to your _System_ variables and add an entry `ERLANG_HOME` to the path, where you installed erlang.
For me (64bit installation), this was `C:\Program Files\erl9.0`.
Also, copy the `C:\Windows\.erlang.cookie` (it might not exist directly after installation, but it will once the rabbitmq-plugins command below was executed) file to your home directory and replace the cookie file that is stored there.

Proceed with [RabbitMQ](http://www.rabbitmq.com/download.html) afterwards.

### Configuring
RabbitMQ has a neat webinterface, that is not enabled by default.
Head to your application menu and search for "RabbitMQ Command Prompt" and launch it.
Execute ```rabbitmq-plugins enable rabbitmq_management``` from there, followed by ```rabbitmq-service start```.
Afterwards, check the output of ```rabbitmqctl status```, it should not print any errors.

You can then open the RabbitMQ web interface using this URL: http://localhost:15672/
If you can't see the web interface and the page errors out, go back to the RabbitMQ Command Prompt and enter "rabbitmq-service.bat start".
The default credentials for RabbitMQ are 'guest' as user name and password.
You should now head to Admin > Virtual Hosts. There you should use "Add virtual host" for creating a host named "Dev" and if you like also already one named "Prod" for your later prod host.

### Running it
For the CRM Listener and the CRM publisher, MSI setups that install the services automatically are available on [AppVeyor](https://ci.appveyor.com/project/DigitalFlow/xrm-oss-interfacing/build/artifacts).
After installing them, head to Program Files\Xrm-Oss and adjust the configs for the RabbitMQ and CRM connections.

For the other components:
Navigate into the project root with your PowerShell and execute `.\build.cmd`.
Afterwards, all interface components will be built and published into the "Publish" folder.
Adjust the application configurations to point to your RabbitMQ instance (will probably already fit if you didn't change anything. The virtual host I created was named "Dev", so either create one as well and give your user access, or change it).
In the configuration for the CRM consumer, adjust the CRM connection string to point to your CRM organization.
The DemoPublisher and DemoContracts .DLLs need to be copied to a "Publishers" folder inside the CRM Publisher, that you need to create if it is not yet existing. Otherwise, the publisher will create the folder itself on the first run.

Start Xrm.Oss.Interfacing.CrmConsumer.exe (always start the consumers first, as they need to register their queues for receiving their messages) and afterwards Xrm.Oss.Interfacing.ThirdPartyPublisher.exe.
When you copy the test csv into the Publisher's Import folder, it will parse it, send it to RabbitMQ and the consumer will send the data to CRM (currently only create).
You can also install the consumers and publishers as services with topshelf, this is what you'll want to do in your production environment. For this you can just copy them to your Program Files folder or anywhere where you like, and execute them in an admin console while appending "install" to the command line call. This will add a windows service that you can start, stop and so on.

For sending messages triggered by CRM actions, register the WorkflowActivity in this project and use it inside an asynchronous workflow, that triggers on the actions that you'd like to cover. You'll have to configure the name of the event and the EndPoint URL of the CRM listener. The workflow activity can be registered in isolation (sandbox), but then you'll not be able to configure an IP address as EndPoint URL, in sandbox it will have to be something that DNS can resolve.
Inside this project, there is a solutions folder, that contains some demo workflows for publishing CRM events on create and update of contacts.
The CRM listener is a small web service that is reachable from your CRM. It sends the events to the bus, where they are preprocessed by the CRM Publisher. The CRM Publisher chooses the appropriate ICrmPublishers for publishing the message to RabbitMQ. 
For this, you have to place your publishers, which implement ICrmPublisher, into the Publishers directory of the CrmPublisher.
This uses MEF - the "Managed Extensibility Framework" for loading your assemblies dynamically. If they reference other DLLs, such as your custom message contract project, you need to copy these DLLs as well.
The Xrm.Oss.Interfacing.DemoPublisher project is a sample for this and can be used for sending update and create messages for contacts to the bus.
The CrmListener needs RabbitMQ configuration and additionally an URL, which it can use for hosting.

For not having to run the CrmListener as Admin, add an urlacl using PowerShell (Switch port if you configured otherwise):
```netsh http add urlacl url=http://+:8080/ user=DOMAIN\username```

Once you have the workflow, CRM listener and ThirdPartyConsumer in place, messages from CRM will be sent to RabbitMQ and processed by the ThirdPartyConsumer.

### Extending
So you tried the examples and everything worked fine. Awesome!
But now you want to add your own logic and messages. No problem!

The CRM Listener and CRM publisher can be used exactly as they are.
Just create a new domain project, where you implement your messages. You should make them inherit the IMessage interface of the Xrm.Oss.Interfacing.Domain project. This project is available on nuget using the same name.

In an additional project, you can then proceed to implement your ICrmPublishers, that send your custom messages to the bus.

For consuming messages, you can use the CRM consumer as a base and just add IConsumer classes for your custom messages.
The Castle Windsor container will register them automatically during the next run.

All of this is already done as a sample, just head over to the "sample" solution folder inside the solution and check it out.

### License
Licensed using the MIT license, enjoy!
