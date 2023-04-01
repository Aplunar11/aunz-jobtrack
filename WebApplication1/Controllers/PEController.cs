using System;
using JobTrack.Models;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using JobTrack.Models.Job;
using JobTrack.Models.Coversheet;
using MySql.Data.MySqlClient;
using System.Globalization;
using JobTrack.Services.Interfaces;
using System.Threading.Tasks;
using JobTrack.Models.Enums;

namespace JobTrack.Controllers
{
    public class PEController : Controller
    {
        private readonly IJobDashboardService _jobDashboardService;
        private readonly ICoversheetService _coversheetService;
        private readonly INotificationService _notificationService;

        public PEController(IJobDashboardService jobDashboardService
            , ICoversheetService coversheetService
            , INotificationService notificationService)
        {
            _jobDashboardService = jobDashboardService;
            _coversheetService = coversheetService;
            _notificationService = notificationService;
        }

        public async Task<ActionResult> TopMenu()
        {
            var userName = (string)Session["UserName"];
            var notifications = await _notificationService.GetNoticationByUserAsync(userName);
            var viewModel = new TopMenuModel
            {
                UnreadCount = notifications.Where(x => !x.IsViewed).Count(),
                Notifications = notifications
            };

            return PartialView("_Topbar", viewModel);
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
            var productsAndServices = await _coversheetService.GetAllProductAndServiceByUsernameAsync(username, UserAccessEnum.Straive_PE);
            var productIds = string.Join(",", productsAndServices.Select(x => x.BPSProductID));
            var serviceNumbers = string.Join(",", productsAndServices.Select(x => x.ServiceNumber));

            ViewBag.MyJobs = await _jobDashboardService.GetAllMyJobsByProductAndServiceAsync(productIds, serviceNumbers, UserAccessEnum.Straive_PE);
            ViewBag.OpenJobs = await _jobDashboardService.GetAllJobsByProductAndServiceAndStatusAsync(productIds, serviceNumbers, CodingStatusEnum.New, UserAccessEnum.Straive_PE);
            ViewBag.CompleteJobs = await _jobDashboardService.GetAllJobsByProductAndServiceAndStatusAsync(productIds, serviceNumbers, CodingStatusEnum.Completed, UserAccessEnum.Straive_PE);
            ViewBag.CancelledJobs = await _jobDashboardService.GetAllJobsByProductAndServiceAndStatusAsync(productIds, serviceNumbers, CodingStatusEnum.Cancelled, UserAccessEnum.Straive_PE);
            ViewBag.LateJobs = await _jobDashboardService.GetAllJobsByProductAndServiceAndDueStatus(productIds, serviceNumbers, CodingStatusEnum.Late, UserAccessEnum.Straive_PE);
            ViewBag.DueJobs = await _jobDashboardService.GetAllJobsByProductAndServiceAndDueStatus(productIds, serviceNumbers, CodingStatusEnum.Due, UserAccessEnum.Straive_PE);
            ViewBag.RevisedJobs = await _jobDashboardService.GetAllJobsByProductAndServiceAndDueStatus(productIds, serviceNumbers, CodingStatusEnum.Revised, UserAccessEnum.Straive_PE);

            return View();
        }

        public ActionResult AllJob()
        {
            #region Check Session
            if (Session["UserName"] == null)
            {
                TempData["alertMessage"] = "You must log in to continue";
                return RedirectToAction("Login", "Login");
            }
            #endregion
            return View();
        }

        public ActionResult PEIndex()
        {
            return View();
        }
    }
}