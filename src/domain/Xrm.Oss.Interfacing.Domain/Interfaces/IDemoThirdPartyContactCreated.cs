using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xrm.Oss.Interfacing.Domain.Interfaces
{
    public interface IDemoThirdPartyContactCreated : IMessage
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string EMailAddress1 { get; set; }
        string Telephone1 { get; set; }
    }
}
