using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using NLog;
using Xrm.Oss.Interfacing.Domain;
using Xrm.Oss.Interfacing.Domain.Interfaces;

namespace Xrm.Oss.ThirdPartyConsumer
{
    public class ContactCreatedConsumer : IConsumer<IDemoCrmContactCreated>
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public ContactCreatedConsumer() { }

        public Task Consume(ConsumeContext<IDemoCrmContactCreated> context)
        {
            var message = context.Message;

            var text = $"Received CRM contact created message, values:{Environment.NewLine}"
                + $"Contact ID: {message.ContactId}{Environment.NewLine}"
                + $"EMailAddress1: {message.EMailAddress1}{Environment.NewLine}"
                + $"First Name: {message.FirstName}{Environment.NewLine}"
                + $"Last Name: {message.LastName}{Environment.NewLine}"
                + $"Telephone1: {message.Telephone1}";

            _logger.Info(text);

            return Task.FromResult(0);
        }
    }
}
