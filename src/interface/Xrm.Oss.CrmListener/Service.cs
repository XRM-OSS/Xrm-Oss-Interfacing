using System;
using System.Collections.Generic;
using System.Configuration;
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
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public Service(Lazy<IBusControl> lazyServiceBus)
        {
            _lazyServiceBus = lazyServiceBus;
        }

        public void Start()
        {
            var url = ConfigurationManager.AppSettings["endPointUrl"];

            _logger.Trace($"Starting endpoint on URL {url}");
            _webApp = WebApp.Start<Startup>(url);
            _logger.Trace("Successfully started endpoint");

            _logger.Info("Starting service bus");
            _serviceBus = _lazyServiceBus.Value;
            _serviceBus.Start();
            _logger.Info("Service bus started");
        }

        public void Stop()
        {
            _webApp.Dispose();

            _logger.Info("Stopping service bus");

            _serviceBus.Stop();

            _logger.Info("Service bus stopped");
        }
    }
}
