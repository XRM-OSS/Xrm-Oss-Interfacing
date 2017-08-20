using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Owin.Hosting;
using NLog;
using Xrm.Oss.Interfacing.Domain;

namespace Xrm.Oss.CrmListener
{
    public class Service : IService
    {
        protected readonly Lazy<IBusControl> _lazyServiceBus;
        protected IBusControl _serviceBus;
        private IDisposable _webApp;
        private IPublisherControlWrapper _publisherControl;
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private AppDomain _domain;

        public Service(Lazy<IBusControl> lazyServiceBus)
        {
            _lazyServiceBus = lazyServiceBus;
        }

        private void StartWebEndpoint()
        {
            var url = ConfigurationManager.AppSettings["endPointUrl"];

            _logger.Trace($"Starting endpoint on URL {url}");
            _webApp = WebApp.Start<Startup>(url);
            _logger.Trace("Successfully started endpoint");
        }

        private void StartBus()
        {
            _logger.Info("Starting service bus");
            _serviceBus = _lazyServiceBus.Value;
            _serviceBus.Start();
            _logger.Info("Service bus started");
        }

        public void Start()
        {
            StartWebEndpoint();
            StartBus();
        }

        public void Stop()
        {
            _logger.Info("Stopping web endpoint");
            _webApp.Dispose();
            _logger.Info("Web endpoint stopped");

            _logger.Info("Stopping service bus");
            _serviceBus.Stop();
            _logger.Info("Service bus stopped");
        }
    }
}
