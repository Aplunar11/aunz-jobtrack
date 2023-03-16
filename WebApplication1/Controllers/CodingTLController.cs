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
using System.Threading.Tasks;

namespace JobTrack.Controllers
{
    public class CodingTLController : Controller
    {
        // CONNECTION STRING
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();
        public ActionResult TopMenu()
        {
            return PartialView("_Topbar");
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

            ViewBag.MyJobs = 0;
            ViewBag.OpenJobs = 0;
            ViewBag.CompleteJobs = 0;
            ViewBag.CancelledJobs = 0;
            ViewBag.LateJobs = 0;
            ViewBag.DueJobs = 0;
            ViewBag.RevisedJobs = 0;

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
    }
}