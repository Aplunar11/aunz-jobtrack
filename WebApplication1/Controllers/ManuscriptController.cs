﻿using System;
using JobTrack.Models;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using DataTables.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using JobTrack.Models.Manuscript;
using JobTrack.Models.Job;
using JobTrack.Services.Interfaces;
using JobTrack.Models.Extensions;

namespace JobTrack.Controllers
{
    public class ManuscriptController : Controller
    {
        // CONNECTION STRING
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        private readonly IManuscriptDataService _manuscriptDataService;

        public ManuscriptController(IManuscriptDataService manuscriptDataService)
        {
            _manuscriptDataService = manuscriptDataService;
        }

        public async Task<ActionResult> _EditManuscriptDataView(int id)
        {
            var viewModel = await _manuscriptDataService.GetManuscriptByIdAsync(id);
            var userAccess = ((string)Session["UserAccess"]).ToUserAccessEnum();
            TempData["UpdateTypes"] = new SelectList(GetAllTurnAroundTime(), "UpdateType", "UpdateType", viewModel.UpdateType);
            ViewBag.UserAccess = userAccess;

            return PartialView(viewModel);
        }

        public async Task<ActionResult> UpdateManuscript(ManuscriptData model)
        {
            var userName = (string)Session["UserName"];
            var result = await _manuscriptDataService.UpdateManuscriptDataAsync(model, userName);

            return Json(result);
        }

        [HttpGet]
        public ActionResult AddNewManuscript()
        {
            ManuscriptData mdata = new ManuscriptData();
            //LastManuscriptID mid = new LastManuscriptID();
            //this.ViewBag.Service = new SelectList(mid.GetLastManuscriptID(), "service_id", "service_no");
            try
            {
                mdata.DateCreated = DateTime.Now;
                //TempData["ManuscriptTier"] = new SelectList(GetAllPubschedTier(), "PubSchedTier", "PubSchedTier");
                TempData["BPSProductID"] = new SelectList(GetAllPubschedBPSProductID(), "PubschedBPSProductID", "PubschedBPSProductID");
                //ConfigureViewModel(mdata);
                TempData["UpdateType"] = new SelectList(GetAllTurnAroundTime(), "TurnAroundTimeID", "UpdateType");
                return PartialView(mdata);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ErrorMessage", ex.Message);
                mdata.ErrorMessage = ex.Message;
                return PartialView(mdata);
            }
        }

