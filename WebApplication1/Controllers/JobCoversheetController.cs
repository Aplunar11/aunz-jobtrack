using System;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using System.Collections.Generic;
using JobTrack.Models.Coversheet;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using System.Net.Mail;
using JobTrack.Models.JobCoversheet;
using JobTrack.Services.Interfaces;
using System.Threading.Tasks;
using JobTrack.Models;

namespace JobTrack.Controllers
{
    public class JobCoversheetController : Controller
    {
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        private readonly IPubSchedService _pubSchedService;
        private readonly IManuscriptDataService _manuscriptDataService;
        private readonly IJobCoversheetService _jobCoversheetService;
        private readonly ICoversheetService _coversheetService;

        public JobCoversheetController(IPubSchedService pubSchedService
            , IManuscriptDataService manuscriptDataService
            , IJobCoversheetService jobCoversheetService
            , ICoversheetService coversheetService)
        {
            _pubSchedService = pubSchedService;
            _manuscriptDataService = manuscriptDataService;
            _jobCoversheetService = jobCoversheetService;
            _coversheetService = coversheetService;
        }

        public async Task<ActionResult> _AddNewJobCoversheetView(string manuscriptids, string bpsproductid, string serviceno)
        {
            var viewModel = new JobCoversheetData();

            if ((manuscriptids ?? bpsproductid ?? serviceno) == null)
            {
                //no record
                viewModel.DateCreated = DateTime.Now;
                viewModel.XMLEditing = true;
                viewModel.OnlineStatus = true ? "New" : null;

                TempData["UpdateTypes"] = new SelectList(GetAllTurnAroundTime(), "UpdateType", "UpdateType");
                return PartialView(viewModel);
            }

            //with record
            viewModel.BPSProductID = bpsproductid;
            viewModel.ServiceNumber = serviceno;
            viewModel.XMLEditing = true;
            viewModel.OnlineStatus = true ? "New" : null;

            Session["ManuscriptIDs"] = manuscriptids.Trim('[', ']');

            try
            {
                //var resultpubsched = PubSchedData(mdata.BPSProductID, mdata.ServiceNumber);
                var pubschedData = await _pubSchedService.GetPubSchedDataByProductAndServiceAsync(viewModel);

                //var resultmanuscript = ManuscriptData(mdata.BPSProductID, mdata.ServiceNumber);
                var manuscriptData = await _manuscriptDataService.GetManuscriptDataMaxTurnAroundTimeAsync(viewModel, manuscriptids);

                //var resultcover = JobCoversheetData(mdata.BPSProductID, mdata.ServiceNumber);
                var jobCoversheet = await _jobCoversheetService.GetJobCoversheetDataByProductAndServiceAsync(viewModel);

                TempData["UpdateTypes"] = new SelectList(GetAllTurnAroundTime(), "UpdateType", "UpdateType", manuscriptData.UpdateType);
                TempData["OnlineStatuses"] = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Text = "New", Value = "New" },
                    new SelectListItem { Text = "Completed", Value = "Completed" },
                    new SelectListItem { Text = "Cancelled", Value = "Cancelled" }
                }, "Text", "Value");

