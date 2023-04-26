using JobTrack.Models;
using JobTrack.Models.Enums;
using JobTrack.Models.JobReassignment;
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
        private readonly IEmployeeService _employeeService;

        public JobReassignmentController(IJobReassignmentService jobReassignmentService, IEmployeeService employeeService)
        {
            _jobReassignmentService = jobReassignmentService;
            _employeeService = employeeService;
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

        public async Task<ActionResult> _EditJobReassignmentView(JobReassignmentModel viewModel)
        {
            var owners = await _employeeService.GetAllEmployeeByAccess(UserAccessEnum.Client_LE);
            TempData["JobOwners"] = new SelectList(owners, "UserName", "UserName", viewModel.ValueAfter);

            return PartialView(await Task.FromResult(viewModel));
        }

        public async Task<ActionResult> GetJobReassignmentByRole(UserAccessEnum userAccess)
        {
            var result = await _jobReassignmentService.GetAllJobReassignment();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAllJobReassignmentByUser()
        {
            var userName = (string)Session["UserName"];
            var result = await _jobReassignmentService.GetAllJobReassignmentByUser(userName);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> UpdateJobReassignment(JobReassignmentModel model)
        {
            var result = new JsonResultModel();
            var userName = (string)Session["UserName"];

            try
            {
                if (model.NewOwner != model.ValueAfter)
                    result.IsSuccess = await _jobReassignmentService.UpdateJobReassignment(model, userName);

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return Json(result);
        }
    }
}