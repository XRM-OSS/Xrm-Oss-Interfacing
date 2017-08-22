using System;

namespace Xrm.Oss.Interfacing.Domain.Interfaces
{
    public interface IScenario : IEquatable<IScenario>
    {
        string Event { get; set; }
        string Entity { get; set; }
    }
}
