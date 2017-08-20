using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm.Oss.Interfacing.Domain;

namespace Xrm.Oss.CrmPublisher
{
    public class CrmMessage : ICrmMessage
    {
        public Dictionary<string, object> Attributes { get; set; }
        public Guid? RecordId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string CorrelationId { get; set; }
        public IScenario Scenario { get; set; }
    }
}
