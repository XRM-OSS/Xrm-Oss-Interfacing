using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm.Oss.Interfacing.Domain;

namespace Xrm.Oss.ThirdPartyPublisher
{
    public class CrudMessage : ICrudMessage
    {
        public Dictionary<string, object> Attributes { get; set; }
        public string Event { get; set; }
        public string Entity { get; set; }
        public Guid? RecordId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string CorrelationId { get; set; }
    }
}
