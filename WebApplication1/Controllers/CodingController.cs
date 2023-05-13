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
using JobTrack.Services.Interfaces;
using JobTrack.Models.Extensions;

namespace JobTrack.Controllers
{
    public class CodingController : Controller
    {
        // CONNECTION STRING
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        private readonly INotificationService _notificationService;

        public CodingController(INotificationService notificationService)
        {
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
            var userAccess = ((string)Session["UserAccess"]).ToUserAccessEnum();
            ViewBag.UserAccess = userAccess;

            return PartialView("_SidebarRegular");
        }

        public ActionResult MainForm()
        {
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

        public ActionResult GetJobTrackData()
        {
            //#region Check Session
            //if (Session["Username"] == null)
            //{
            //    TempData["Message"] = "You must log in to continue";
            //    TempData["Toastr"] = "error";
            //    return RedirectToAction("Login", "Login");
            //}
            //#endregion
            if (dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();

            List<JobData> mdata = new List<JobData>();
            DataTable dt = new DataTable();

            cmd = new MySqlCommand("GetAllJobData", dbConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@int_owner", owner);
            adp = new MySqlDataAdapter(cmd);
            adp.Fill(dt);
            //DateTime? temp = null; //this is fine
            //var indexOfYourColumn = dt.Columns.IndexOf(dt.Columns[6]);
            foreach (DataRow dr in dt.Rows)
            {
                //temp = dr[indexOfYourColumn] != DBNull.Value ? (DateTime?)null : DateTime.Parse(dr[indexOfYourColumn].ToString());
                mdata.Add(new JobData
                {


                    JobID = Convert.ToInt32(dr["JobID"].ToString()),
                    //JobNumber = Convert.ToInt32(dr["JobNumber"].ToString()),
                    JobNumber = dr["JobNumber"].ToString().PadLeft(8, '0'),
                    ManuscriptTier = dr["ManuscriptTier"].ToString(),
                    BPSProductID = dr["BPSProductID"].ToString(),
                    ServiceNumber = dr["ServiceNumber"].ToString(),
                    TargetPressDate = Convert.ToDateTime(dr["TargetPressDate"].ToString()),

                    ActualPressDate = dr.Field<DateTime?>("ActualPressDate"),
                    //TargetPressDate = DateTime.ParseExact(dr["TargetPressDate"].ToString(), "yyyy/MM/dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                    //ActualPressDate = DateTime.ParseExact(dr["ActualPressDate"].ToString(), "yyyy/MM/dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                    CopyEditStatus = dr["CopyEditStatus"].ToString(),
                    CodingStatus = dr["CodingStatus"].ToString(),
                    OnlineStatus = dr["OnlineStatus"].ToString(),
                    STPStatus = dr["STPStatus"].ToString()

                });
            }
            dbConnection.Close();
            return Json(mdata, JsonRequestBehavior.AllowGet);
        }
    }
}