using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xrm.Oss.Interfacing.Domain
{
    public interface IMessage
    {
		string Event { get; set; }
        string Entity { get; set; }
		Guid? RecordId { get; set; }
		DateTime TimeStamp { get; set; }
		string CorrelationId { get; set; }
    }
}
