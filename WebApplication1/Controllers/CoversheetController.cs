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
using System.Threading.Tasks;
using JobTrack.Services.Interfaces;
using JobTrack.Models.JobCoversheet;
using JobTrack.Models.Enums;

namespace JobTrack.Controllers
{
    public class CoversheetController : Controller
    {
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        private readonly ICoversheetService _coversheetService;
        private readonly IParameterService _parameterService;
        private readonly IEmployeeService _employeeService;

        public CoversheetController(ICoversheetService coversheetService
            , IParameterService parameterService
            , IEmployeeService employeeService)
        {
            _coversheetService = coversheetService;
            _parameterService = parameterService;
            _employeeService = employeeService;
        }

        public async Task<ActionResult> _EditCoversheetView(int id)
        {
            var viewModel = await _coversheetService.GetCoversheetDataByIdAsync(id);
            var updateTypes = await _parameterService.GetAllTurnAroundTimeAsync();
            var owners = await _employeeService.GetAllEmployeeByAccess(UserAccessEnum.Coding);

            TempData["UpdateTypes"] = new SelectList(updateTypes
                , "UpdateType"
                , "UpdateType"
                , updateTypes.FirstOrDefault(x => x.UpdateType == viewModel.UpdateType).UpdateType);

            TempData["JobOwners"] = new SelectList(owners
                , "ID"
                , "UserName"
                , viewModel.JobOwnerID.HasValue ? viewModel.JobOwnerID.Value : 0);

            return PartialView(viewModel);
        }

        public async Task<ActionResult> _EditCoversheetCodingTLView(int id)
        {
            var viewModel = await _coversheetService.GetCoversheetDataByIdAsync(id);
            var updateTypes = await _parameterService.GetAllTurnAroundTimeAsync();
            var owners = await _employeeService.GetAllEmployeeByAccess(UserAccessEnum.Coding);

            TempData["UpdateTypes"] = new SelectList(updateTypes
                , "UpdateType"
                , "UpdateType"
                , updateTypes.FirstOrDefault(x => x.UpdateType == viewModel.UpdateType).UpdateType);

            TempData["JobOwners"] = new SelectList(owners
                , "ID"
                , "UserName"
                , viewModel.JobOwnerID.HasValue ? viewModel.JobOwnerID.Value : 0);

            return PartialView(viewModel);
        }

        public async Task<ActionResult> _EditCoversheetCodingView(int id)
        {
            var viewModel = await _coversheetService.GetCoversheetDataByIdAsync(id);
            var updateTypes = await _parameterService.GetAllTurnAroundTimeAsync();
            var owners = await _employeeService.GetAllEmployeeByAccess(UserAccessEnum.Coding);

            TempData["UpdateTypes"] = new SelectList(updateTypes
                , "UpdateType"
                , "UpdateType"
                , updateTypes.FirstOrDefault(x => x.UpdateType == viewModel.UpdateType).UpdateType);

            TempData["JobOwners"] = new SelectList(owners
                , "ID"
                , "UserName"
                , viewModel.JobOwnerID.HasValue ? viewModel.JobOwnerID.Value : 0);

            return PartialView(viewModel);
        }

