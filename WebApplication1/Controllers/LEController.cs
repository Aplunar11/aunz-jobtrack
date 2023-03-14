﻿using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using JobTrack.Models.Job;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using JobTrack.Services.Interfaces;
using JobTrack.Models.Enums;
using System.Linq;

namespace JobTrack.Controllers
{
    public class LEController : Controller
    {
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        private readonly IJobDashboardService _jobDashboardService;
        private readonly ICoversheetService _coversheetService;

        public LEController(IJobDashboardService jobDashboardService, ICoversheetService coversheetService)
        {
            _jobDashboardService = jobDashboardService;
            _coversheetService = coversheetService;
        }

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

            //var username = (string)Session["UserName"];
            //var productsAndServices = await _coversheetService.GetAllProductAndServiceByUsernameAsync(username, UserAccessEnum.Client_LE);
            //var productIds = string.Join(",", productsAndServices.Select(x => x.BPSProductID).Distinct());
            //var serviceNumbers = string.Join(",", productsAndServices.Select(x => x.ServiceNumber));

            //ViewBag.MyJobs = await _jobDashboardService.GetAllMyJobsByProductAndServiceAsync(productIds, serviceNumbers, UserAccessEnum.Client_LE);

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
        //public ActionResult GetJobTrackData()
        //{
        //    if (dbConnection.State == ConnectionState.Closed)
        //        dbConnection.Open();

        //    List<JobData> mdata = new List<JobData>();
        //    DataTable dt = new DataTable();

        //    cmd = new MySqlCommand("GetAllJobData", dbConnection);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.Clear();
        //    adp = new MySqlDataAdapter(cmd);
        //    adp.Fill(dt);
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        mdata.Add(new JobData
        //        {


        //            JobID = Convert.ToInt32(dr["JobID"].ToString()),
        //            JobNumber = dr["JobNumber"].ToString().PadLeft(8, '0'),
        //            ManuscriptTier = dr["ManuscriptTier"].ToString(),
        //            BPSProductID = dr["BPSProductID"].ToString(),
        //            ServiceNumber = dr["ServiceNumber"].ToString(),
        //            TargetPressDate = Convert.ToDateTime(dr["TargetPressDate"].ToString()),

        //            ActualPressDate = dr.Field<DateTime?>("ActualPressDate"),
        //            //TargetPressDate = DateTime.ParseExact(dr["TargetPressDate"].ToString(), "yyyy/MM/dd hh:mm:ss tt", CultureInfo.InvariantCulture),
        //            //ActualPressDate = DateTime.ParseExact(dr["ActualPressDate"].ToString(), "yyyy/MM/dd hh:mm:ss tt", CultureInfo.InvariantCulture),
        //            CopyEditStatus = dr["CopyEditStatus"].ToString(),
        //            CodingStatus = dr["CodingStatus"].ToString(),
        //            OnlineStatus = dr["OnlineStatus"].ToString(),
        //            STPStatus = dr["STPStatus"].ToString()

        //        });
        //    }
        //    dbConnection.Close();
        //    return Json(mdata, JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult GetJobTrackDataByUserName()
        //{
        //    var Username = Session["UserName"];
        //    if (dbConnection.State == ConnectionState.Closed)
        //        dbConnection.Open();

        //    List<JobData> mdata = new List<JobData>();
        //    DataTable dt = new DataTable();

        //    cmd = new MySqlCommand("GetAllJobDataByUserName", dbConnection);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.Clear();
        //    cmd.Parameters.AddWithValue("@p_Username", Username);
        //    adp = new MySqlDataAdapter(cmd);
        //    adp.Fill(dt);
        //    //DateTime? temp = null; //this is fine
        //    //var indexOfYourColumn = dt.Columns.IndexOf(dt.Columns[6]);
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        //temp = dr[indexOfYourColumn] != DBNull.Value ? (DateTime?)null : DateTime.Parse(dr[indexOfYourColumn].ToString());
        //        mdata.Add(new JobData
        //        {


        //            JobID = Convert.ToInt32(dr["JobID"].ToString()),
        //            //JobNumber = Convert.ToInt32(dr["JobNumber"].ToString()),
        //            JobNumber = dr["JobNumber"].ToString().PadLeft(8, '0'),
        //            ManuscriptTier = dr["ManuscriptTier"].ToString(),
        //            BPSProductID = dr["BPSProductID"].ToString(),
        //            ServiceNumber = dr["ServiceNumber"].ToString(),
        //            TargetPressDate = Convert.ToDateTime(dr["TargetPressDate"].ToString()),

        //            ActualPressDate = dr.Field<DateTime?>("ActualPressDate"),
        //            //TargetPressDate = DateTime.ParseExact(dr["TargetPressDate"].ToString(), "yyyy/MM/dd hh:mm:ss tt", CultureInfo.InvariantCulture),
        //            //ActualPressDate = DateTime.ParseExact(dr["ActualPressDate"].ToString(), "yyyy/MM/dd hh:mm:ss tt", CultureInfo.InvariantCulture),
        //            CopyEditStatus = dr["CopyEditStatus"].ToString(),
        //            CodingStatus = dr["CodingStatus"].ToString(),
        //            OnlineStatus = dr["OnlineStatus"].ToString(),
        //            STPStatus = dr["STPStatus"].ToString()

        //        });
        //    }
        //    dbConnection.Close();
        //    return Json(mdata, JsonRequestBehavior.AllowGet);
        //}
    }
}