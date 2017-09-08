using System;
using Castle.Windsor;
using Castle.Windsor.Installer;
using MassTransit;
using NLog;
using Topshelf;
using Xrm.Oss.Interfacing.Domain.Contracts;

namespace Xrm.Oss.Interfacing.CrmListener
{
    public class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

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
                        container = Container.Instance;

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
                x.OnException(ex => _logger.Error(ex));
                x.EnableShutdown();
                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.SetServiceName("Xrm-Oss-CrmListener");
                x.SetDisplayName("Xrm-Oss-CrmListener");
                x.SetDescription("Xrm-Oss-CrmListener");
            });
        }
    }
}