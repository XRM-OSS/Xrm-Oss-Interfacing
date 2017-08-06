using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using Castle.Windsor.Installer;
using MassTransit;
using Topshelf;
using Xrm.Oss.Interfacing.Domain;

namespace Xrm.Oss.ThirdPartyPublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                IWindsorContainer container = null;

                x.Service<IService>(s =>
                {
                    s.BeforeStartingService(sc => sc.RequestAdditionalTime(TimeSpan.FromSeconds(30)));
                    s.BeforeStoppingService(sc => sc.RequestAdditionalTime(TimeSpan.FromSeconds(30)));

                    s.ConstructUsing(() =>
                    {
                            // container should be initialized in here according to Chris Patterson
                            // https://groups.google.com/d/msg/masstransit-discuss/Pz7ttS7niGQ/A-K7MTK8aiUJ
                            container = new WindsorContainer().Install(FromAssembly.This());

                            return container.Resolve<IService>();
                    });

                    s.WhenStarted(service => { service.Start(); });

                    s.WhenStopped(service =>
                    {
                        service.Stop();

                        if (container != null)
                        {
                            container.Dispose();
                        }
                    });

                    s.WhenShutdown(service =>
                    {
                        service.Stop();

                        if (container != null)
                        {
                            container.Dispose();
                        }
                    });
                });

                x.UseNLog();
                x.EnableShutdown();
                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.SetServiceName("Xrm-Oss-ThirdPartyPublisher");
                x.SetDisplayName("Xrm-Oss-ThirdPartyPublisher");
                x.SetDescription("Xrm-Oss-ThirdPartyPublisher");
            });
        }
    }    
}