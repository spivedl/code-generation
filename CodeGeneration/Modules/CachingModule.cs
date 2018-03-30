using System.Linq;
using Autofac;
using Autofac.Core;
using CodeGeneration.Services.Cache;

namespace CodeGeneration.Modules
{
    public class CachingModule : Module
    {
        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            if (registration.Services.Any(s => s.Description == typeof(ICacheService).FullName)) return;

            registration.Preparing += (sender, e) =>
            {
                var callerType = e.Component.Activator.LimitType;
                e.Parameters = e.Parameters.Union(new[] {
                    new ResolvedParameter(
                        (pi, c) => pi.ParameterType == typeof(ICacheService) && pi.Name.Equals("cacheService"),
                        (pi, c) => c.Resolve<ICacheService>(new NamedParameter("callerType", callerType))),
                });
            };
        }
    }
}
