using JobTrack.Models.Enums;
using JobTrack.Models.Extensions;
using JobTrack.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JobTrack.Controllers
{
    public class CodingSTPTLController : Controller
    {
        private readonly ICoversheetService _coversheetService;
        private readonly IJobDashboardService _jobDashboardService;

        public CodingSTPTLController(ICoversheetService coversheetService, IJobDashboardService jobDashboardService)
        {
            _coversheetService = coversheetService;
            _jobDashboardService = jobDashboardService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SideMenu()
        {
            return PartialView("_SidebarRegular");
        }

        public async Task<ActionResult> MainForm()
        {
            // relogin for new session
            if (Session["UserName"] == null)
            {
                TempData["alertMessage"] = "You must log in to continue";
                return RedirectToAction("Login", "Login");
            }

            var username = (string)Session["UserName"];
            var productsAndServices = await _coversheetService.GetAllProductAndServiceByUsernameAsync(username, UserAccessEnum.CodingSTP);
            var productIds = string.Join(",", productsAndServices.Select(x => x.BPSProductID));
            var serviceNumbers = string.Join(",", productsAndServices.Select(x => x.ServiceNumber));

            ViewBag.MyJobs = await _jobDashboardService.GetAllMyJobsByProductAndServiceAsync(productIds, serviceNumbers, UserAccessEnum.CodingSTP);
            ViewBag.OpenJobs = await _jobDashboardService.GetAllJobsByProductAndServiceAndStatusAsync(productIds, serviceNumbers, CodingStatusEnum.New, UserAccessEnum.CodingSTP);
            ViewBag.CompleteJobs = await _jobDashboardService.GetAllJobsByProductAndServiceAndStatusAsync(productIds, serviceNumbers, CodingStatusEnum.Completed, UserAccessEnum.CodingSTP);
            ViewBag.CancelledJobs = await _jobDashboardService.GetAllJobsByProductAndServiceAndStatusAsync(productIds, serviceNumbers, CodingStatusEnum.Cancelled, UserAccessEnum.CodingSTP);
            ViewBag.LateJobs = await _jobDashboardService.GetAllJobsByProductAndServiceAndDueStatus(productIds, serviceNumbers, CodingStatusEnum.Late, UserAccessEnum.CodingSTP);
            ViewBag.DueJobs = await _jobDashboardService.GetAllJobsByProductAndServiceAndDueStatus(productIds, serviceNumbers, CodingStatusEnum.Due, UserAccessEnum.CodingSTP);
            ViewBag.RevisedJobs = await _jobDashboardService.GetAllJobsByProductAndServiceAndDueStatus(productIds, serviceNumbers, CodingStatusEnum.Revised, UserAccessEnum.CodingSTP);

            return View();
        }
    }
}