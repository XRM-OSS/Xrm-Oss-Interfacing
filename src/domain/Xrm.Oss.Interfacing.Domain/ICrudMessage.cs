using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace Xrm.Oss.Interfacing.Domain
{
    public interface ICrudMessage : IMessage
    {
        Dictionary<string, object> Attributes { get; set; } 
    }
}
