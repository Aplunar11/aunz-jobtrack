using JobTrack.Models;
using JobTrack.Models.JobCoversheet;
using JobTrack.Models.QuerySTP;
using JobTrack.Services.Interfaces;
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

        public CodingSTPController(ISTPService sTPService
            , IQuerySTPService querySTPService
            , IJobCoversheetService jobCoversheetService)
        {
            _sTPService = sTPService;
            _querySTPService = querySTPService;
            _jobCoversheetService = jobCoversheetService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MainForm()
        {
            ViewBag.MyJobs = 0;
            ViewBag.OpenJobs = 0;
            ViewBag.CompleteJobs = 0;
            ViewBag.CancelledJobs = 0;
            ViewBag.LateJobs = 0;
            ViewBag.DueJobs = 0;
            ViewBag.RevisedJobs = 0;

            return View();
        }

        public async Task<ActionResult> GetSTPData()
        {
            var mdata = await _querySTPService.GetAllSTPDataAsync();

            return Json(mdata, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> _AddNewSTPView(string coversheetids, string bpsproductid, string serviceno)
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

        public async Task<ActionResult> AddNewSTP(STPDataModel model)
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
    }
}