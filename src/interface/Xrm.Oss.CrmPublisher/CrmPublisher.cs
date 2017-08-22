using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Xrm.Sdk;
using Xrm.Oss.Interfacing.Domain.Interfaces;

namespace Xrm.Oss.CrmPublisher
{
    public class CrmPublisher : IConsumer<ICrmEvent>
    {
        private IBusControl _busControl;
        private IOrganizationService _service;

        [ImportMany(typeof(ICrmPublisher))]
        private IEnumerable<ICrmPublisher> _publishers;
        private Dictionary<IScenario, List<ICrmPublisher>> _registrations;

        private void ComposeRegistrations()
        {
            foreach (var publisher in _publishers)
            {
                var supportedScenarios = publisher.RetrieveSupportedScenarios();

                foreach (var scenario in supportedScenarios)
                {
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
        }

        private void InitializePublishers()
        {
            var publisherPath = Path.Combine(Assembly.GetExecutingAssembly().Location, "Publishers");
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

        public CrmPublisher(IBusControl busControl, IOrganizationService service)
        {
            _busControl = busControl;
            _service = service;

            InitializePublishers();
            ComposeRegistrations();
        }

        public Task Consume(ConsumeContext<ICrmEvent> context)
        {
            var message = context.Message;
            var scenario = message.Scenario;

            if (!_registrations.ContainsKey(scenario))
            {
                throw new InvalidOperationException($"No publisher registered for scenario {scenario}, aborting.");
            }

            var publishers = _registrations[scenario];

            Parallel.ForEach(publishers, (publisher) =>
            {
                publisher.ProcessMessage(message, _service, _busControl);
            });

            return Task.FromResult(0);
        }
    }
}
