using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MassTransit;
using Xrm.Oss.Interfacing.Domain;

namespace Xrm.Oss.ThirdPartyConsumer
{
    public class Container : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Types.FromThisAssembly().BasedOn<IConsumer>());

            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                // Send only bus does not need any receive points
                var host = cfg.Host(new Uri(ConfigurationManager.AppSettings["RabbitMq.Url"]), h =>
                {
                    h.Username(ConfigurationManager.AppSettings["RabbitMq.Username"]);
                    h.Password(ConfigurationManager.AppSettings["RabbitMq.Password"]);
                });

                cfg.ReceiveEndpoint(host, "Xrm-Oss-ThirdPartyConsumer", ec => {
                    ec.UseMessageScope();
                    ec.LoadFrom(container);
                });
            });

            container.Register(Component.For<IBus>().Forward<IBusControl>().Instance(busControl));
            container.Register(Component.For<ILazyComponentLoader>().ImplementedBy<LazyOfTComponentLoader>());
            container.Register(Component.For<IService>().ImplementedBy<BaseService>());
        }
    }
}