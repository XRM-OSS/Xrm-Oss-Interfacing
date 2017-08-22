using System;

namespace Xrm.Oss.Interfacing.Domain.Interfaces
{
    public interface ICrmEvent : IMessage
    {
        IScenario Scenario { get; set; }
        Guid? RecordId { get; set; }
    }
}
