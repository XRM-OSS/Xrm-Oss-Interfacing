using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using NLog;
using Xrm.Oss.Interfacing.Domain;

namespace Xrm.Oss.CrmConsumer
{
    public class Service : IService
    {
        private readonly Lazy<IBusControl> _lazyServiceBus;
        private IBusControl _serviceBus;
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public Service(Lazy<IBusControl> lazyServiceBus)
        {
            _lazyServiceBus = lazyServiceBus;
        }

        public void Start()
        {
            _logger.Info("Starting service bus");

            _serviceBus = _lazyServiceBus.Value;
            _serviceBus.Start();

            _logger.Info("Service bus started");
        }

        public void Stop()
        {
            _logger.Info("Stopping service bus");

            _serviceBus.Stop();

            _logger.Info("Service bus stopped");
        }
    }
}