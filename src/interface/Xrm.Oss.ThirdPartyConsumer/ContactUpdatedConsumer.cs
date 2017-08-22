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
                + $"Contact ID: {message.ContactId}"
                + $"EMailAddress1: {message.EMailAddress1}"
                + $"First Name: {message.FirstName}"
                + $"Last Name: {message.LastName}"
                + $"Telephone1: {message.Telephone1}";

            _logger.Info(text);

            return Task.FromResult(0);
        }
    }
}
