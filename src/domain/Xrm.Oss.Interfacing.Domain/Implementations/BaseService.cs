using System;
using MassTransit;
using NLog;
using Xrm.Oss.Interfacing.Domain.Contracts;

namespace Xrm.Oss.Interfacing.Domain.Implementations
{
    public class BaseService : IService
    {
        protected readonly Lazy<IBusControl> _lazyServiceBus;
        protected IBusControl _serviceBus;
        protected Logger _logger = LogManager.GetCurrentClassLogger();

        public BaseService(Lazy<IBusControl> lazyServiceBus)
        {
            _lazyServiceBus = lazyServiceBus;
        }

        public virtual void Start()
        {
            _logger.Info("Starting service bus");

            _serviceBus = _lazyServiceBus.Value;
            _serviceBus.Start();

            _logger.Info("Service bus started");
        }

        public virtual void Stop()
        {
            _logger.Info("Stopping service bus");

            _serviceBus.Stop();

            _logger.Info("Service bus stopped");
        }
    }
}