using System;

namespace Xrm.Oss.Interfacing.Domain.Contracts
{
    public interface IMessage
    {
		DateTime TimeStamp { get; set; }
        string CorrelationId { get; set; }
    }
}
