using JobTrack.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JobTrack.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GenerateReport()
        {
            return View();
        }

        public async Task<ActionResult> GetAllManuscriptData()
        {
            var mdata = await _reportService.GetAllManuscriptDataAsync();
            return Json(mdata, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAllCoversheetData()
        {
            var mdata = await _reportService.GetAllCoversheetDataAsync();
            return Json(mdata, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAllSTPData()
        {
            var mdata = await _reportService.GetAllSTPDataAsync();
            return Json(mdata, JsonRequestBehavior.AllowGet);
        }
    }
}