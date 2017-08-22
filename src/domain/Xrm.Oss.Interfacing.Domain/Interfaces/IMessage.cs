using System;

namespace Xrm.Oss.Interfacing.Domain.Interfaces
{
    public interface IMessage
    {
		DateTime TimeStamp { get; set; }
        string CorrelationId { get; set; }
    }
}
