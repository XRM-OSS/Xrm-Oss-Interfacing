using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xrm.Oss.Interfacing.Domain
{
    public interface ICrmMessage : IMessage
    {
        Dictionary<string, object> Attributes { get; set; }
    }
}
