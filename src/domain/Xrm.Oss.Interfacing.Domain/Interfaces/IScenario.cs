using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xrm.Oss.Interfacing.Domain
{
    public interface IScenario : IEquatable<IScenario>
    {
        string Event { get; set; }
        string Entity { get; set; }
    }
}