        public List<Models.Manuscript.GetPubschedBPSProductID> GetAllPubschedBPSProductID()
        {
            DataTable dt = new DataTable();

            cmd = new MySqlCommand("GetAllPubschedBPSProductID", dbConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            adp = new MySqlDataAdapter(cmd);
            adp.Fill(dt);

            if (dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();

            List<Models.Manuscript.GetPubschedBPSProductID> lst = new List<Models.Manuscript.GetPubschedBPSProductID>();
            foreach (DataRow dr in dt.Rows)
            {
                lst.Add(new Models.Manuscript.GetPubschedBPSProductID
                {
                    PubschedBPSProductID = Convert.ToString(dr[0])

                });
            }
            return lst;
        }
        [HttpPost]
        public JsonResult GetAllPubschedServiceNumber(string bpsproductid, string servicenumber)
        {

            if (dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();

            List<Models.Manuscript.GetAllPubschedServiceNumber> lst = new List<Models.Manuscript.GetAllPubschedServiceNumber>();

            string storedProcName;
            storedProcName = "GetAllPubschedServiceNumber";

            using (MySqlCommand command = new MySqlCommand(storedProcName, dbConnection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_BPSProductID", bpsproductid);
                if (servicenumber == null)
                    command.Parameters.AddWithValue("@p_ServiceNumber", DBNull.Value);
                else
                    command.Parameters.AddWithValue("@p_ServiceNumber", servicenumber);

                MySqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        //DateTime d = Convert.ToDateTime(reader[3].ToString());
                        lst.Add(new Models.Manuscript.GetAllPubschedServiceNumber()
                        {
                            PubschedTier = reader[0].ToString(),
                            PubschedBPSProductID = reader[1].ToString(),
                            PubschedServiceNumber = reader[2].ToString(),
                            PubschedTargetPressDate = Convert.ToDateTime(reader[3].ToString())
                            //PubschedTargetPressDate = Convert.ToDateTime(reader[3], CultureInfo.CurrentCulture).ToString("yyyy-MM-dd")
                            //PubschedTargetPressDate = DateTime.ParseExact(reader[3].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture)
                        });
                    }
                }
                else
                {
                    lst = new List<Models.Manuscript.GetAllPubschedServiceNumber>();
                }
                reader.Close();
            }
            dbConnection.Close();

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public List<Models.Manuscript.GetAllTurnAroundTime> GetAllTurnAroundTime()
        {
            DataTable dt = new DataTable();

            cmd = new MySqlCommand("GetAllTurnAroundTime", dbConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            adp = new MySqlDataAdapter(cmd);
            adp.Fill(dt);

            if (dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();

            List<Models.Manuscript.GetAllTurnAroundTime> lst = new List<Models.Manuscript.GetAllTurnAroundTime>();
            foreach (DataRow dr in dt.Rows)
            {
                lst.Add(new Models.Manuscript.GetAllTurnAroundTime
                {
                    TurnAroundTimeID = Convert.ToInt32(dr[0]),
                    UpdateType = Convert.ToString(dr[1]),
                    TaskType = Convert.ToString(dr[2]),
                    TATCopyEdit = Convert.ToInt32(dr[3]),
                    TATCoding = Convert.ToInt32(dr[4]),
                    TATOnline = Convert.ToInt32(dr[6]),
                    BenchMarkDays = Convert.ToInt32(dr[7]),
                });
            }
            return lst;
        }
        public ActionResult GetTaskType(int selectedItem)
        {
            var data = GetAllTurnAroundTime().Where(model => model.TurnAroundTimeID == selectedItem).FirstOrDefault();

            return Json(data.TaskType, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTATCopyEdit(int selectedItem)
        {
            var data = GetAllTurnAroundTime().Where(model => model.TurnAroundTimeID == selectedItem).FirstOrDefault();
            DateTime d = AddBusinessDays(DateTime.Now, data.TATCopyEdit);
            return Json(d.ToString("yyyy-MM-dd"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTATCoding(int selectedItem)
        {
            var data = GetAllTurnAroundTime().Where(model => model.TurnAroundTimeID == selectedItem).FirstOrDefault();
            //DateTime d = DateTime.Now.AddDays(data.TATCoding);
            DateTime d = AddBusinessDays(DateTime.Now, data.TATCoding);
            return Json(d.ToString("yyyy-MM-dd"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTATOnline(int selectedItem)
        {
            var data = GetAllTurnAroundTime().Where(model => model.TurnAroundTimeID == selectedItem).FirstOrDefault();
            //DateTime d = DateTime.Now.AddDays(data.TATOnline);
            DateTime d = AddBusinessDays(DateTime.Now, data.TATOnline);
            return Json(d.ToString("yyyy-MM-dd"), JsonRequestBehavior.AllowGet);
        }

        public static DateTime AddBusinessDays(DateTime date, int days)
        {
            if (days < 0)
            {
                throw new ArgumentException("days cannot be negative", "days");
            }

            if (days == 0) return date;

            if (date.DayOfWeek == DayOfWeek.Saturday)
            {
                date = date.AddDays(2);
                days -= 1;
            }
            else if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
                days -= 1;
            }

            date = date.AddDays(days / 5 * 7);
            int extraDays = days % 5;

            if ((int)date.DayOfWeek + extraDays > 5)
            {
                extraDays += 2;
            }

            return date.AddDays(extraDays);

        }

        public JsonResult GetJobData(string servicenumber, string bpsproductid)
        {
            List<ManuscriptData> mdata = new List<ManuscriptData>();
            DataTable dt = new DataTable();

            cmd = new MySqlCommand("GetSpecificJobData", dbConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@p_BPSProductID", bpsproductid);
            cmd.Parameters.AddWithValue("@p_ServiceNumber", servicenumber);
            adp = new MySqlDataAdapter(cmd);
            adp.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                mdata.Add(new ManuscriptData
                {

                    ManuscriptLegTitle = dr["ManuscriptLegTitle"].ToString()
                    //ManuscriptID = Convert.ToInt32(dr["ManuscriptID"].ToString()),
                    //JobNumber = Convert.ToInt32(dr["JobNumber"].ToString()),
                    //ManuscriptTier = dr["ManuscriptTier"].ToString(),
                    //BPSProductID = dr["BPSProductID"].ToString(),
                    //ServiceNumber = dr["ServiceNumber"].ToString(),
                    //TargetPressDate = Convert.ToDateTime(dr["TargetPressDate"].ToString()),
                    //ActualPressDate = Convert.ToDateTime(dr["ActualPressDate"].ToString()),
                    ////TargetPressDate = DateTime.ParseExact(dr["TargetPressDate"].ToString(), "yyyy/MM/dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                    ////ActualPressDate = DateTime.ParseExact(dr["ActualPressDate"].ToString(), "yyyy/MM/dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                    //CopyEditStatus = dr["CopyEditStatus"].ToString(),
                    //CodingStatus = dr["CodingStatus"].ToString(),
                    //OnlineStatus = dr["OnlineStatus"].ToString(),
                    //STPStatus = dr["STPStatus"].ToString()

                });
            }
            return Json(mdata, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddNewManuscript(ManuscriptData mdata, JobData jdata)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(jdata.BPSProductID) && !string.IsNullOrEmpty(jdata.ServiceNumber))
                    {
                        var result = IsJobExists(jdata.BPSProductID, jdata.ServiceNumber);
                        if (result != null)
                        {
                            if (result.JobID == 0 || result.JobID < 0)
                            {
                                mdata.Response = "N";
                                mdata.ErrorMessage = "Entered invalid Product or Service Number";
                            }
                            else
                            {
                                var Username = Session["UserName"];
                                //var JobNumber = Session["JobNumber"];
                                MySqlCommand com = new MySqlCommand("InsertManuscript", dbConnection);
                                com.CommandType = CommandType.StoredProcedure;
                                com.Parameters.AddWithValue("@p_Username", Username);
                                //com.Parameters.AddWithValue("@p_JobNumber", JobNumber);
                                com.Parameters.AddWithValue("@p_ManuscriptTier", mdata.ManuscriptTier);
                                com.Parameters.AddWithValue("@p_BPSProductID", mdata.BPSProductID);
                                com.Parameters.AddWithValue("@p_ServiceNumber", mdata.ServiceNumber);
                                com.Parameters.AddWithValue("@p_ManuscriptLegTitle", mdata.ManuscriptLegTitle);
                                com.Parameters.AddWithValue("@p_TargetPressDate", mdata.TargetPressDate);
                                //com.Parameters.AddWithValue("@p_ActualPressDate", mdata.ActualPressDate);
                                com.Parameters.AddWithValue("@p_LatupAttribution", mdata.LatupAttribution);
                                com.Parameters.AddWithValue("@p_DateReceivedFromAuthor", mdata.DateReceivedFromAuthor);
                                com.Parameters.AddWithValue("@p_UpdateType", mdata.UpdateType);
                                com.Parameters.AddWithValue("@p_JobSpecificInstruction", mdata.JobSpecificInstruction);
                                com.Parameters.AddWithValue("@p_TaskType", mdata.TaskType);
                                com.Parameters.AddWithValue("@p_CopyEditDueDate", mdata.CopyEditDueDate);
                                //com.Parameters.AddWithValue("@p_CopyEditStatus", mdata.CopyEditStatus);
                                com.Parameters.AddWithValue("@p_CodingDueDate", mdata.CodingDueDate);
                                com.Parameters.AddWithValue("@p_OnlineDueDate", mdata.OnlineDueDate);
                                //com.Parameters.AddWithValue("@p_STPStatus", mdata.STPStatus);
                                if (dbConnection.State == ConnectionState.Closed)
                                    dbConnection.Open();
                                int Count = com.ExecuteNonQuery();

                                if (Count > 0)
                                {
                                    mdata.Response = "Y";

                                }
                                else
                                {
                                    mdata.Response = "N";
                                    mdata.ErrorMessage = "Manuscript data could not be added";
                                }

                            }
                        }
                        else
                        {
                            try
                            {
                                var Username = Session["UserName"];
                                MySqlCommand com = new MySqlCommand("InsertJob", dbConnection);
                                com.CommandType = CommandType.StoredProcedure;
                                com.Parameters.AddWithValue("@p_Username", Username);
                                com.Parameters.AddWithValue("@p_ManuscriptTier", mdata.ManuscriptTier);
                                com.Parameters.AddWithValue("@p_BPSProductID", mdata.BPSProductID);
                                com.Parameters.AddWithValue("@p_ServiceNumber", mdata.ServiceNumber);
                                com.Parameters.AddWithValue("@p_ManuscriptLegTitle", mdata.ManuscriptLegTitle);
                                com.Parameters.AddWithValue("@p_TargetPressDate", mdata.TargetPressDate);
                                com.Parameters.AddWithValue("@p_LatupAttribution", mdata.LatupAttribution);
                                com.Parameters.AddWithValue("@p_DateReceivedFromAuthor", mdata.DateReceivedFromAuthor);
                                com.Parameters.AddWithValue("@p_UpdateType", mdata.UpdateType);
                                com.Parameters.AddWithValue("@p_JobSpecificInstruction", mdata.JobSpecificInstruction);
                                com.Parameters.AddWithValue("@p_TaskType", mdata.TaskType);
                                com.Parameters.AddWithValue("@p_CopyEditDueDate", mdata.CopyEditDueDate);
                                com.Parameters.AddWithValue("@p_CodingDueDate", mdata.CodingDueDate);
                                com.Parameters.AddWithValue("@p_OnlineDueDate", mdata.OnlineDueDate);
                                if (dbConnection.State == ConnectionState.Closed)
                                    dbConnection.Open();
                                //int Count = com.ExecuteNonQuery();
                                mdata.Response = "Y";

                            }
                            catch
                            {
                                mdata.Response = "N";
                                mdata.ErrorMessage = "Job data could not be added";
                                //mdata.ErrorMessage = resultExecute;
                            }
                            finally
                            {
                                dbConnection.Close();
                            }

                            // Json(new { success = false, responseText = "registration failed, please check", JsonRequestBehavior.AllowGet);
                        }
                    }

                }
                else
                {
                    foreach (var Key in ModelState.Keys)
                    {
                        if (ModelState[Key].Errors.Count > 0)
                        {
                            mdata.Response = "N";
                            mdata.ErrorMessage = ModelState[Key].Errors[0].ErrorMessage;

                            //return Json(new { success = false, responseText = "registration failed, please check", JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ErrorMessage", ex.Message);
                ViewBag.ErrorMessage = ex.Message;
                mdata.Response = "N";
                mdata.ErrorMessage = "Error : " + ex.Message;
            }
            finally
            {
                dbConnection.Close();
            }
            return Json(mdata, JsonRequestBehavior.AllowGet);
        }

        public JobData IsJobExists(string bpsproductid, string servicenumber)
        {
            try
            {
                var jobdata = GetJobData().FirstOrDefault(model => model.BPSProductID == bpsproductid && model.ServiceNumber == servicenumber);
                return jobdata;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<JobData> GetJobData()
        {

            List<JobData> mdata = new List<JobData>();
            DataTable dt = new DataTable();

            cmd = new MySqlCommand("GetAllJobData", dbConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            adp = new MySqlDataAdapter(cmd);
            adp.Fill(dt);

            if (dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();

            foreach (DataRow dr in dt.Rows)
            {
                mdata.Add(new JobData
                {
                    JobID = Convert.ToInt32(dr[0]),
                    BPSProductID = Convert.ToString(dr[3]),
                    ServiceNumber = Convert.ToString(dr[4]),
                    DateCreated = Convert.ToDateTime(dr[11])
                });
            }
            dbConnection.Close();
            return mdata;
        }

        public async Task<ActionResult> GetManuscriptData(string bpsproductid, string servicenumber)
        {
            var mdata = await _manuscriptDataService.GetManuscriptDataByProductAndServiceAsync(bpsproductid, servicenumber);
            return Json(mdata, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditManuscript(string manuscriptid, string bpsproductid, string servicenumber)
        {
            ManuscriptData mdata = new ManuscriptData();
            try
            {
                DataTable dt = new DataTable();

                cmd = new MySqlCommand("GetManuscriptDataByManuscriptID", dbConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@p_ManuscriptID", manuscriptid);
                adp = new MySqlDataAdapter(cmd);
                adp.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {

                    mdata.ManuscriptID = Convert.ToInt32(dr["ManuscriptID"].ToString());
                    mdata.JobNumber = dr["JobNumber"].ToString().PadLeft(8, '0');
                    mdata.ManuscriptTier = dr["ManuscriptTier"].ToString();
                    mdata.BPSProductID = dr["BPSProductID"].ToString();
                    mdata.ServiceNumber = dr["ServiceNumber"].ToString();
                    mdata.ManuscriptLegTitle = dr["ManuscriptLegTitle"].ToString();
                    mdata.ManuscriptStatus = dr["ManuscriptStatus"].ToString();
                    mdata.TargetPressDate = dr.Field<DateTime?>("TargetPressDate");
                    mdata.ActualPressDate = dr.Field<DateTime?>("ActualPressDate");
                    mdata.LatupAttribution = dr["LatupAttribution"].ToString();
                    mdata.DateReceivedFromAuthor = dr.Field<DateTime?>("DateReceivedFromAuthor");
                    mdata.UpdateType = dr["UpdateType"].ToString();
                    mdata.JobSpecificInstruction = dr["JobSpecificInstruction"].ToString();
                    mdata.TaskType = dr["TaskType"].ToString();
                    //mdata.PEGuideCard = dr["PEGuideCard"].ToString();
                    //mdata.PECheckbox = dr["PECheckbox"].ToString();
                    //mdata.PETaskNumber = dr["PETaskNumber"].ToString();
                    mdata.RevisedOnlineDueDate = dr.Field<DateTime?>("RevisedOnlineDueDate");
                    mdata.CopyEditDueDate = Convert.ToDateTime(dr["CopyEditDueDate"].ToString());
                    mdata.CopyEditStartDate = dr.Field<DateTime?>("CopyEditStartDate");
                    mdata.CopyEditDoneDate = dr.Field<DateTime?>("CopyEditDoneDate");
                    mdata.CopyEditStatus = dr["CopyEditStatus"].ToString();
                    mdata.CodingDueDate = Convert.ToDateTime(dr["CodingDueDate"].ToString());
                    mdata.CodingDoneDate = dr.Field<DateTime?>("CodingDoneDate");
                    mdata.CodingStatus = dr["CodingStatus"].ToString();
                    mdata.OnlineDueDate = Convert.ToDateTime(dr["OnlineDueDate"].ToString());
                    mdata.OnlineDoneDate = dr.Field<DateTime?>("OnlineDoneDate");
                    mdata.OnlineStatus = dr["OnlineStatus"].ToString();
                    //mdata.STPStatus = dr["PESTPStatus"].ToString();
                    //mdata.EstimatedPages = Convert.ToInt32(dr["EstimatedPages"].ToString());
                    mdata.EstimatedPages = dr.Field<int?>("EstimatedPages");
                    //mdata.ActualTurnAroundTime = Convert.ToInt32(dr["ActualTurnAroundTime"].ToString());
                    mdata.ActualTurnAroundTime = dr.Field<int?>("ActualTurnAroundTime");
                    mdata.OnlineTimeliness = dr["OnlineTimeliness"].ToString();
                    //mdata.ReasonIfLate = dr["ReasonIfLate"].ToString();
                    //mdata.PECoversheetNumber = dr["PECoversheetNumber"].ToString();
                    mdata.DateCreated = Convert.ToDateTime(dr["DateCreated"].ToString());

                }
                return PartialView(mdata);
            }



            catch (Exception ex)
            {
                ModelState.AddModelError("ErrorMessage", ex.Message);
                mdata.ErrorMessage = ex.Message;
                return PartialView(mdata);
            }
        }

        [HttpGet]
        public ActionResult EditPEManuscript(string manuscriptid, string bpsproductid, string servicenumber)
        {
            ManuscriptData mdata = new ManuscriptData();
            try
            {
                DataTable dt = new DataTable();

                cmd = new MySqlCommand("GetManuscriptDataByManuscriptID", dbConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@p_ManuscriptID", manuscriptid);
                adp = new MySqlDataAdapter(cmd);
                adp.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {

                    mdata.ManuscriptID = Convert.ToInt32(dr["ManuscriptID"].ToString());
                    mdata.JobNumber = dr["JobNumber"].ToString().PadLeft(8, '0');
                    mdata.ManuscriptTier = dr["ManuscriptTier"].ToString();
                    mdata.BPSProductID = dr["BPSProductID"].ToString();
                    mdata.ServiceNumber = dr["ServiceNumber"].ToString();
                    mdata.ManuscriptLegTitle = dr["ManuscriptLegTitle"].ToString();
                    mdata.ManuscriptStatus = dr["ManuscriptStatus"].ToString();
                    mdata.TargetPressDate = dr.Field<DateTime?>("TargetPressDate");
                    mdata.ActualPressDate = dr.Field<DateTime?>("ActualPressDate");
                    mdata.LatupAttribution = dr["LatupAttribution"].ToString();
                    mdata.DateReceivedFromAuthor = dr.Field<DateTime?>("DateReceivedFromAuthor");
                    mdata.UpdateType = dr["UpdateType"].ToString();
                    mdata.JobSpecificInstruction = dr["JobSpecificInstruction"].ToString();
                    mdata.TaskType = dr["TaskType"].ToString();
                    mdata.PEGuideCard = dr["PEGuideCard"].ToString();
                    mdata.PECheckbox = dr["PECheckbox"].ToString();
                    mdata.PETaskNumber = dr["PETaskNumber"].ToString();
                    mdata.RevisedOnlineDueDate = dr.Field<DateTime?>("RevisedOnlineDueDate");
                    mdata.CopyEditDueDate = dr.Field<DateTime?>("CopyEditDueDate");
                    mdata.CopyEditStartDate = dr.Field<DateTime?>("CopyEditStartDate");
                    mdata.CopyEditDoneDate = dr.Field<DateTime?>("CopyEditDoneDate");
                    mdata.CopyEditStatus = dr["CopyEditStatus"].ToString();
                    mdata.CodingDueDate = dr.Field<DateTime?>("CodingDueDate");
                    mdata.CodingDoneDate = dr.Field<DateTime?>("CodingDoneDate");
                    mdata.CodingStatus = dr["CodingStatus"].ToString();
                    mdata.OnlineDueDate = dr.Field<DateTime?>("OnlineDueDate");
                    mdata.OnlineDoneDate = dr.Field<DateTime?>("OnlineDoneDate");
                    mdata.OnlineStatus = dr["OnlineStatus"].ToString();
                    mdata.STPStatus = dr["PESTPStatus"].ToString();
                    //mdata.EstimatedPages = Convert.ToInt32(dr["EstimatedPages"].ToString());
                    mdata.EstimatedPages = dr.Field<int?>("EstimatedPages");
                    //mdata.ActualTurnAroundTime = Convert.ToInt32(dr["ActualTurnAroundTime"].ToString());
                    mdata.ActualTurnAroundTime = dr.Field<int?>("ActualTurnAroundTime");
                    mdata.OnlineTimeliness = dr["OnlineTimeliness"].ToString();
                    mdata.ReasonIfLate = dr["ReasonIfLate"].ToString();
                    mdata.PECoversheetNumber = dr["PECoversheetNumber"].ToString();
                    mdata.DateCreated = Convert.ToDateTime(dr["DateCreated"].ToString());

                }
                return PartialView(mdata);
            }



            catch (Exception ex)
            {
                ModelState.AddModelError("ErrorMessage", ex.Message);
                mdata.ErrorMessage = ex.Message;
                return PartialView(mdata);
            }
        }

        [HttpPost]
        public JsonResult EditPEManuscript(ManuscriptData mdata)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(mdata.BPSProductID) && !string.IsNullOrEmpty(mdata.ServiceNumber))
                    {

                        var Username = Session["UserName"];
                        //var JobNumber = Session["JobNumber"];
                        MySqlCommand com = new MySqlCommand("UpdateManuscript", dbConnection);
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@p_Username", Username);
                        //com.Parameters.AddWithValue("@p_JobNumber", JobNumber);
                        com.Parameters.AddWithValue("@p_ManuscriptID", mdata.ManuscriptID);
                        com.Parameters.AddWithValue("@p_BPSProductID", mdata.BPSProductID);
                        com.Parameters.AddWithValue("@p_ServiceNumber", mdata.ServiceNumber);
                        com.Parameters.AddWithValue("@p_PEGuideCard", mdata.PEGuideCard);
                        com.Parameters.AddWithValue("@p_RevisedOnlineDueDate", mdata.RevisedOnlineDueDate);
                        com.Parameters.AddWithValue("@p_CopyEditDueDate", mdata.CopyEditDueDate);
                        if (mdata.CopyEditDoneDate == null)
                        {
                            mdata.CopyEditStatus = "On-Going";
                            com.Parameters.AddWithValue("@p_CopyEditStatus", mdata.CopyEditStatus);
                        }
                        if (mdata.CopyEditStartDate != null && mdata.CopyEditDoneDate != null)
                        {
                            mdata.CopyEditStatus = "Completed";
                            com.Parameters.AddWithValue("@p_CopyEditStatus", mdata.CopyEditStatus);
                        }
                        com.Parameters.AddWithValue("@p_CopyEditStartDate", mdata.CopyEditStartDate);
                        com.Parameters.AddWithValue("@p_CopyEditDoneDate", mdata.CopyEditDoneDate);
                        com.Parameters.AddWithValue("@p_EstimatedPages", mdata.EstimatedPages);
                        com.Parameters.AddWithValue("@p_ActualTurnAroundTime", mdata.ActualTurnAroundTime);
                        com.Parameters.AddWithValue("@p_ReasonIfLate", mdata.ReasonIfLate);
                        if (dbConnection.State == ConnectionState.Closed)
                            dbConnection.Open();
                        int Count = com.ExecuteNonQuery();

                        if (Count > 0)
                        {
                            mdata.Response = "Y";

                        }
                        else
                        {
                            mdata.Response = "N";
                            mdata.ErrorMessage = "Manuscript data could not be updated";
                        }

                    }


                }
                else
                {
                    foreach (var Key in ModelState.Keys)
                    {
                        if (ModelState[Key].Errors.Count > 0)
                        {
                            mdata.Response = "N";
                            mdata.ErrorMessage = ModelState[Key].Errors[0].ErrorMessage;

                            //return Json(new { success = false, responseText = "registration failed, please check", JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ErrorMessage", ex.Message);
                ViewBag.ErrorMessage = ex.Message;
                mdata.Response = "N";
                mdata.ErrorMessage = "Error : " + ex.Message;
            }
            finally
            {
                dbConnection.Close();
            }
            return Json(mdata, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateStartDate(ManuscriptData mdata)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(mdata.BPSProductID) && !string.IsNullOrEmpty(mdata.ServiceNumber))
                    {

                        var Username = Session["UserName"];
                        //var JobNumber = Session["JobNumber"];
                        MySqlCommand com = new MySqlCommand("UpdateManuscriptStartDate", dbConnection);
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@p_Username", Username);
                        com.Parameters.AddWithValue("@p_ManuscriptID", mdata.ManuscriptID);
                        com.Parameters.AddWithValue("@p_BPSProductID", mdata.BPSProductID);
                        com.Parameters.AddWithValue("@p_ServiceNumber", mdata.ServiceNumber);
                        com.Parameters.AddWithValue("@p_CopyEditStartDate", mdata.CopyEditStartDate);
                        if (mdata.CopyEditDoneDate == null)
                        {
                            mdata.CopyEditStatus = "On-Going";
                            com.Parameters.AddWithValue("@p_CopyEditStatus", mdata.CopyEditStatus);
                        }
                        if (mdata.CopyEditStartDate != null && mdata.CopyEditDoneDate != null)
                        {
                            mdata.CopyEditStatus = "Completed";
                            com.Parameters.AddWithValue("@p_CopyEditStatus", mdata.CopyEditStatus);
                        }
                        if (dbConnection.State == ConnectionState.Closed)
                            dbConnection.Open();
                        int Count = com.ExecuteNonQuery();

                        if (Count > 0)
                        {
                            mdata.Response = "Y";

                        }
                        else
                        {
                            mdata.Response = "N";
                            mdata.ErrorMessage = "Manuscript data could not be updated";
                        }

                    }


                }
                else
                {
                    foreach (var Key in ModelState.Keys)
                    {
                        if (ModelState[Key].Errors.Count > 0)
                        {
                            mdata.Response = "N";
                            mdata.ErrorMessage = ModelState[Key].Errors[0].ErrorMessage;

                            //return Json(new { success = false, responseText = "registration failed, please check", JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ErrorMessage", ex.Message);
                ViewBag.ErrorMessage = ex.Message;
                mdata.Response = "N";
                mdata.ErrorMessage = "Error : " + ex.Message;
            }
            finally
            {
                dbConnection.Close();
            }
            return Json(mdata, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateDoneDate(ManuscriptData mdata)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(mdata.BPSProductID) && !string.IsNullOrEmpty(mdata.ServiceNumber))
                    {

                        var Username = Session["UserName"];
                        //var JobNumber = Session["JobNumber"];
                        MySqlCommand com = new MySqlCommand("UpdateManuscriptDoneDate", dbConnection);
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@p_Username", Username);
                        com.Parameters.AddWithValue("@p_ManuscriptID", mdata.ManuscriptID);
                        com.Parameters.AddWithValue("@p_BPSProductID", mdata.BPSProductID);
                        com.Parameters.AddWithValue("@p_ServiceNumber", mdata.ServiceNumber);
                        com.Parameters.AddWithValue("@p_CopyEditDoneDate", mdata.CopyEditDoneDate);
                        if (mdata.CopyEditDoneDate == null)
                        {
                            mdata.CopyEditStatus = "On-Going";
                            com.Parameters.AddWithValue("@p_CopyEditStatus", mdata.CopyEditStatus);
                        }
                        if (mdata.CopyEditStartDate != null && mdata.CopyEditDoneDate != null)
                        {
                            mdata.CopyEditStatus = "Completed";
                            com.Parameters.AddWithValue("@p_CopyEditStatus", mdata.CopyEditStatus);
                        }
                        if (dbConnection.State == ConnectionState.Closed)
                            dbConnection.Open();
                        int Count = com.ExecuteNonQuery();

                        if (Count > 0)
                        {
                            mdata.Response = "Y";

                        }
                        else
                        {
                            mdata.Response = "N";
                            mdata.ErrorMessage = "Manuscript data could not be updated";
                        }

                    }


                }
                else
                {
                    foreach (var Key in ModelState.Keys)
                    {
                        if (ModelState[Key].Errors.Count > 0)
                        {
                            mdata.Response = "N";
                            mdata.ErrorMessage = ModelState[Key].Errors[0].ErrorMessage;

                            //return Json(new { success = false, responseText = "registration failed, please check", JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ErrorMessage", ex.Message);
                ViewBag.ErrorMessage = ex.Message;
                mdata.Response = "N";
                mdata.ErrorMessage = "Error : " + ex.Message;
            }
            finally
            {
                dbConnection.Close();
            }
            return Json(mdata, JsonRequestBehavior.AllowGet);
        }
    }
}