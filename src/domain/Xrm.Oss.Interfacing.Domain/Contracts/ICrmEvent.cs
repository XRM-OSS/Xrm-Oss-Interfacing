using System;
using Xrm.Oss.Interfacing.Domain.Implementations;

namespace Xrm.Oss.Interfacing.Domain.Contracts
{
    public interface ICrmEvent : IMessage
    {
        Scenario Scenario { get; set; }
        Guid? RecordId { get; set; }
    }
}
