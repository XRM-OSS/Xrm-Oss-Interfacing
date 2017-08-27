using System;
using System.Configuration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace Xrm.Oss.Interfacing.CrmPublisher
{
    public class CrmConnection : IWindsorInstaller
    {
        private IOrganizationService Connect()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["CRM"].ConnectionString;
            var conn = new CrmServiceClient(connectionString);

            if (!conn.IsReady)
            {
                throw new Exception($"Error establishing CRM connection, last error: {conn.LastCrmException.Message}.", conn.LastCrmException);
            }

            return conn.OrganizationWebProxyClient != null
                ? (IOrganizationService)conn.OrganizationWebProxyClient
                : (IOrganizationService)conn.OrganizationServiceProxy;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IOrganizationService>()
                .UsingFactoryMethod(Connect)
                .LifeStyle
                .PerThread);
        }
    }
}