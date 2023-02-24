using JobTrack.Services;
using JobTrack.Services.Interfaces;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace JobTrack
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<IQueryManuscriptService, QueryManuscriptService>();
            container.RegisterType<IQueryCoversheetService, QueryCoversheetService>();
            
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}