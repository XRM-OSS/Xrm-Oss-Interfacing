using System;

namespace Xrm.Oss.Interfacing.Domain.Contracts
{
    public interface IScenario : IEquatable<IScenario>
    {
        string Event { get; set; }
        string Entity { get; set; }
    }
}
