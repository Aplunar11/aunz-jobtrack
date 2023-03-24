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

            ViewBag.MyJobs = 0;
            ViewBag.OpenJobs = 0;
            ViewBag.CompleteJobs = 0;
            ViewBag.CancelledJobs = 0;
            ViewBag.LateJobs = 0;
            ViewBag.DueJobs = 0;
            ViewBag.RevisedJobs = 0;

            return View();
        }
    }
}