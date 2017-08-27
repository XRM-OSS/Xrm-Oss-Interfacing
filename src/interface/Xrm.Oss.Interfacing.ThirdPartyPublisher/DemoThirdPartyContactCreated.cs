using System;
using Xrm.Oss.Interfacing.DemoContracts;

namespace Xrm.Oss.Interfacing.ThirdPartyPublisher
{
    public class DemoThirdPartyContactCreated : IDemoThirdPartyContactCreated
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EMailAddress1 { get; set; }
        public string Telephone1 { get; set; }
        public DateTime TimeStamp { get; set; }
        public string CorrelationId { get; set; }
    }
}