                var userName = (string)Session["UserName"];
                viewModel.Editor = userName;
                viewModel.ChargeCode = pubschedData.ChargeCode;
                viewModel.CoversheetTier = manuscriptData.ManuscriptTier;
                viewModel.TargetPressDate = manuscriptData.TargetPressDate;
                viewModel.TaskType = manuscriptData.TaskType;
                viewModel.CodingDueDate = manuscriptData.CodingDueDate;
                viewModel.OnlineDueDate = manuscriptData.OnlineDueDate;
                viewModel.DateCreated = manuscriptData.DateCreated;                
                viewModel.GuideCard = manuscriptData.PEGuideCard;                
                viewModel.TaskNumber = jobCoversheet == null ? "Task1" : $"Task{jobCoversheet.LatestTaskNumber + 1}";
                viewModel.CoversheetNumber = bpsproductid + '_' + serviceno + '_' + viewModel.TaskNumber;
                viewModel.IsOnline = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return PartialView(viewModel);
        }

        public async Task<ActionResult> AddNewJobCoversheet(CoversheetData model)
        {
            var result = new JsonResultModel();
            var username = (string)Session["UserName"];
            var manuscriptIds = (string)Session["ManuscriptIDs"];

            try
            {
                if (!string.IsNullOrEmpty(model.BPSProductID) && !string.IsNullOrEmpty(model.ServiceNumber))
                {
                    var data = await _jobCoversheetService.IsJobExists(model.BPSProductID, model.ServiceNumber);

                    model.ManuscriptID = manuscriptIds;
                    model.CoversheetNumber = model.CoversheetNumber + "_" + model.GuideCard;
                    model.GuideCard = model.GuideCard.Replace("\"", "");

                    if (data != null)
                    {
                        if (data.JobCoversheetID == 0 || data.JobCoversheetID < 0)
                        {
                            result.ErrorMessage = "Entered invalid Product or Service Number";
                            return Json(result);
                        }
                        
                        result = await _coversheetService.InsertCoversheetDataAsync(model, username);
                        return Json(result);
                    }

                    result = await _jobCoversheetService.InsertJobCoversheetAsync(model, username);
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }

            return Json(result);
        }

        public JobCoversheetData PubSchedData(string prodid, string serno)
        {
            try
            {
                var pubsched = GetPubSchedData().FirstOrDefault(model => model.BPSProductID == prodid && model.ServiceNumber == serno);
                return pubsched;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public JobCoversheetData ManuscriptData(string prodid, string serno)
        {
            //try
            //{
            //    //string mainUpdateType = "";
            //    JobCoversheetData temp = new JobCoversheetData();
            //    //var manuscript = GetManuscriptData().FirstOrDefault(model => model.ManuscriptID == manuids);
            //    if (manuids.Contains(','))
            //    {
            //        List<UpdateTypeData> updateTypes = GetUpdateType();
            //        //var manuscripts = GetManuscriptData().Where(model => model.ManuscriptID.Replace(" ", "").Split(',').Any(id => manuids.Contains(id)));
            //        var manuscripts = GetManuscriptData().Where(model => model.ManuscriptID.Replace(" ", "").Split(',').Any(id => manuids != null));

            //        List<string> results = manuscripts.Select(x => x.UpdateType).ToList();
            //        int updateTypeID = 0;
            //        foreach (string e in results)
            //        {
            //            UpdateTypeData t = updateTypes.Find(x => x.UpdateType == e);
            //            if (updateTypeID < t.UpdateTypeID)
            //            {
            //                updateTypeID = t.UpdateTypeID;
            //            }
            //        }
            //        temp.UpdateType = updateTypes.Find(x => x.UpdateTypeID == updateTypeID).UpdateType;
            //    }
            //    else
            //    {
            //        var manuscript = GetManuscriptData().FirstOrDefault(model => model.ManuscriptID == manuids);
            //        temp.UpdateType = manuscript.UpdateType;
            //    }
            var manuscript = GetManuscriptData().FirstOrDefault(model => model.BPSProductID == prodid && model.ServiceNumber == serno);

            return manuscript;
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
        }

        public JobCoversheetData JobCoversheetData(string prodid, string serno)
        {
            try
            {
                var cover = GetJobCoversheetDetails().OrderByDescending(model => model.JobCoversheetID).FirstOrDefault(model => model.BPSProductID == prodid && model.ServiceNumber == serno);
                return cover;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<JobCoversheetData> GetPubSchedData()
        {
            List<JobCoversheetData> mdata = new List<JobCoversheetData>();
            DataTable dt = new DataTable();

            cmd = new MySqlCommand("GetAllPubSched_MT", dbConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            adp = new MySqlDataAdapter(cmd);
            adp.Fill(dt);

            if (dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();

            foreach (DataRow dr in dt.Rows)
            {
                mdata.Add(new JobCoversheetData
                {
                    BPSProductID = Convert.ToString(dr[6]),
                    Editor = Convert.ToString(dr[7]),
                    ChargeCode = Convert.ToString(dr[9]),
                    ServiceNumber = Convert.ToString(dr[17])
                });
            }

            return mdata;
        }

        public List<JobCoversheetData> GetManuscriptData()
        {

            List<JobCoversheetData> mdata = new List<JobCoversheetData>();
            DataTable dt = new DataTable();

            cmd = new MySqlCommand("GetAllManuscriptData", dbConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            adp = new MySqlDataAdapter(cmd);
            adp.Fill(dt);

            if (dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();

            foreach (DataRow dr in dt.Rows)
            {
                mdata.Add(new JobCoversheetData
                {
                    ManuscriptID = Convert.ToString(dr[0]),
                    CoversheetTier = Convert.ToString(dr[2]),
                    BPSProductID = Convert.ToString(dr[3]),
                    ServiceNumber = Convert.ToString(dr[4]),
                    TargetPressDate = Convert.ToDateTime(dr[7]),
                    TaskType = Convert.ToString(dr[13]),
                    UpdateType = Convert.ToString(dr[11]),
                    CodingDueDate = Convert.ToDateTime(dr[22]),
                    OnlineDueDate = Convert.ToDateTime(dr[25]),
                    //6-15
                    DateCreated = Convert.ToDateTime(dr[34])
                });
            }
            return mdata;
        }

        public List<JobCoversheetData> GetJobCoversheetDetails()
        {

            List<JobCoversheetData> mdata = new List<JobCoversheetData>();
            DataTable dt = new DataTable();

            cmd = new MySqlCommand("GetAllJobCoversheetData", dbConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            adp = new MySqlDataAdapter(cmd);
            adp.Fill(dt);

            if (dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();

            foreach (DataRow dr in dt.Rows)
            {
                mdata.Add(new JobCoversheetData
                {
                    JobCoversheetID = Convert.ToInt32(dr[0]),
                    BPSProductID = Convert.ToString(dr[3]),
                    ServiceNumber = Convert.ToString(dr[4]),
                    TaskNumber = Convert.ToString(dr[5])
                });
            }
            return mdata;
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

        public ActionResult GetTaskType(string selectedItem)
        {
            var data = GetAllTurnAroundTime().Where(model => model.UpdateType == selectedItem).FirstOrDefault();

            return Json(data.TaskType, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTATCoding(string selectedItem, DateTime datecreated)
        {
            var data = GetAllTurnAroundTime().Where(model => model.UpdateType == selectedItem).FirstOrDefault();
            //DateTime d = DateTime.Now.AddDays(data.TATCoding);
            //DateTime d = AddBusinessDays(DateTime.Now, data.TATCoding);
            DateTime d = AddBusinessDays(datecreated, data.TATCoding);
            return Json(d.ToString("yyyy-MM-dd"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTATOnline(string selectedItem, DateTime datecreated)
        {
            var data = GetAllTurnAroundTime().Where(model => model.UpdateType == selectedItem).FirstOrDefault();
            //DateTime d = DateTime.Now.AddDays(data.TATOnline);
            //DateTime d = AddBusinessDays(DateTime.Now, data.TATOnline);
            DateTime d = AddBusinessDays(datecreated, data.TATOnline);
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

        public async Task<ActionResult> GetJobCoversheetData()
        {
            var result = await _jobCoversheetService.GetAllJobCoversheetDataAsync();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region [Old Code] Get all job coversheet
        /* public ActionResult GetJobCoversheetData()
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

            List<JobCoversheetData> mdata = new List<JobCoversheetData>();
            DataTable dt = new DataTable();

            cmd = new MySqlCommand("GetAllJobCoversheetData", dbConnection);
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
                mdata.Add(new JobCoversheetData
                {

                    JobCoversheetID = Convert.ToInt32(dr["CoversheetID"].ToString()),
                    CoversheetTier = dr["CoversheetTier"].ToString(),
                    BPSProductID = dr["BPSProductID"].ToString(),
                    ServiceNumber = dr["ServiceNumber"].ToString(),
                    //LatestTaskNumber = dr["LatestTaskNumber"].ToString(),
                    LatestTaskNumber = "TBD",
                    CodingStatus = dr["CodingStatus"].ToString(),
                    //PDFQAStatus = dr["PDFQAStatus"].ToString(),
                    PDFQAStatus = "TBD",
                    OnlineStatus1 = dr["OnlineStatus"].ToString(),
                    DateCreated = Convert.ToDateTime(dr["DateCreated"].ToString())

                });
            }
            dbConnection.Close();
            return Json(mdata, JsonRequestBehavior.AllowGet);
        } */
        #endregion

        public async Task<ActionResult> GetAllJobCoversheetDataByUserNameLEorPE()
        {
            var username = (string)Session["Username"];
            var result = await _jobCoversheetService.GetAllJobCoversheetDataByUserNameLEorPEAsync(username);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JobCoversheetData IsJobExists(string bpsproductid, string servicenumber)
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

        public List<JobCoversheetData> GetJobData()
        {

            List<JobCoversheetData> mdata = new List<JobCoversheetData>();
            DataTable dt = new DataTable();

            cmd = new MySqlCommand("GetAllJobCoversheetData", dbConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            adp = new MySqlDataAdapter(cmd);
            adp.Fill(dt);

            if (dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();

            foreach (DataRow dr in dt.Rows)
            {
                mdata.Add(new JobCoversheetData
                {
                    JobCoversheetID = Convert.ToInt32(dr[0]),
                    //JobNumber = Convert.ToInt32(dr[1]),
                    BPSProductID = Convert.ToString(dr[2]),
                    ServiceNumber = Convert.ToString(dr[3]),
                    DateCreated = Convert.ToDateTime(dr[10])
                });
            }
            dbConnection.Close();
            return mdata;
        }

        public void SendEmail(JobCoversheetData mdata)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var mail = new MailMessage())
                    {
                        //const string password = "2544Joey9067!";


                        mail.From = new MailAddress("JobTrack_AUNZ-NoReply@spi-global.com", "JobTrack_AUNZ-NoReply@spi-global.com");
                        mail.To.Add(new MailAddress("jeffrey.danque@spi-global.com"));
                        mail.To.Add(new MailAddress("recto.aseron@spi-global.com"));
                        mail.To.Add(new MailAddress("Katherine.Masangkay@straive.com"));
                        mail.Subject = "[JobTrack AUNZ] New Coversheet data " + mdata.BPSProductID + "_" + mdata.ServiceNumber + "_" + mdata.CoversheetNumber;

                        string body = "<div>" +
                        "<table border=0 cellspacing=1 cellpadding=0 width='100%'" +
                        "style='width:100.0%;mso-cellspacing:.7pt;mso-padding-alt:4.5pt 4.5pt 4.5pt 4.5pt'>" +
                        "<tr>" +
                        "<td valign=top style='background:whitesmoke;padding:4.5pt 4.5pt 4.5pt 4.5pt'>" +
                        "<p>" +
                        "<b>" +
                        "<span style='font-size:12.0pt;font-family:Verdana'>JobTrack AUNZ Manuscript</span>" +
                        "</b>" +
                        "<span style='font-size:12.0pt;font-family:Verdana'></span>" +
                        "</p>" +
                        "</td>" +
                        "</tr>" +
                        "<tr>" +
                        "<td style='padding:4.5pt 4.5pt 4.5pt 4.5pt'>" +
                        "<p>" +
                        "<b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> Date Created: </span>" +
                        "</b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> " + DateTime.Now.ToString("yyyy-MM-dd") + " </span>" +
                        "</p>" +
                        "</td>" +
                        "</tr>" +
                        "<tr style='height:12.25pt'>" +
                        "<td style='padding:4.5pt 4.5pt 4.5pt 4.5pt;height:12.25pt'>" +
                        "<p>" +
                        "<b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> Coversheet Number : </span>" +
                        "</b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> " + mdata.CoversheetNumber + " </span>" +
                        "</p>" +
                        "</td>" +
                        "</tr>" +
                        "<tr style='height:12.25pt'>" +
                        "<td style='padding:4.5pt 4.5pt 4.5pt 4.5pt;height:12.25pt'>" +
                        "<p>" +
                        "<b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> Service Number : </span>" +
                        "</b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> " + mdata.ServiceNumber + " </span>" +
                        "</p>" +
                        "</td>" +
                        "</tr>" +
                        "<tr style='height:7.75pt'>" +
                        "<td style='padding:4.5pt 4.5pt 4.5pt 4.5pt;height:7.75pt'>" +
                        "<p>" +
                        "<b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> Task Number : </span>" +
                        "</b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> " + mdata.TaskNumber + " </span>" +
                        "</p>" +
                        "</td>" +
                        "</tr>" +
                        "<tr style='height:7.75pt'>" +
                        "<td style='padding:4.5pt 4.5pt 4.5pt 4.5pt;height:7.75pt'>" +
                        "<p>" +
                        "<b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> Product : </span>" +
                        "</b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> " + mdata.BPSProductID + " </span>" +
                        "</p>" +
                        "</td>" +
                        "</tr>" +
                        "<tr>" +
                        "<td style='padding:4.5pt 4.5pt 4.5pt 4.5pt'>" +
                        "<p>" +
                        "<b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> Editor :</span>" +
                        "</b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> " + mdata.Editor + " </span>" +
                        "</p>" +
                        "</td>" +
                        "</tr>" +
                        "<tr>" +
                        "<td style='padding:4.5pt 4.5pt 4.5pt 4.5pt'>" +
                        "<p>" +
                        "<b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> Charge Code : </span>" +
                        "</b>" +
                        "<span>" +
                        "<span style='font-size:8.0pt;font-family: Verdana'> " + mdata.ChargeCode + " </span>" +
                        "</span>" +
                        "</td>" +
                        "</tr>" +
                        "<tr>" +
                        "<td style='padding:4.5pt 4.5pt 4.5pt 4.5pt'>" +
                        "<p>" +
                        "<b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> Target Press Date: </span>" +
                        "</b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> " + mdata.TargetPressDate + " </span>" +
                        "</p>" +
                        "</td>" +
                        "</tr>" +
                        "<tr>" +
                        "<td style='padding:4.5pt 4.5pt 4.5pt 4.5pt'>" +
                        "<p>" +
                        "<b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> Task Type: </span>" +
                        "</b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> " + mdata.TaskType + " </span>" +
                        "</p>" +
                        "</td>" +
                        "</tr>" +
                        "<tr>" +
                        "<td style='padding:4.5pt 4.5pt 4.5pt 4.5pt'>" +
                        "<p>" +
                        "<b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> Guide Cards: </span>" +
                        "</b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> " + mdata.GuideCard + " </span>" +
                        "</p>" +
                        "</td>" +
                        "</tr>" +
                        "<tr>" +
                        "<td style='padding:4.5pt 4.5pt 4.5pt 4.5pt'>" +
                        "<p>" +
                        "<b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> Further Instructions: </span>" +
                        "</b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> " + mdata.FurtherInstructions + " </span>" +
                        "</p>" +
                        "</td>" +
                        "</tr>" +
                        "<tr>" +
                        "<td style='padding:4.5pt 4.5pt 4.5pt 4.5pt'>" +
                        "<p>" +
                        "<b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> Update Type: </span>" +
                        "</b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> " + mdata.UpdateType + " </span>" +
                        "</p>" +
                        "</td>" +
                        "</tr>" +
                        "<tr>" +
                        "<td style='padding:4.5pt 4.5pt 4.5pt 4.5pt'>" +
                        "<p>" +
                        "<b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> General: </span>" +
                        "</b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> " + mdata.GeneralData + " </span>" +
                        "</p>" +
                        "</td>" +
                        "</tr>" +
                        "<tr>" +
                        "<td style='padding:4.5pt 4.5pt 4.5pt 4.5pt'>" +
                        "<p>" +
                        "<b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> Special Instruction: </span>" +
                        "</b>" +
                        "<span style='font-size:8.0pt;font-family:Verdana'> " + mdata.SpecialInstruction + " </span>" +
                        "</p>" +
                        "</td>" +
                        "</tr>" +
                            "<tr>" +
                            "<td width=581 style='width:435.8pt;padding:4.5pt 4.5pt 4.5pt 4.5pt'>" +
                            "<p>" +
                            "<span style='font-size:7.0pt;font-family:Verdana'> This is an auto-generated e-mail. No need to reply to this e-mail. </span>" +
                            "</p>" +
                            "</td>" +
                            "</tr>" +
                            "<tr>" +
                            "<td width=581 style='width:435.8pt;padding:4.5pt 4.5pt 4.5pt 4.5pt'>" +
                            "<p>" +
                            "<span style='font-size:7.0pt;font-family:Verdana'> The content of this e-mail message may be privileged and confidential." +
                            "Therefore, if this message has been received in error, please delete it without reading it." +
                            "Your receipt of this message is not intended to waive any applicable privilege." +
                            "Please do not disseminate this message without the permission of the author </span>" +
                            "</p>" +
                            "</td>" +
                            "</tr>" +
                            "</table>" +
                            "</div>";

                        mail.Body = body;
                        mail.IsBodyHtml = true;

                        try
                        {
                            //comment for local
                            //using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                            //{
                            //    smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                            //    smtpClient.EnableSsl = true;
                            //    smtpClient.UseDefaultCredentials = false;
                            //    smtpClient.Credentials = new NetworkCredential("jeffrey.b.danque@gmail.com", "nvylrkfuoiwxnfqy");


                            //    smtpClient.Send(mail);
                            //}
                            //comment for live
                            SmtpClient objSmtp = new SmtpClient(ConfigurationManager.AppSettings["smtp_server"].ToString());

                            mail.DeliveryNotificationOptions =
                               DeliveryNotificationOptions.OnSuccess |
                               DeliveryNotificationOptions.OnFailure |
                               DeliveryNotificationOptions.Delay;

                            //SmtpClient objSmtp = new SmtpClient("MySMPTHost");
                            //objSmtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                            objSmtp.Timeout = 30000;

                            objSmtp.Send(mail);
                        }

                        finally
                        {
                            //dispose the client
                            mail.Dispose();
                        }

                    }
                }
                catch (SmtpFailedRecipientsException ex)
                {
                    foreach (SmtpFailedRecipientException t in ex.InnerExceptions)
                    {
                        var status = t.StatusCode;
                        if (status == SmtpStatusCode.MailboxBusy ||
                            status == SmtpStatusCode.MailboxUnavailable)
                        {
                            Response.Write("Delivery failed - retrying in 5 seconds.");
                            System.Threading.Thread.Sleep(5000);
                            //resend
                            //smtpClient.Send(message);
                        }
                        else
                        {
                            //Response.Write("Failed to deliver message to {0}",
                            //                  t.FailedRecipient);
                        }
                    }
                }
                catch (SmtpException Se)
                {
                    // handle exception here
                    Response.Write(Se.ToString());
                }

                catch (Exception ex)
                {
                    Response.Write(ex.ToString());
                }
            }
        }
    }
}