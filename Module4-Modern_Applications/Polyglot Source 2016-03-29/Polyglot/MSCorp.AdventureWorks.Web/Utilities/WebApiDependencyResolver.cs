using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http.Dependencies;
using Common;
using Microsoft.Practices.Unity;

namespace MSCorp.AdventureWorks.Web.Utilities
{
    /// <summary>
    /// bridge class to allow Unity IoC in 
    /// </summary>
    public class WebApiDependencyResolver : IDependencyResolver
    {
        private IUnityContainer _container;

        public WebApiDependencyResolver(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            _container = container;
        }

        [DebuggerStepThrough]
        public object GetService(Type serviceType)
        {
            try
            {
                return _container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return _container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }


        public IDependencyScope BeginScope()
        {
            var child = _container.CreateChildContainer();
            return new WebApiDependencyResolver(child);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_container != null)
                {
                    _container.Dispose();
                }
            }
        }

    }
}