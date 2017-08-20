using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.IO;
using System.Linq;
using System.Threading;
using MassTransit;
using Microsoft.Xrm.Sdk;
using NLog;
using Xrm.Oss.Interfacing.Domain;

namespace Xrm.Oss.CrmListener
{
    public class PublisherControl : MarshalByRefObject, IPublisherControlWrapper
    {
        private static readonly RegistrationBuilder regBuilder = new RegistrationBuilder();
        private CompositionContainer container;
        private DirectoryCatalog directoryCatalog = new DirectoryCatalog(publisherPath, regBuilder);
        private static readonly string publisherPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
        "Publishers");
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public PublisherControl()
        {
            regBuilder.ForTypesDerivedFrom<ICrmPublisher>().Export<ICrmPublisher>();

            var catalog = new AggregateCatalog
            {
                Catalogs = {
                    new AssemblyCatalog(typeof(PublisherControl).Assembly, regBuilder),
                    directoryCatalog
                }
            };

            container = new CompositionContainer(catalog, true);
            container.ComposeExportedValue(container);

            _logger.Info($"Registered {container.GetExportedValues<ICrmPublisher>().Count()} publishers.");

            var watcher = new FileSystemWatcher
            {
                Filter = "*.dll",
                Path = publisherPath
            };

            watcher.Created += ProcessChanges;
            watcher.Deleted += ProcessChanges;
            watcher.Changed += ProcessChanges;
            watcher.Renamed += ProcessChanges;

            watcher.EnableRaisingEvents = true;
        }

        private async void ProcessChanges(object sender, FileSystemEventArgs e)
        {
            _logger.Info($"Publicher control change triggered by file {e.FullPath}");

            if (!CheckFileHasCopied(e.FullPath))
            {
                return;
            }

            Recompose();

            _logger.Info($"Done processing changes");
        }

        private bool CheckFileHasCopied(string FilePath)
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    using (File.OpenRead(FilePath))
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                Thread.Sleep(100);

                return CheckFileHasCopied(FilePath);
            }

        }

        public List<ICrmPublisher> GetPublishers()
        {
            return container.GetExportedValues<ICrmPublisher>().ToList();
        }

        public void Recompose()
        {
            directoryCatalog.Refresh();
            container.ComposeParts(directoryCatalog.Parts);

            _logger.Info($"Registered {container.GetExportedValues<ICrmPublisher>().Count()} publishers.");
        }

        public void RouteMessage(ICrmMessage message, IOrganizationService service, IBusControl busControl)
        {
            GetPublishers()
                .Where(p => p.RetrieveSupportedScenarios().Contains(message.Scenario))
                .ToList()
                .ForEach(p => p.ProcessMessage(message, service, busControl));
        }
    }
}