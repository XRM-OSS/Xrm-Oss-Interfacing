using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xrm.Oss.Interfacing.Domain.Implementations
{
    public class Scenario : IScenario
    {
        public string Event { get; set; }
        public string Entity { get; set; }

        public Scenario(string @event, string entity)
        {
            Event = @event;
            Entity = entity;
        }

        public override string ToString()
        {
            return $"{Event}-{Entity}";
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as IScenario);
        }

        public bool Equals(IScenario other)
        {
            return string.Equals(ToString(), other?.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
