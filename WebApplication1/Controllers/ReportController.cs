using JobTrack.Models;
using JobTrack.Models.Coversheet;
using JobTrack.Models.Enums;
using JobTrack.Models.Extensions;
using JobTrack.Models.Manuscript;
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

        public async Task<ActionResult> GenerateReport()
        {
            if (Session["UserName"] == null)
            {
                TempData["alertMessage"] = "You must log in to continue";
                return RedirectToAction("Login", "Login");
            }

            var manuscript = await _reportService.GetFilterParameterManuscriptAsync();
            var manuscriptFilter = await _reportService.GetReportFilterManuscriptAsync(ReportFilterManuscriptEnum.BPSProductID);
            var coversheet = await _reportService.GetFilterParameterCoversheetAsync();
            var coversheetFilter = await _reportService.GetReportFilterCoversheetAsync(ReportFilterCoversheetEnum.CodingDueDate);
            var stp = await _reportService.GetFilterParameterSTPAsync();
            var stpFilter = await _reportService.GetReportFilterSTPAsync(ReportFilterSTPEnum.BPSProductID);

            TempData["Manuscript"] = new SelectList(manuscript, "Value", "Name", null);
            TempData["Coversheet"] = new SelectList(coversheet, "Value", "Name", null);
            TempData["STP"] = new SelectList(stp, "Value", "Name", null);
            TempData["ManuscriptFilter"] = new SelectList(manuscriptFilter, "FilterValue", "FilterValue", null);
            TempData["CoversheetFilter"] = new SelectList(coversheetFilter, "FilterValue", "FilterValue", null);
            TempData["STPFilter"] = new SelectList(stpFilter, "FilterValue", "FilterValue", null);

            return View();
        }

        public ActionResult _ManuscriptView()
        {
            return PartialView();
        }

        public ActionResult _CoversheetView()
        {
            return PartialView();
        }

        public ActionResult _SendToPrintView()
        {
            return PartialView();
        }

        public ActionResult _FilterView()
        {
            return PartialView();
        }

        public async Task<ActionResult> GetAllManuscriptData(object selected, ReportFilterManuscriptEnum filterBy)
        {
            var model = new ManuscriptData();

            switch (filterBy)
            {
                case ReportFilterManuscriptEnum.BPSProductID:
                    model.BPSProductID = (string)selected.ToArray().FirstOrDefault();
                    break;

                case ReportFilterManuscriptEnum.ServiceNumber:
                    model.ServiceNumber = (string)selected.ToArray().FirstOrDefault();
                    break;

                case ReportFilterManuscriptEnum.UpdateType:
                    model.UpdateType = (string)selected.ToArray().FirstOrDefault();
                    break;

                case ReportFilterManuscriptEnum.CopyEditDueDate:
                    model.CopyEditDueDate = selected.ToArray().FirstOrDefault().ToDateValue();
                    break;
            }

            // set "filterBy" to default if "selected" item is "Select"
            filterBy = selected is string[] && (string)selected.ToArray().FirstOrDefault() == "Select"
                ? ReportFilterManuscriptEnum.Select
                : filterBy;

            var list = await _reportService.GetAllManuscriptDataAsync(model, filterBy);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAllCoversheetData(object selected, ReportFilterCoversheetEnum filterBy)
        {
            var model = new CoversheetData();

            switch (filterBy)
            {
                case ReportFilterCoversheetEnum.CodingDueDate:
                    model.CodingDueDate = selected.ToArray().FirstOrDefault().ToDateValue();
                    break;

                case ReportFilterCoversheetEnum.OnlineDueDate:
                    model.OnlineDueDate = selected.ToArray().FirstOrDefault().ToDateValue();
                    break;

                case ReportFilterCoversheetEnum.OnlineTimeliness:
                    model.OnlineTimeliness = (string)selected.ToArray().FirstOrDefault();
                    break;
            }

            // set "filterBy" to default if "selected" item is "Select"
            filterBy = selected is string[] && (string)selected.ToArray().FirstOrDefault() == "Select"
                ? ReportFilterCoversheetEnum.Select
                : filterBy;

            var list = await _reportService.GetAllCoversheetDataAsync(model, filterBy);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAllSTPData(object selected, ReportFilterSTPEnum filterBy)
        {
            var model = new STPDataModel();

            switch (filterBy)
            {
                case ReportFilterSTPEnum.BPSProductID:
                    model.BPSProductID = (string)selected.ToArray().FirstOrDefault();
                    break;

                case ReportFilterSTPEnum.ServiceNumber:
                    model.ServiceNumber = (string)selected.ToArray().FirstOrDefault();
                    break;

                case ReportFilterSTPEnum.SendToPrintStatus:
                    model.SendToPrintStatus = (string)selected.ToArray().FirstOrDefault();
                    break;
            }

            // set "filterBy" to default if "selected" item is "Select"
            filterBy = selected is string[] && (string)selected.ToArray().FirstOrDefault() == "Select"
                ? ReportFilterSTPEnum.Select
                : filterBy;

            var list = await _reportService.GetAllSTPDataAsync(model, filterBy);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetFilterManuscriptValues(ReportFilterManuscriptEnum filter)
        {
            var list = await _reportService.GetReportFilterManuscriptAsync(filter);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetFilterCoversheetValues(ReportFilterCoversheetEnum filter)
        {
            var list = await _reportService.GetReportFilterCoversheetAsync(filter);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetFilterSTPValues(ReportFilterSTPEnum filter)
        {
            var list = await _reportService.GetReportFilterSTPAsync(filter);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}