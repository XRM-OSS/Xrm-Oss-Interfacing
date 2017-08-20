# Xrm-OSS-Interfacing

This is a template for creating interfaces from and to Dynamics CRM/365, using messaging for enabling real time interfaces.
It uses RabbitMQ and MassTransit for transporting the messages.
The purpose is to demonstrate, how interfaces can work using this technology and to be a template for your interface projects.

## Support
This project uses the latest Dynamics 365 SDK, so there is support for CRM >=2011.
For CRM/365 online, the CRM listener will need to be reachable from outside of your network.

## Getting Started
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
Navigate into the project root with your PowerShell and execute `.\build.cmd`.
Afterwards, all interface components will be built and published into the "Publish" folder.
Adjust the application configurations to point to your RabbitMQ instance (will probably already fit if you didn't change anything. The virtual host I created was named "Dev", so either create one as well and give your user access, or change it).
In the configuration for the CRM consumer, adjust the CRM connection string to point to your CRM organization.

Start Xrm.Oss.CrmConsumer.exe (always start the consumers first, as they need to register their queues for receiving their messages) and afterwards Xrm.Oss.ThirdPartyPublisher.exe.
When you copy the test csv into the Publisher's Import folder, it will parse it, send it to RabbitMQ and the consumer will send the data to CRM (currently only create).
You can also install the consumers and publishers as services with topshelf, this is what you'll want to do in your production environment. For this you can just copy them to your Program Files folder or anywhere where you like, and execute them in an admin console while appending "install" to the command line call. This will add a windows service that you can start, stop and so on.

For sending messages triggered by CRM actions, register the WorkflowActivity in this project and use it inside an asynchronous workflow, that triggers on the actions that you'd like to cover. You'll have to configure the name of the event and the EndPoint URL of the CRM listener. The workflow activity can be registered in isolation (sandbox), but then you'll not be able to configure an IP address as EndPoint URL, in sandbox it will have to be something that DNS can resolve.
The CRM listener is a small web service that is reachable from your CRM. It sends the events to the listener, which chooses the appropriate Publisher for publishing the message to RabbitMQ. Right now the CRM listener sends the messages to RabbitMQ itself, but this is subject to change and later on you'll be able to copy own publishers into its directory, that will be loaded and used for processing.
The CrmListener needs RabbitMQ configuration, CRM configuration and additionally an URL, which it can use for hosting.

For not having to run the CrmListener as Admin, add an urlacl using PowerShell (Switch port if you configured otherwise):
```netsh http add urlacl url=http://+:8080/ user=DOMAIN\username```

Once you have the workflow, CRM listener and ThirdPartyConsumer in place, messages from CRM will be sent to RabbitMQ and processed by the ThirdPartyConsumer.
