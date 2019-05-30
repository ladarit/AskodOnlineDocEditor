using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;
using Microsoft.AspNet.SignalR;

namespace AskodOnline.Editor
{
    public class SignalRDependencyResolver : DefaultDependencyResolver
    {
        private readonly IKernel _kernel;

        public SignalRDependencyResolver(IKernel kernel)
        {
            _kernel = kernel;
        }

        public override object GetService(Type serviceType)
        {
            //check if component exists in container, if not - use base to resolve
            return _kernel.HasComponent(serviceType) ? _kernel.Resolve(serviceType) : base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            var objects = _kernel.HasComponent(serviceType) ? _kernel.ResolveAll(serviceType).Cast<object>() : new object[] { };
            return objects.Concat(base.GetServices(serviceType));
        }
    }
}