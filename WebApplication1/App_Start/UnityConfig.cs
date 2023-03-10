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
            container.RegisterType<IQuerySTPService, QuerySTPService>();
            container.RegisterType<IPublicationAssignService, PublicationAssignService>();
            container.RegisterType<IEmployeeService, EmployeeService>();
            container.RegisterType<IJobdataService, JobdataService>();
            container.RegisterType<ITransactionLogService, TransactionLogService>();
            container.RegisterType<IManuscriptDataService, ManuscriptDataService>();
            container.RegisterType<IPubSchedService, PubSchedService>();
            container.RegisterType<IJobCoversheetService, JobCoversheetService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}