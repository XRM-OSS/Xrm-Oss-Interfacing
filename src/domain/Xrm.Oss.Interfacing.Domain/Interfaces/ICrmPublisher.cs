using System.Collections.Generic;
using MassTransit;
using Microsoft.Xrm.Sdk;

namespace Xrm.Oss.Interfacing.Domain.Interfaces
{
	public interface ICrmPublisher
	{
		void ProcessMessage(ICrmEvent message, IOrganizationService service, IBusControl busControl);
        List<IScenario> RetrieveSupportedScenarios();
	}
}