        public async Task<ActionResult> EditCoversheetData(CoversheetData model, UserAccessEnum userAccess)
        {
            var userName = (string)Session["UserName"];
            var isSuccess = false;

            try
            {
                switch (userAccess)
                {
                    case UserAccessEnum.Straive_PE:
                        await _coversheetService.UpdateCoversheetByPE(model, userName, (int)userAccess);
                        break;

                    case UserAccessEnum.Coding_TL:
                        await _coversheetService.UpdateCoversheetByCodingTL(model);
                        break;

                    case UserAccessEnum.Coding:
                        await _coversheetService.UpdateCoversheetByCoding(model);
                        break;
                }

                isSuccess = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return Json(new { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddNewCoversheet(string manuscriptids, string bpsproductid, string serviceno)
        {
            //no record
            if ((manuscriptids ?? bpsproductid ?? serviceno) == null)
            {
                CoversheetData mdata = new CoversheetData
                {
                    DateCreated = DateTime.Now,
                    IsXMLEditing = true,
                    //OnlineStatus = true   TODO: uncomment
                };
                try
                {
                    TempData["UpdateTypes"] = new SelectList(GetAllTurnAroundTime(), "UpdateType", "UpdateType");

                    return PartialView(mdata);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ErrorMessage", ex.Message);
                    mdata.ErrorMessage = ex.Message;
                    return PartialView(mdata);
                }
            }
            //with record
            else
            {
                CoversheetData mdata = new CoversheetData
                {
                    //DateCreated = DateTime.Now,
                    //ManuscriptID = manuscriptids.Trim('[', ']'),

                    BPSProductID = bpsproductid,
                    ServiceNumber = serviceno,
                    IsXMLEditing = true,
                    //OnlineStatus = true   TODO: uncomment
                };
                try
                {
                    Session["ManuscriptIDs"] = manuscriptids.Trim('[', ']');

                    var resultpubsched = PubSchedData(mdata.BPSProductID, mdata.ServiceNumber);
                    var resultmanuscript = ManuscriptData(mdata.BPSProductID, mdata.ServiceNumber);
                    TempData["UpdateTypes"] = new SelectList(GetAllTurnAroundTime(), "UpdateType", "UpdateType", resultmanuscript.UpdateType);
                    var resultcover = CoversheetData(mdata.BPSProductID, mdata.ServiceNumber);

                    mdata.Editor = resultpubsched.Editor;
                    mdata.ChargeCode = resultpubsched.ChargeCode;
                    mdata.CoversheetTier = resultmanuscript.CoversheetTier;
                    mdata.TargetPressDate = resultmanuscript.TargetPressDate;
                    mdata.TaskType = resultmanuscript.TaskType;
                    mdata.CodingDueDate = resultmanuscript.CodingDueDate;
                    mdata.OnlineDueDate = resultmanuscript.OnlineDueDate;
                    mdata.DateCreated = resultmanuscript.DateCreated;

                    //tasknumber counter
                    int val = 0;
                    if
                    (resultcover == null)
                    {
                        mdata.TaskNumber = "1";
                    }
                    else
                    {
                        mdata.TaskNumber = resultcover.TaskNumber;
                        val = Convert.ToInt32(mdata.TaskNumber);
                        val++;
                        mdata.TaskNumber = Convert.ToString(val);
                    }


                    mdata.CoversheetNumber = bpsproductid + '_' + serviceno + '_' + mdata.TaskNumber;
                    //TempData["ManuscriptTier"] = new SelectList(GetAllPubschedTier(), "PubSchedTier", "PubSchedTier");
                    //TempData["BPSProductIDJobs"] = new SelectList(GetAllBPSProductIDJobs(), "BPSProductIDJobs", "BPSProductIDJobs");
                    //TempData["ServiceNumberJobs"] = new SelectList(GetAllServiceNumberJobs(), "ServiceNumberJobs", "ServiceNumberJobs");
                    //TempData["BPSProductID"] = new SelectList(GetAllPubschedBPSProductID(), "PubschedBPSProductID", "PubschedBPSProductID");

                    return PartialView(mdata);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ErrorMessage", ex.Message);
                    mdata.ErrorMessage = ex.Message;
                    return PartialView(mdata);
                }
            }
        }

        public CoversheetData PubSchedData(string prodid, string serno)
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

        public CoversheetData ManuscriptData(string prodid, string serno)
        {
            //try
            //{
            //    //string mainUpdateType = "";
            //    CoversheetData temp = new CoversheetData();
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

        public CoversheetData CoversheetData(string prodid, string serno)
        {
            try
            {
                var cover = GetCoversheetDetails().OrderByDescending(model => model.CoversheetID).FirstOrDefault(model => model.BPSProductID == prodid && model.ServiceNumber == serno);
                return cover;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<CoversheetData> GetPubSchedData()
        {

            List<CoversheetData> mdata = new List<CoversheetData>();
            DataTable dt = new DataTable();

            cmd = new MySqlCommand("GetAllPubSched_MT", dbConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            adp = new MySqlDataAdapter(cmd);
            adp.Fill(dt);

            if (dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();

            foreach (DataRow dr in dt.Rows)
            {
                mdata.Add(new CoversheetData
                {
                    BPSProductID = Convert.ToString(dr[6]),
                    Editor = Convert.ToString(dr[7]),
                    ChargeCode = Convert.ToString(dr[9]),
                    ServiceNumber = Convert.ToString(dr[17])
                });
            }
            return mdata;
        }

        public List<CoversheetData> GetManuscriptData()
        {

            List<CoversheetData> mdata = new List<CoversheetData>();
            DataTable dt = new DataTable();

            cmd = new MySqlCommand("GetAllManuscriptData", dbConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            adp = new MySqlDataAdapter(cmd);
            adp.Fill(dt);

            if (dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();

            foreach (DataRow dr in dt.Rows)
            {
                mdata.Add(new CoversheetData
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

        public List<CoversheetData> GetCoversheetDetails()
        {

            List<CoversheetData> mdata = new List<CoversheetData>();
            DataTable dt = new DataTable();

            cmd = new MySqlCommand("GetAllCoversheetData", dbConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            adp = new MySqlDataAdapter(cmd);
            adp.Fill(dt);

            if (dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();

            foreach (DataRow dr in dt.Rows)
            {
                mdata.Add(new CoversheetData
                {
                    CoversheetID = Convert.ToInt32(dr[0]),
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

        [HttpPost]
        public ActionResult GetTaskType(string selectedItem)
        {
            var data = GetAllTurnAroundTime().Where(model => model.UpdateType == selectedItem).FirstOrDefault();
            return Json(data.TaskType, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetTATCoding(string selectedItem, DateTime datecreated)
        {
            var data = GetAllTurnAroundTime().Where(model => model.UpdateType == selectedItem).FirstOrDefault();
            DateTime date = AddBusinessDays(datecreated, data.TATCoding);
            return Json(date.ToString("yyyy-MM-dd"), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetTATOnline(string selectedItem, DateTime datecreated)
        {
            var data = GetAllTurnAroundTime().Where(model => model.UpdateType == selectedItem).FirstOrDefault();
            DateTime date = AddBusinessDays(datecreated, data.TATOnline);
            return Json(date.ToString("yyyy-MM-dd"), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetEmployeeEmail(int selectedItem)
        {
            var owners = _employeeService.GetAllEmployeeByAccess(UserAccessEnum.Coding).Result;
            var selected = owners.FirstOrDefault(x => x.ID == selectedItem);
            return Json(selected.EmailAddress, JsonRequestBehavior.AllowGet);
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

        #region [Old Code] AddNewCoversheet
        /* [HttpPost]
        public JsonResult AddNewCoversheet(CoversheetData mdata)
        {

            if (ModelState.IsValid)
            {

                try
                {
                    //string Manus = Session["ManuscriptIDs"].ToString();
                    var Username = Session["UserName"];
                    MySqlCommand com = new MySqlCommand("InsertCoversheet", dbConnection);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@p_Username", Username);
                    //com.Parameters.AddWithValue("@p_ManuscriptID", Manus);
                    com.Parameters.AddWithValue("@p_CoversheetNumber", mdata.CoversheetNumber);
                    com.Parameters.AddWithValue("@p_BPSProductID", mdata.BPSProductID);
                    com.Parameters.AddWithValue("@p_ServiceNumber", mdata.ServiceNumber);
                    com.Parameters.AddWithValue("@p_TaskNumber", mdata.TaskNumber);
                    com.Parameters.AddWithValue("@p_CoversheetTier", mdata.CoversheetTier);
                    com.Parameters.AddWithValue("@p_Editor", mdata.Editor);
                    com.Parameters.AddWithValue("@p_ChargeCode", mdata.ChargeCode);
                    com.Parameters.AddWithValue("@p_TargetPressDate", mdata.TargetPressDate);
                    com.Parameters.AddWithValue("@p_TaskType", mdata.TaskType);
                    com.Parameters.AddWithValue("@p_GuideCard", mdata.GuideCard);
                    com.Parameters.AddWithValue("@p_FurtherInstructions", mdata.FurtherInstructions);
                    com.Parameters.AddWithValue("@p_UpdateType", mdata.UpdateType);
                    com.Parameters.AddWithValue("@p_GeneralData", mdata.GeneralData);
                    com.Parameters.AddWithValue("@p_SpecialInstruction", mdata.SpecialInstruction);
                    com.Parameters.AddWithValue("@p_CodingDueDate", mdata.CodingDueDate);
                    com.Parameters.AddWithValue("@p_XMLEditing", mdata.XMLEditing);
                    com.Parameters.AddWithValue("@p_OnlineDueDate", mdata.OnlineDueDate);
                    if (mdata.OnlineDueDate >= DateTime.Now)
                    {
                        mdata.OnlineTimeliness = "On Time";
                        com.Parameters.AddWithValue("@p_OnlineTimeliness", mdata.OnlineTimeliness);
                    }
                    else
                    {
                        mdata.OnlineTimeliness = "Late";
                        com.Parameters.AddWithValue("@p_OnlineTimeliness", mdata.OnlineTimeliness);
                    }
                    com.Parameters.AddWithValue("@p_OnlineStatus", mdata.OnlineStatus);
                    com.Parameters.AddWithValue("@p_CorrectionDueDate", mdata.CorrectionDueDate);
                    com.Parameters.AddWithValue("@p_CorrectionData", mdata.CorrectionData);
                    if (dbConnection.State == ConnectionState.Closed)
                        dbConnection.Open();
                    int Counto = com.ExecuteNonQuery();

                    if (Counto > 0)
                    {
                        mdata.Response = "Y";
                        //SendEmail(mdata);
                    }
                    else
                    {
                        mdata.Response = "N";
                        mdata.ErrorMessage = "Coversheet data could not be added";
                    }
                    //string[] array;
                    //if (Manus.Contains(","))
                    //{
                    //    int Count = 0;
                    //    array = Manus.Split(',');
                    //    for (int i = 0; i < array.Length; i++)
                    //    {
                    //        MySqlCommand com = new MySqlCommand("InsertCoversheet", dbConnection);
                    //        com.CommandType = CommandType.StoredProcedure;
                    //        com.Parameters.AddWithValue("@p_Username", Username);
                    //        com.Parameters.AddWithValue("@p_ManuscriptID", Convert.ToInt32(array[i]));
                    //        com.Parameters.AddWithValue("@p_CoversheetNumber", mdata.CoversheetNumber);
                    //        com.Parameters.AddWithValue("@p_BPSProductID", mdata.BPSProductID);
                    //        com.Parameters.AddWithValue("@p_ServiceNumber", mdata.ServiceNumber);
                    //        com.Parameters.AddWithValue("@p_TaskNumber", mdata.TaskNumber);
                    //        com.Parameters.AddWithValue("@p_CoversheetTier", mdata.CoversheetTier);
                    //        com.Parameters.AddWithValue("@p_Editor", mdata.Editor);
                    //        com.Parameters.AddWithValue("@p_ChargeCode", mdata.ChargeCode);
                    //        com.Parameters.AddWithValue("@p_TargetPressDate", mdata.TargetPressDate);
                    //        com.Parameters.AddWithValue("@p_TaskType", mdata.TaskType);
                    //        com.Parameters.AddWithValue("@p_GuideCard", mdata.GuideCard);
                    //        com.Parameters.AddWithValue("@p_FurtherInstructions", mdata.FurtherInstructions);
                    //        com.Parameters.AddWithValue("@p_UpdateType", mdata.UpdateType);
                    //        com.Parameters.AddWithValue("@p_GeneralData", mdata.GeneralData);
                    //        com.Parameters.AddWithValue("@p_SpecialInstruction", mdata.SpecialInstruction);
                    //        com.Parameters.AddWithValue("@p_CodingDueDate", mdata.CodingDueDate);
                    //        com.Parameters.AddWithValue("@p_XMLEditing", mdata.XMLEditing);
                    //        com.Parameters.AddWithValue("@p_OnlineDueDate", mdata.OnlineDueDate);
                    //        com.Parameters.AddWithValue("@p_OnlineStatus", mdata.OnlineStatus);
                    //        com.Parameters.AddWithValue("@p_CorrectionDueDate", mdata.CorrectionDueDate);
                    //        com.Parameters.AddWithValue("@p_CorrectionData", mdata.CorrectionData);
                    //        if (dbConnection.State == ConnectionState.Closed)
                    //            dbConnection.Open();
                    //        Count = com.ExecuteNonQuery();
                    //        com.Parameters.Clear();
                    //    }
                    //    if (Count > 0)
                    //    {
                    //        mdata.Response = "Y";
                    //        //SendEmail(mdata);
                    //    }
                    //    else
                    //    {
                    //        mdata.Response = "N";
                    //        mdata.ErrorMessage = "Coversheet data could not be added";
                    //    }
                    //}
                    //else
                    //{

                }


                //}
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

                // Json(new { success = false, responseText = "registration failed, please check", JsonRequestBehavior.AllowGet);
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
            return Json(mdata, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetCoversheetDatafields()
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

            List<CoversheetData> mdata = new List<CoversheetData>();
            DataTable dt = new DataTable();

            cmd = new MySqlCommand("GetAllCoversheetData", dbConnection);
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
                mdata.Add(new CoversheetData
                {

                    CoversheetID = Convert.ToInt32(dr["CoversheetID"].ToString()),
                    ManuscriptID = dr["ManuscriptID"].ToString(),
                    //JobNumber = Convert.ToInt32(dr["JobNumber"].ToString()),
                    CoversheetNumber = dr["CoversheetNumber"].ToString(),
                    CoversheetTier = dr["CoversheetTier"].ToString(),
                    //TaskNumber = dr["TaskNumber"].ToString(),

                    BPSProductID = dr["BPSProductID"].ToString(),
                    ServiceNumber = dr["ServiceNumber"].ToString(),
                    CurrentTask = dr["CurrentTask"].ToString(),
                    TaskStatus = dr["TaskStatus"].ToString(),
                    //TargetPressDate = dr.Field<DateTime?>("TargetPressDate"),
                    //ActualPressDate = dr.Field<DateTime?>("ActualPressDate"),
                    //TargetPressDate = DateTime.ParseExact(dr["TargetPressDate"].ToString(), "yyyy/MM/dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                    //ActualPressDate = DateTime.ParseExact(dr["ActualPressDate"].ToString(), "yyyy/MM/dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                    //CodingDueDate = dr.Field<DateTime?>("CodingDueDate"),
                    //CodingStartDate = dr.Field<DateTime?>("CodingStartDate"),
                    //CodingDoneDate = dr.Field<DateTime?>("CodingDoneDate"),
                    //SubsequentPass = dr["SubsequentPass"].ToString(),
                    //OnlineDueDate = dr.Field<DateTime?>("OnlineDueDate"),
                    //OnlineStartDate = dr.Field<DateTime?>("OnlineStartDate"),
                    //OnlineDoneDate = dr.Field<DateTime?>("OnlineDoneDate"),
                    //OnlineTimeliness = dr["OnlineTimeliness"].ToString(),
                    //ReasonIfLate = dr["ReasonIfLate"].ToString()

                });
            }
            dbConnection.Close();
            return Json(mdata, JsonRequestBehavior.AllowGet);
        } */
        #endregion

        public async Task<ActionResult> GetCoversheetData(string bpsproductid, string servicenumber, int? userAccess)
        {
            var username = userAccess.HasValue ? (string)Session["UserName"] : string.Empty;
            var result = await _coversheetService.GetCoversheetDataByProductAndServiceAsync(bpsproductid, servicenumber, username);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region [Old Code] GetCoversheetData
        /* public ActionResult GetCoversheetData(string bpsproductid, string servicenumber)
        {
            if (dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();

            List<CoversheetData> mdata = new List<CoversheetData>();
            DataTable dt = new DataTable();

            cmd = new MySqlCommand("GetCoversheetDataByID", dbConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@p_BPSProductID", bpsproductid);
            cmd.Parameters.AddWithValue("@p_ServiceNumber", servicenumber);
            adp = new MySqlDataAdapter(cmd);
            adp.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                mdata.Add(new CoversheetData
                {
                    CoversheetID = Convert.ToInt32(dr["CoversheetID"].ToString()),
                    CoversheetNumber = dr["CoversheetNumber"].ToString(),
                    CoversheetTier = dr["CoversheetTier"].ToString(),
                    TaskNumber = dr["TaskNumber"].ToString(),
                    BPSProductID = dr["BPSProductID"].ToString(),
                    ServiceNumber = dr["ServiceNumber"].ToString(),
                    GuideCard = dr["GuideCard"].ToString(),
                    LocationOfManuscript = dr["LocationOfManuscript"].ToString(),
                    FurtherInstructions = dr["FurtherInstruction"].ToString(),
                    CurrentTask = dr["CurrentTask"].ToString(),
                    TaskStatus = dr["TaskStatus"].ToString(),
                    TargetPressDate = dr.Field<DateTime?>("TargetPressDate"),
                    ActualPressDate = dr.Field<DateTime?>("ActualPressDate"),
                    CodingDueDate = dr.Field<DateTime?>("CodingDueDate"),
                    CodingStartDate = dr.Field<DateTime?>("CodingStartDate"),
                    CodingDoneDate = dr.Field<DateTime?>("CodingDoneDate"),
                    OnlineDueDate = dr.Field<DateTime?>("OnlineDueDate"),
                    OnlineStartDate = dr.Field<DateTime?>("OnlineStartDate"),
                    OnlineDoneDate = dr.Field<DateTime?>("OnlineDoneDate"),
                    OnlineTimeliness = dr["OnlineTimeliness"].ToString(),
                    ReasonIfLate = dr["ReasonIfLate"].ToString(),
                    JobOwner = dr["JobOwner"].ToString()
                });
            }
            dbConnection.Close();
            return Json(mdata, JsonRequestBehavior.AllowGet);
        } */
        #endregion

        [HttpGet]
        public ActionResult EditCoversheet(string coversheetid, string bpsproductid, string servicenumber)
        {
            EditCoversheetViewModel mdata = new EditCoversheetViewModel();
            mdata.model1 = new CoversheetData();
            try
            {

                DataTable dt = new DataTable();

                cmd = new MySqlCommand("GetCoversheetDataByCoversheetID", dbConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@p_CoversheetID", coversheetid);
                adp = new MySqlDataAdapter(cmd);
                adp.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {

                    mdata.model1.CoversheetID = Convert.ToInt32(dr["CoversheetID"].ToString());
                    //mdata.ManuscriptID = dr["ManuscriptID"].ToString();
                    //mdata.model1.CoversheetNumber = dr["CoversheetNumber"].ToString();
                    mdata.model1.CoversheetNumber = "TBD";
                    mdata.model1.BPSProductID = dr["BPSProductID"].ToString();
                    mdata.model1.ServiceNumber = dr["ServiceNumber"].ToString();
                    mdata.model1.TaskNumber = dr["TaskNumber"].ToString();
                    mdata.model1.CoversheetTier = dr["CoversheetTier"].ToString();
                    mdata.model1.Editor = dr["Editor"].ToString();
                    mdata.model1.ChargeCode = dr["ChargeCode"].ToString();
                    mdata.model1.TargetPressDate = dr.Field<DateTime?>("TargetPressDate");
                    mdata.model1.ActualPressDate = dr.Field<DateTime?>("ActualPressDate");
                    mdata.model1.CurrentTask = dr["CurrentTask"].ToString();
                    mdata.model1.TaskStatus = dr["TaskStatus"].ToString();
                    mdata.model1.TaskType = dr["TaskType"].ToString();
                    mdata.model1.GuideCard = dr["GuideCard"].ToString();
                    mdata.model1.FurtherInstruction = dr["FurtherInstruction"].ToString();
                    mdata.model1.UpdateType = dr["UpdateType"].ToString();
                    mdata.model1.GeneralData = dr["GeneralData"].ToString();
                    mdata.model1.SpecialInstruction = dr["SpecialInstruction"].ToString();
                    mdata.model1.AcceptedDate = dr.Field<DateTime?>("AcceptedDate");
                    mdata.model1.JobOwner = dr["JobOwner"].ToString();
                    mdata.model1.UpdateEmailCC = dr["UpdateEmailCC"].ToString();
                    mdata.model1.IsXMLEditing = Convert.ToBoolean(Convert.ToInt32(dr["XMLEditing"]));
                    mdata.model1.CodingDueDate = dr.Field<DateTime?>("CodingDueDate");
                    mdata.model1.CodingDoneDate = dr.Field<DateTime?>("CodingDoneDate");
                    mdata.model1.SubsequentPass = dr["SubsequentPass"].ToString();
                    mdata.model1.SubTask = dr["SubTask"].ToString();
                    mdata.model1.PDFQCStatus = dr["PDFQCStatus"].ToString();
                    mdata.model1.OnlineDueDate = dr.Field<DateTime?>("OnlineDueDate");
                    mdata.model1.OnlineStartDate = dr.Field<DateTime?>("OnlineStartDate");
                    mdata.model1.OnlineDoneDate = dr.Field<DateTime?>("OnlineDoneDate");
                    //mdata.model1.OnlineStatus = Convert.ToBoolean(Convert.ToInt32(dr["OnlineStatus"]));   TODO: uncomment
                    //mdata.model1.Attachment = dr["Attachment"].ToString();
                    mdata.model1.CorrectionDueDate = dr.Field<DateTime?>("CorrectionDueDate");
                    mdata.model1.CorrectionData = dr["CorrectionData"].ToString();
                    mdata.model1.OnlineTimeliness = dr["OnlineTimeliness"].ToString();
                    mdata.model1.ReasonIfLate = dr["ReasonIfLate"].ToString();
                    mdata.model1.DateCreated = Convert.ToDateTime(dr["DateCreated"].ToString());

                }
                return PartialView(mdata);
            }



            catch (Exception ex)
            {
                ModelState.AddModelError("ErrorMessage", ex.Message);
                mdata.model1.ErrorMessage = ex.Message;
                return PartialView(mdata);
            }
        }

        [HttpGet]
        public ActionResult EditCodingTLCoversheet(string coversheetid, string bpsproductid, string servicenumber)
        {
            EditCoversheetViewModel mdata = new EditCoversheetViewModel();
            mdata.model1 = new CoversheetData();
            try
            {

                DataTable dt = new DataTable();

                cmd = new MySqlCommand("GetCoversheetDataByCoversheetID", dbConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@p_CoversheetID", coversheetid);
                adp = new MySqlDataAdapter(cmd);
                adp.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {

                    mdata.model1.CoversheetID = Convert.ToInt32(dr["CoversheetID"].ToString());
                    //mdata.ManuscriptID = dr["ManuscriptID"].ToString();
                    mdata.model1.CoversheetNumber = dr["CoversheetNumber"].ToString();
                    mdata.model1.BPSProductID = dr["BPSProductID"].ToString();
                    mdata.model1.ServiceNumber = dr["ServiceNumber"].ToString();
                    mdata.model1.TaskNumber = dr["TaskNumber"].ToString();
                    mdata.model1.CoversheetTier = dr["CoversheetTier"].ToString();
                    mdata.model1.Editor = dr["Editor"].ToString();
                    mdata.model1.ChargeCode = dr["ChargeCode"].ToString();
                    mdata.model1.TargetPressDate = dr.Field<DateTime?>("TargetPressDate");
                    mdata.model1.ActualPressDate = dr.Field<DateTime?>("ActualPressDate");
                    mdata.model1.CurrentTask = dr["CurrentTask"].ToString();
                    mdata.model1.TaskStatus = dr["TaskStatus"].ToString();
                    mdata.model1.TaskType = dr["TaskType"].ToString();
                    mdata.model1.GuideCard = dr["GuideCard"].ToString();
                    mdata.model1.FurtherInstruction = dr["FurtherInstruction"].ToString();
                    mdata.model1.UpdateType = dr["UpdateType"].ToString();
                    mdata.model1.GeneralData = dr["GeneralData"].ToString();
                    mdata.model1.SpecialInstruction = dr["SpecialInstruction"].ToString();
                    mdata.model1.AcceptedDate = dr.Field<DateTime?>("AcceptedDate");
                    mdata.model1.JobOwner = dr["JobOwner"].ToString();
                    mdata.model1.UpdateEmailCC = dr["UpdateEmailCC"].ToString();
                    mdata.model1.IsXMLEditing = Convert.ToBoolean(Convert.ToInt32(dr["XMLEditing"]));
                    mdata.model1.CodingDueDate = dr.Field<DateTime?>("CodingDueDate");
                    mdata.model1.CodingDoneDate = dr.Field<DateTime?>("CodingDoneDate");
                    mdata.model1.SubsequentPass = dr["SubsequentPass"].ToString();
                    mdata.model1.SubTask = dr["SubTask"].ToString();
                    mdata.model1.PDFQCStatus = dr["PDFQCStatus"].ToString();
                    mdata.model1.OnlineDueDate = dr.Field<DateTime?>("OnlineDueDate");
                    mdata.model1.OnlineStartDate = dr.Field<DateTime?>("OnlineStartDate");
                    mdata.model1.OnlineDoneDate = dr.Field<DateTime?>("OnlineDoneDate");
                    //mdata.model1.OnlineStatus = Convert.ToBoolean(Convert.ToInt32(dr["OnlineStatus"]));   TODO: uncomment
                    //mdata.model1.Attachment = dr["Attachment"].ToString();
                    mdata.model1.CorrectionDueDate = dr.Field<DateTime?>("CorrectionDueDate");
                    mdata.model1.CorrectionData = dr["CorrectionData"].ToString();
                    mdata.model1.OnlineTimeliness = dr["OnlineTimeliness"].ToString();
                    mdata.model1.ReasonIfLate = dr["ReasonIfLate"].ToString();
                    mdata.model1.DateCreated = Convert.ToDateTime(dr["DateCreated"].ToString());

                }
                return PartialView(mdata);
            }



            catch (Exception ex)
            {
                ModelState.AddModelError("ErrorMessage", ex.Message);
                mdata.model1.ErrorMessage = ex.Message;
                return PartialView(mdata);
            }
        }

        [HttpGet]
        public ActionResult EditPECoversheet(string coversheetid, string bpsproductid, string servicenumber)
        {
            CoversheetData mdata = new CoversheetData();
            try
            {
                DataTable dt = new DataTable();

                cmd = new MySqlCommand("GetCoversheetDataByCoversheetID", dbConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@p_CoversheetID", coversheetid);
                adp = new MySqlDataAdapter(cmd);
                adp.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {

                    mdata.CoversheetID = Convert.ToInt32(dr["CoversheetID"].ToString());
                    //mdata.ManuscriptID = dr["ManuscriptID"].ToString();
                    mdata.CoversheetNumber = dr["CoversheetNumber"].ToString();
                    mdata.BPSProductID = dr["BPSProductID"].ToString();
                    mdata.ServiceNumber = dr["ServiceNumber"].ToString();
                    mdata.TaskNumber = dr["TaskNumber"].ToString();
                    mdata.CoversheetTier = dr["CoversheetTier"].ToString();
                    mdata.Editor = dr["Editor"].ToString();
                    mdata.ChargeCode = dr["ChargeCode"].ToString();
                    mdata.TargetPressDate = dr.Field<DateTime?>("TargetPressDate");
                    mdata.ActualPressDate = dr.Field<DateTime?>("ActualPressDate");
                    mdata.CurrentTask = dr["CurrentTask"].ToString();
                    mdata.TaskStatus = dr["TaskStatus"].ToString();
                    mdata.TaskType = dr["TaskType"].ToString();
                    mdata.GuideCard = dr["GuideCard"].ToString();
                    mdata.FurtherInstruction = dr["FurtherInstruction"].ToString();
                    mdata.UpdateType = dr["UpdateType"].ToString();
                    mdata.GeneralData = dr["GeneralData"].ToString();
                    mdata.SpecialInstruction = dr["SpecialInstruction"].ToString();
                    mdata.AcceptedDate = dr.Field<DateTime?>("AcceptedDate");
                    mdata.JobOwner = dr["JobOwner"].ToString();
                    mdata.UpdateEmailCC = dr["UpdateEmailCC"].ToString();
                    mdata.IsXMLEditing = Convert.ToBoolean(Convert.ToInt32(dr["XMLEditing"]));
                    mdata.CodingDueDate = dr.Field<DateTime?>("CodingDueDate");
                    mdata.CodingDoneDate = dr.Field<DateTime?>("CodingDoneDate");
                    mdata.SubsequentPass = dr["SubsequentPass"].ToString();
                    mdata.SubTask = dr["SubTask"].ToString();
                    mdata.PDFQCStatus = dr["PDFQCStatus"].ToString();
                    mdata.OnlineDueDate = dr.Field<DateTime?>("OnlineDueDate");
                    mdata.OnlineStartDate = dr.Field<DateTime?>("OnlineStartDate");
                    mdata.OnlineDoneDate = dr.Field<DateTime?>("OnlineDoneDate");
                    //mdata.OnlineStatus = Convert.ToBoolean(Convert.ToInt32(dr["OnlineStatus"]));  TODO: uncomment
                    //mdata.Attachment = dr["Attachment"].ToString();
                    mdata.CorrectionDueDate = dr.Field<DateTime?>("CorrectionDueDate");
                    mdata.CorrectionData = dr["CorrectionData"].ToString();
                    mdata.OnlineTimeliness = dr["OnlineTimeliness"].ToString();
                    mdata.ReasonIfLate = dr["ReasonIfLate"].ToString();
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

        public void SendEmail(CoversheetData mdata)
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
                        "<span style='font-size:8.0pt;font-family:Verdana'> " + mdata.FurtherInstruction + " </span>" +
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

        [HttpPost]
        public JsonResult UpdateStartDateCoding(EditCoversheetViewModelBase mdata)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(mdata.model1.BPSProductID) && !string.IsNullOrEmpty(mdata.model1.ServiceNumber))
                    {

                        var Username = Session["UserName"];
                        //var JobNumber = Session["JobNumber"];
                        MySqlCommand com = new MySqlCommand("UpdateCoversheetStartDateCoding", dbConnection);
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@p_Username", Username);
                        com.Parameters.AddWithValue("@p_CoversheetID", mdata.model1.CoversheetID);
                        com.Parameters.AddWithValue("@p_BPSProductID", mdata.model1.BPSProductID);
                        com.Parameters.AddWithValue("@p_ServiceNumber", mdata.model1.ServiceNumber);
                        com.Parameters.AddWithValue("@p_CodingStartDate", mdata.model1.CodingStartDate);
                        if (mdata.model1.CodingDoneDate == null)
                        {
                            mdata.model1.TaskStatus = "Assigned";
                            com.Parameters.AddWithValue("@p_TaskStatus", mdata.model1.TaskStatus);
                        }
                        if (mdata.model1.CodingStartDate != null && mdata.model1.CodingDoneDate != null)
                        {
                            mdata.model1.TaskStatus = "Closed";
                            com.Parameters.AddWithValue("@p_TaskStatus", mdata.model1.TaskStatus);
                        }
                        if (dbConnection.State == ConnectionState.Closed)
                            dbConnection.Open();
                        int Count = com.ExecuteNonQuery();

                        if (Count > 0)
                        {
                            mdata.model1.Response = "Y";

                        }
                        else
                        {
                            mdata.model1.Response = "N";
                            mdata.model1.ErrorMessage = "Coding data could not be updated";
                        }

                    }


                }
                else
                {
                    foreach (var Key in ModelState.Keys)
                    {
                        if (ModelState[Key].Errors.Count > 0)
                        {
                            mdata.model1.Response = "N";
                            mdata.model1.ErrorMessage = ModelState[Key].Errors[0].ErrorMessage;

                            //return Json(new { success = false, responseText = "registration failed, please check", JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ErrorMessage", ex.Message);
                ViewBag.ErrorMessage = ex.Message;
                mdata.model1.Response = "N";
                mdata.model1.ErrorMessage = "Error : " + ex.Message;
            }
            finally
            {
                dbConnection.Close();
            }
            return Json(mdata, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateDoneDateCoding(EditCoversheetViewModelBase mdata)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(mdata.model1.BPSProductID) && !string.IsNullOrEmpty(mdata.model1.ServiceNumber))
                    {

                        var Username = Session["UserName"];
                        //var JobNumber = Session["JobNumber"];
                        MySqlCommand com = new MySqlCommand("UpdateCoversheetDoneDateCoding", dbConnection);
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@p_Username", Username);
                        com.Parameters.AddWithValue("@p_CoversheetID", mdata.model1.CoversheetID);
                        com.Parameters.AddWithValue("@p_BPSProductID", mdata.model1.BPSProductID);
                        com.Parameters.AddWithValue("@p_ServiceNumber", mdata.model1.ServiceNumber);
                        com.Parameters.AddWithValue("@p_CodingDoneDate", mdata.model1.CodingDoneDate);
                        if (mdata.model1.CodingDoneDate == null)
                        {
                            mdata.model1.TaskStatus = "Assigned";
                            com.Parameters.AddWithValue("@p_TaskStatus", mdata.model1.TaskStatus);
                        }
                        if (mdata.model1.CodingStartDate != null && mdata.model1.CodingDoneDate != null)
                        {
                            mdata.model1.TaskStatus = "Closed";
                            com.Parameters.AddWithValue("@p_TaskStatus", mdata.model1.TaskStatus);
                        }
                        if (dbConnection.State == ConnectionState.Closed)
                            dbConnection.Open();
                        int Count = com.ExecuteNonQuery();

                        if (Count > 0)
                        {
                            mdata.model1.Response = "Y";

                        }
                        else
                        {
                            mdata.model1.Response = "N";
                            mdata.model1.ErrorMessage = "Coding data could not be updated";
                        }

                    }


                }
                else
                {
                    foreach (var Key in ModelState.Keys)
                    {
                        if (ModelState[Key].Errors.Count > 0)
                        {
                            mdata.model1.Response = "N";
                            mdata.model1.ErrorMessage = ModelState[Key].Errors[0].ErrorMessage;

                            //return Json(new { success = false, responseText = "registration failed, please check", JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ErrorMessage", ex.Message);
                ViewBag.ErrorMessage = ex.Message;
                mdata.model1.Response = "N";
                mdata.model1.ErrorMessage = "Error : " + ex.Message;
            }
            finally
            {
                dbConnection.Close();
            }
            return Json(mdata, JsonRequestBehavior.AllowGet);
        }
    }
}