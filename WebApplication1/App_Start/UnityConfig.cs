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
            container.RegisterType<ICoversheetService, CoversheetService>();
            container.RegisterType<IJobDashboardService, JobDashboardService>();
            container.RegisterType<ISTPService, STPService>();
            container.RegisterType<IHistoryTrailService, HistoryTrailService>();
            container.RegisterType<IReportService, ReportService>();
            container.RegisterType<INotificationService, NotificationService>();
            container.RegisterType<IParameterService, ParameterService>();
            container.RegisterType<IJobReassignmentService, JobReassignmentService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}