using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using MassTransit;
using NLog;
using Topshelf;
using Xrm.Oss.Interfacing.Domain;

namespace Xrm.Oss.ThirdPartyPublisher
{
    public class Service : IService
    {
        private readonly Lazy<IBusControl> _lazyServiceBus;
        private IBusControl _serviceBus;
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly string _importDir;
        private readonly string _archiveDir;

        public Service(Lazy<IBusControl> lazyServiceBus)
        {
            _lazyServiceBus = lazyServiceBus;

            _importDir = ConfigurationManager.AppSettings["ImportDirectory"];
            _archiveDir = ConfigurationManager.AppSettings["ArchiveDirectory"];

            Directory.CreateDirectory(_importDir);
            Directory.CreateDirectory(_archiveDir);
        }

        private async Task<bool> PublishMessage(ExpandoObject record)
        {
            var iterable = record as ICollection<KeyValuePair<string, object>>;

            if (iterable == null)
            {
                _logger.Error("Failed to iterate object, skipping record");
                return false;
            }

            var logicalName = iterable.FirstOrDefault(pair => "logicalName".Equals(pair.Key, StringComparison.InvariantCultureIgnoreCase)).Value?.ToString();
            var ev = iterable.FirstOrDefault(pair => "event".Equals(pair.Key, StringComparison.InvariantCultureIgnoreCase)).Value?.ToString();

            if (string.IsNullOrEmpty(ev))
            {
                _logger.Error("Record had no event, skipping.");
                return false;
            }

            if (string.IsNullOrEmpty(logicalName))
            {
                _logger.Error("Record had no logical name, skipping.");
                return false;
            }

            var message = new CrudMessage
            {
                Attributes = iterable.Where(pair => !new[] { "logicalName", "event" }.Contains(pair.Key)).ToDictionary(pair => pair.Key, pair => pair.Value),
                Entity = logicalName,
                TimeStamp = DateTime.UtcNow,
                Event = ev
            };

            await _serviceBus.Publish(message);
            return true;
        }

        private async void ProcessFile(object sender, FileSystemEventArgs e)
        {
            _logger.Info($"Processing file {e.FullPath}");

            if (!CheckFileHasCopied(e.FullPath))
            {
                return;
            }

            using (var textReader = File.OpenText(e.FullPath))
            {
                using (var csvReader = new CsvReader(textReader))
                {
                    var records = csvReader.GetRecords<dynamic>().ToList();

                    records.ForEach(async rec =>
                    {
                        await PublishMessage(rec);
                    });
                }
            }

            var targetPath = Path.Combine(_archiveDir, DateTime.UtcNow.ToString("yyyyMMdd-hh-mm-ss") + e.Name);

            if (File.Exists(targetPath))
            {
                _logger.Warn("Not moving file to archive, there is already a file named like this");
            }

            File.Move(e.FullPath, targetPath);

            _logger.Info($"Done processing file {e.FullPath}, moved to archive.");
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

        public void Start()
        {
            _logger.Info("Starting service bus");

            _serviceBus = _lazyServiceBus.Value;
            _serviceBus.Start();

            var watcher = new FileSystemWatcher
            {
                Filter = "*.csv",
                Path = _importDir
            };

            watcher.Created += ProcessFile;

            watcher.EnableRaisingEvents = true;

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