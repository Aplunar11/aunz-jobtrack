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
        private readonly IQuerySTPService _querySTPService;

        public CodingSTPController(IQuerySTPService querySTPService)
        {
            _querySTPService = querySTPService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MainForm()
        {
            return View();
        }

        public async Task<ActionResult> GetSTPData()
        {
            var mdata = await _querySTPService.GetAllSTPDataAsync();

            return Json(mdata, JsonRequestBehavior.AllowGet);
        }
    }
}