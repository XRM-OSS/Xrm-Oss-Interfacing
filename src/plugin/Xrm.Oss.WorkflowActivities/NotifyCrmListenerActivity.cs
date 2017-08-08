using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace Xrm.Oss.WorkflowActivities
{
    public class NotifyCrmListenerActivity : CodeActivity
    {
        [Input("Action")]
        [RequiredArgument]
        public InArgument<string> Action { get; set; }

        [Input("EndpointUrl")]
        [RequiredArgument]
        public InArgument<string> EndpointUrl { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            //Create the tracing service
            var tracingService = executionContext.GetExtension<ITracingService>();
            //Create the context
            var context = executionContext.GetExtension<IWorkflowContext>();

            var action = Action.Get(executionContext);
            var endpointUrl = EndpointUrl.Get(executionContext);
            var entity = context.PrimaryEntityName;
            var id = context.PrimaryEntityId;

            using (var httpClient = new HttpClient())
            {
                var url = $"{endpointUrl}/trigger/{action}/{entity}/{id}";

                using (var result = httpClient.GetAsync(url).Result)
                {
                    if(result.StatusCode != HttpStatusCode.OK)
                    {
                        throw new InvalidPluginExecutionException($"Received status code {result.StatusCode} for url {url}");
                    }
                }
            }
        }
    }
}
