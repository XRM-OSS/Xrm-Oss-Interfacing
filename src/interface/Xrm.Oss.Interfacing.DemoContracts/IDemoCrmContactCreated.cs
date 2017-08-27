using System;
using Xrm.Oss.Interfacing.Domain.Contracts;

namespace Xrm.Oss.Interfacing.DemoContracts
{
    public interface IDemoCrmContactCreated : IMessage
    {
        Guid ContactId { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string EMailAddress1 { get; set; }
        string Telephone1 { get; set; }
    }
}
