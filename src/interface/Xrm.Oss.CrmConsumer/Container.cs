using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MassTransit;
using Xrm.Oss.Interfacing.Domain.Implementations;
using Xrm.Oss.Interfacing.Domain.Interfaces;

namespace Xrm.Oss.CrmConsumer
{
    public class Container : IWindsorInstaller
    {
        private void InitializePublishers()
        {
            var consumerPath = Path.Combine(Assembly.GetExecutingAssembly().Location, "Consumers");
            Directory.CreateDirectory(consumerPath);

            var catalog = new AggregateCatalog
            {
                Catalogs =
                {
                    new DirectoryCatalog(consumerPath)
                }
            };

            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }

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

                cfg.ReceiveEndpoint(host, "Xrm-Oss-CrmConsumer", ec => {
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