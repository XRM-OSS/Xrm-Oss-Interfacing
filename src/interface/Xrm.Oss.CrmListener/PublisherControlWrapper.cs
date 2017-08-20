using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Xrm.Sdk;
using Xrm.Oss.Interfacing.Domain;

namespace Xrm.Oss.CrmListener
{
    public class PublisherControlWrapper : IDisposable, IPublisherControlWrapper
    {
        private PublisherControl _publisherControl;
        private AppDomain _domain;

        public PublisherControlWrapper()
        {
            var cachePath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "ShadowCopyCache");
            var pluginPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Publishers");

            Directory.CreateDirectory(cachePath);
            Directory.CreateDirectory(pluginPath);

            var setup = new AppDomainSetup
            {
                CachePath = cachePath,
                ShadowCopyFiles = "true",
                ShadowCopyDirectories = pluginPath
            };

            _domain = AppDomain.CreateDomain("CrmListener_AppDomain", AppDomain.CurrentDomain.Evidence, setup);
            _publisherControl = (PublisherControl)_domain.CreateInstanceAndUnwrap(typeof(PublisherControl).Assembly.FullName, typeof(PublisherControl).FullName);
        }

        public void RouteMessage(ICrmMessage message, IOrganizationService service, IBusControl busControl)
        {
            _publisherControl.RouteMessage(message, service, busControl);
        }

        public void Dispose()
        {
            AppDomain.Unload(_domain);
        }
    }
}
