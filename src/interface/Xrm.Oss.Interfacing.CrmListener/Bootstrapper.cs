using Castle.Windsor;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Windsor;

namespace Xrm.Oss.Interfacing.CrmListener
{
    public class Bootstrapper : WindsorNancyBootstrapper
    {
        protected override void ApplicationStartup(IWindsorContainer container, IPipelines pipelines)
        {
            // No registrations should be performed in here, however you may
            // resolve things that are needed during application startup.
        }

        protected override void ConfigureApplicationContainer(IWindsorContainer existingContainer)
        {
            // Perform registrations here
            base.ConfigureApplicationContainer(existingContainer);
        }

        protected override void RequestStartup(IWindsorContainer container, IPipelines pipelines, NancyContext context)
        {
            // No registrations should be performed in here, however you may
            // resolve things that are needed during request startup.
        }

        protected override IWindsorContainer GetApplicationContainer()
        {
            return Container.Instance;
        }
    }
}
