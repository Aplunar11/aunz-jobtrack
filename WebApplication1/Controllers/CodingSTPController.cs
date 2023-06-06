using JobTrack.Models;
using JobTrack.Models.Enums;
using JobTrack.Models.Extensions;
using JobTrack.Models.JobCoversheet;
using JobTrack.Models.QuerySTP;
using JobTrack.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JobTrack.Controllers
{
    public class CodingSTPController : Controller
    {
        private readonly ISTPService _sTPService;
        private readonly IQuerySTPService _querySTPService;
        private readonly IJobCoversheetService _jobCoversheetService;
        private readonly IJobDashboardService _jobDashboardService;
        private readonly ICoversheetService _coversheetService;
        private readonly IEmployeeService _employeeService;
        private readonly INotificationService _notificationService;

        public CodingSTPController(ISTPService sTPService
            , IQuerySTPService querySTPService
            , IJobCoversheetService jobCoversheetService
            , IJobDashboardService jobDashboardService
            , ICoversheetService coversheetService
            , IEmployeeService employeeService
            , INotificationService notificationService
            , IHistoryTrailService historyTrailService)
        {
            _sTPService = sTPService;
            _querySTPService = querySTPService;
            _jobCoversheetService = jobCoversheetService;
            _jobDashboardService = jobDashboardService;
            _coversheetService = coversheetService;
            _employeeService = employeeService;
            _notificationService = notificationService;
        }

        public ActionResult Index()
        {
            return View();
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

        public async Task<ActionResult> GetSTPData(int userAccessType)
        {
            var username = (string)Session["UserName"];
            var mdata = await _querySTPService.GetAllSTPDataAsync(username, (UserAccessEnum)userAccessType);

            return Json(mdata, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> _AddNewSTPView(string coversheetids
            , string bpsproductid
            , string serviceno
            , int userAccessType)
        {
            var maxID = await _sTPService.GetSTPMaxIDAsync();

            var jobCoversheet = await _jobCoversheetService.GetJobCoversheetDataByProductAndServiceAsync(new JobCoversheetData
            {
                BPSProductID = bpsproductid,
                ServiceNumber = serviceno
            });

            var viewModel = new STPDataModel
            {
                SendToPrintNumber = $"STP{(maxID + 1).ToString().PadLeft(5, '0')}",
                SendToPrintTier = jobCoversheet.CoversheetTier,
                BPSProductID = bpsproductid,
                ServiceNumber = serviceno,
                TargetPressDate = jobCoversheet.TargetPressDate,
                CoversheetIDs = coversheetids
            };

            return PartialView(viewModel);
        }

        public async Task<ActionResult> _EditSTPView(int StpID, UserAccessEnum userAccessType)
        {
            var model = await _sTPService.GetSTPDataByIDAsync(StpID);
            var owners = await _employeeService.GetAllEmployeeByAccess(UserAccessEnum.CodingSTP);
            model.IsCodingSTP = ((string)Session["UserAccess"]).ToUserAccessEnum() == UserAccessEnum.CodingSTP;

            model.IsConsoHighlighReadOnly = model.IsConsoHighlight && model.IsCodingSTP ? false : model.IsCodingSTP;
            model.IsFilingInstructionReadOnly = model.IsFilingInstruction && model.IsCodingSTP ? false : model.IsCodingSTP;
            model.IsDummyFiling1ReadOnly = model.IsDummyFiling1 && model.IsCodingSTP ? false : model.IsCodingSTP;
            model.IsDummyFiling2ReadOnly = model.IsDummyFiling2 && model.IsCodingSTP ? false : model.IsCodingSTP;
            model.IsUECJReadOnly = model.IsUECJ && model.IsCodingSTP ? false : model.IsCodingSTP;
            model.IsPC1PC2ReadOnly = model.IsPC1PC2 && model.IsCodingSTP ? false : model.IsCodingSTP;
            model.IsReadyToPrintReadOnly = model.IsReadyToPrint && model.IsCodingSTP ? false : model.IsCodingSTP;
            model.IsSendingFinalPagesReadOnly = model.IsSendingFinalPages && model.IsCodingSTP ? false : model.IsCodingSTP;
            model.IsPostBackReadOnly = model.IsPostBack && model.IsCodingSTP ? false : model.IsCodingSTP;
            model.IsUpdateEBinderReadOnly = model.IsUpdateEBinder && model.IsCodingSTP ? false : model.IsCodingSTP;

            ViewBag.UserAccess = userAccessType;
            TempData["JobOwners"] = new SelectList(owners, "ID", "UserName", model.OwnerUserID);
            return PartialView(model);
        }

        public async Task<ActionResult> UpdateSTP(STPDataModel model)
        {
            var result = new JsonResultModel();
            var username = (string)Session["UserName"];

            try
            {
                result.Collection = await _sTPService.InsertSendToPrintAsync(model, username);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<ActionResult> GetEmployeeEmail(int id)
        {
            var owners = await _employeeService.GetAllEmployeeByAccess(UserAccessEnum.CodingSTP);
            return Json(owners.FirstOrDefault(x => x.ID == id).EmailAddress, JsonRequestBehavior.AllowGet);
        }
    }
}