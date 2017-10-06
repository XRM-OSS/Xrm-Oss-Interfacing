using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Xrm.Sdk;
using NLog;
using Xrm.Oss.Interfacing.Domain.Contracts;

namespace Xrm.Oss.Interfacing.CrmPublisher
{
    public class PublisherControl : IPublisherControl
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        [ImportMany(typeof(ICrmPublisher))]
        private IEnumerable<ICrmPublisher> _publishers;
        private Dictionary<IScenario, List<ICrmPublisher>> _registrations = new Dictionary<IScenario, List<ICrmPublisher>>();

        private void ComposeRegistrations()
        {
            _logger.Info("Registering publishers");

            foreach (var publisher in _publishers)
            {
                var supportedScenarios = publisher.RetrieveSupportedScenarios();

                _logger.Trace($"Registering {publisher.GetType().Name}");

                foreach (var scenario in supportedScenarios)
                {
                    _logger.Trace($"Registering for {scenario}");

                    if (_registrations.ContainsKey(scenario))
                    {
                        _registrations[scenario].Add(publisher);
                    }
                    else
                    {
                        _registrations[scenario] = new List<ICrmPublisher> { publisher };
                    }
                }
            }
            _logger.Info("Registrations Done");
        }

        private void InitializePublishers()
        {
            _logger.Info("Initializing Publishers");

            var publisherPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Publishers");
            Directory.CreateDirectory(publisherPath);

            var catalog = new AggregateCatalog
            {
                Catalogs =
                {
                    new DirectoryCatalog(publisherPath)
                }
            };

            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }

        public PublisherControl()
        {
            InitializePublishers();
            ComposeRegistrations();
        }

        public void RouteMessage(ICrmEvent crmEvent, IOrganizationService service, IBusControl busControl)
        {
            var scenario = crmEvent.Scenario;

            if (!_registrations.ContainsKey(scenario))
            {
                throw new InvalidOperationException($"No publisher registered for scenario {scenario}, aborting.");
            }

            var publishers = _registrations[scenario];

            _logger.Trace($"Routing message {scenario} to {string.Join(", ", publishers.Select(p => p.GetType().Name))}");

            Parallel.ForEach(publishers, (publisher) =>
            {
                try
                {
                    publisher.ProcessMessage(crmEvent, service, busControl);
                }
                catch(Exception ex)
                {
                    _logger.Error(ex, $"Exception occured during processing of message {crmEvent.CorrelationId} in {publisher.GetType().Name}, moving to error queue. Message: {ex.Message}");
                    throw;
                }
            });
        }
    }
}
