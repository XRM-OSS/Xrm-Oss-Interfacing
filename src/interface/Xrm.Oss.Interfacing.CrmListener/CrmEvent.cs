using System;
using Xrm.Oss.Interfacing.Domain.Implementations;
using Xrm.Oss.Interfacing.Domain.Contracts;

namespace Xrm.Oss.Interfacing.CrmListener
{
    public class CrmEvent : ICrmEvent
    {
        public Guid? RecordId { get; set; }
        public DateTime TimeStamp { get; set; }
        public Scenario Scenario { get; set; }
        public string CorrelationId { get; set; }
    }
}
