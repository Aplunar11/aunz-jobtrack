using JobTrack.Models.Enums;
using JobTrack.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JobTrack.Controllers
{
    public class JobReassignmentController : Controller
    {
        private readonly IJobReassignmentService _jobReassignmentService;

        public JobReassignmentController(IJobReassignmentService jobReassignmentService)
        {
            _jobReassignmentService = jobReassignmentService;
        }

        public ActionResult Index()
        {
            // relogin for new session
            if (Session["UserName"] == null)
            {
                TempData["alertMessage"] = "You must log in to continue";
                return RedirectToAction("Login", "Login");
            }

            return View();
        }

        public async Task<ActionResult> GetJobReassignmentByRole(UserAccessEnum userAccess)
        {
            var result = await _jobReassignmentService.GetAllJobReassignment();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}