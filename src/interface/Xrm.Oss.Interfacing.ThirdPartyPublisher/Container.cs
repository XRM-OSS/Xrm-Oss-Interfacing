using System;
using System.Configuration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MassTransit;
using Xrm.Oss.Interfacing.Domain.Contracts;

namespace Xrm.Oss.Interfacing.ThirdPartyPublisher
{
    public class Container : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                // Send only bus does not need any receive points
                var host = cfg.Host(new Uri(ConfigurationManager.AppSettings["RabbitMq.Url"]), h =>
                {
                    h.Username(ConfigurationManager.AppSettings["RabbitMq.Username"]);
                    h.Password(ConfigurationManager.AppSettings["RabbitMq.Password"]);
                });
            });

            container.Register(Component.For<IBus>().Forward<IBusControl>().Instance(busControl));
            container.Register(Component.For<ILazyComponentLoader>().ImplementedBy<LazyOfTComponentLoader>());
            container.Register(Component.For<IService>().ImplementedBy<Service>());
        }
    }
}
