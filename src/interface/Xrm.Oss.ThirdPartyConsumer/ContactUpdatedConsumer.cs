using System;
using System.Threading.Tasks;
using MassTransit;
using NLog;
using Xrm.Oss.Interfacing.Domain.Interfaces;

namespace Xrm.Oss.ThirdPartyConsumer
{
    public class ContactUpdatedConsumer : IConsumer<IDemoCrmContactUpdated>
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public ContactUpdatedConsumer() { }

        public Task Consume(ConsumeContext<IDemoCrmContactUpdated> context)
        {
            var message = context.Message;

            var text = $"Received CRM contact updated message, values:{Environment.NewLine}"
                + $"Contact ID: {message.ContactId}{Environment.NewLine}"
                + $"EMailAddress1: {message.EMailAddress1}{Environment.NewLine}"
                + $"First Name: {message.FirstName}{Environment.NewLine}"
                + $"Last Name: {message.LastName}{Environment.NewLine}"
                + $"Telephone1: {message.Telephone1}{Environment.NewLine}";

            _logger.Info(text);

            return Task.FromResult(0);
        }
    }
}
