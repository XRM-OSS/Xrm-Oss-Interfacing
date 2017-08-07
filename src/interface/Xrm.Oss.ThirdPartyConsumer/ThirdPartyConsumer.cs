using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using NLog;
using Xrm.Oss.Interfacing.Domain;

namespace Xrm.Oss.ThirdPartyConsumer
{
    public class CrudMessageConsumer : IConsumer<ICrmMessage>
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public CrudMessageConsumer() { }

        public Task Consume(ConsumeContext<ICrmMessage> context)
        {
            var message = context.Message;

            var text = $@"Received message for action {message.Action} on entity {message.Entity} with id {message.Id}.
Payload: {string.Join(",", message.Attributes?.Select(pair => $"{pair.Key}: {pair.Value}") ?? new[] { "" })}";

            _logger.Info(text);

            return Task.FromResult(0);
        }
    }
}
