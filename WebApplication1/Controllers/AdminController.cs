using System;
using System.Web.Mvc;
using System.Collections.Generic;
using JobTrack.Models.Employee;
using JobTrack.Models.Admin;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using JobTrack.Models.Job;
using System.Threading.Tasks;
using JobTrack.Models;
using JobTrack.Services.Interfaces;
using Newtonsoft.Json;

namespace JobTrack.Controllers
{
    public class AdminController : Controller
    {
        // CONNECTION STRING
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        // DECLARE MODEL
        public EmployeeModel dbConnModel = new EmployeeModel();
        public ProductDatabaseModel dbManuModel = new ProductDatabaseModel();

        private readonly IPublicationAssignService _publicationAssignService;
        private readonly IEmployeeService _employeeService;

        public AdminController(IPublicationAssignService publicationAssignService, IEmployeeService employeeService)
        {
            _publicationAssignService = publicationAssignService;
            _employeeService = employeeService;
        }

        public ActionResult _EditEmployeeView()
        {
            return PartialView();
        }

        public ActionResult _EditEmployeeView2()
        {
            return PartialView();
        }

        public ActionResult _EditPublicationView()
        {
            return PartialView();
        }

        public ActionResult _TextFieldView(FormFieldModel model)
        {
            return PartialView(model);
        }

        public ActionResult _CheckboxView(FormFieldModel model)
        {
            return PartialView(model);
        }

        public ActionResult TopMenu()
        {
            return PartialView("_Topbar");
        }

        public ActionResult SideMenu()
        {
            return PartialView("_SidebarAdmin");
        }

        public ActionResult MainForm()
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

        public ActionResult AllJob()
        {
            #region Check Session
            if (Session["UserName"] == null)
            {
                TempData["alertMessage"] = "You must log in to continue";
                return RedirectToAction("Login", "Login");
            }

            var userAccess = Session["UserAccess"];

            #endregion
            return View();
        }

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
        public ActionResult Employees()
        {
            return View();
        }

        public ActionResult ProductDatabase()
        {
            List<ProductDatabaseData> mdata = new List<ProductDatabaseData>();
            DataTable dt = new DataTable();

            cmd = new MySqlCommand("GetAllPubSched_MT", dbConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@int_owner", owner);
            adp = new MySqlDataAdapter(cmd);
            adp.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                mdata.Add(new ProductDatabaseData
                {


                    PubSchedID = Convert.ToInt32(dr["PubSchedID"].ToString()),
                    isSPI = dr["isSPI"].ToString(),
                    OrderNumber = Convert.ToInt32(dr["OrderNumber"].ToString()),
                    BudgetPressMonth = dr["BudgetPressMonth"].ToString(),
                    PubSchedTier = dr["PubSchedTier"].ToString(),
                    PubSchedTeam = dr["PubSchedTeam"].ToString(),
                    BPSProductID = dr["BPSProductID"].ToString(),
                    LegalEditor = dr["LegalEditor"].ToString(),
                    ChargeType = dr["ChargeType"].ToString(),
                    ProductChargeCode = dr["ProductChargeCode"].ToString(),
                    BPSProductIDMaster = dr["BPSProductIDMaster"].ToString(),
                    BPSSublist = dr["BPSSublist"].ToString(),
                    ServiceUpdate = dr["ServiceUpdate"].ToString(),
                    LastManuscriptHandover = Convert.ToDateTime(dr["LastManuscriptHandover"].ToString()),
                    BudgetPressDate = Convert.ToDateTime(dr["BudgetPressDate"].ToString()),
                    RevisedPressDate = Convert.ToDateTime(dr["RevisedPressDate"].ToString()),
                    ReasonForRevisedPressDate = dr["ReasonForRevisedPressDate"].ToString(),
                    ServiceNumber = dr["ServiceNumber"].ToString(),
                    ForecastPages = Convert.ToInt32(dr["ForecastPages"].ToString()),
                    ActualPages = Convert.ToInt32(dr["ActualPages"].ToString()),
                    DataFromLE = Convert.ToDateTime(dr["DataFromLE"].ToString()),
                    DataFromLEG = Convert.ToDateTime(dr["DataFromLEG"].ToString()),
                    DataFromCoding = Convert.ToDateTime(dr["DataFromCoding"].ToString()),
                    isReceived = Convert.ToInt32(dr["isReceived"].ToString()),
                    isCompleted = Convert.ToInt32(dr["isCompleted"].ToString()),
                    AheadOnTime = Convert.ToInt32(dr["AheadOnTime"].ToString()),
                    WithRevisedPressDate = dr["WithRevisedPressDate"].ToString(),
                    ActualPressDate = Convert.ToDateTime(dr["ActualPressDate"].ToString()),
                    ServiceAndBPSProductID = dr["ServiceAndBPSProductID"].ToString(),
                    PubSchedRemarks = dr["PubSchedRemarks"].ToString(),

                });
            }
            return View(mdata);
        }
        public ActionResult Tables()
        {
            return View();
        }

        public ActionResult LoadEmployees()
        {
            dbConnModel.Employees = new List<EmployeeData>();

            //string userID = user_ID;
            //userID = Session["EmployeeID"].ToString();

            dbConnection.Open();

            string storedProcName;
            storedProcName = "GetAllUsers";

            using (MySqlCommand command = new MySqlCommand(storedProcName, dbConnection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                //command.Parameters.AddWithValue("@pUserID", userID);

                MySqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {

                        dbConnModel.Employees.Add(new EmployeeData()
                        {
                            EmployeeID = int.Parse(reader[0].ToString()),
                            UserAccess = reader[1].ToString(),
                            CreatedDate = DateTime.Parse(reader[2].ToString()),
                            UserName = reader[3].ToString(),
                            FirstName = reader[4].ToString(),
                            LastName = reader[5].ToString(),
                            EmailAddress = reader[6].ToString(),
                            isManager = int.Parse(reader[7].ToString()),
                            isEditorialContact = int.Parse(reader[8].ToString()),
                            isEmailList = int.Parse(reader[9].ToString()),
                            isMandatoryRecepient = int.Parse(reader[10].ToString()),
                            isShowUser = int.Parse(reader[11].ToString()),
                            PasswordUpdateDate = DateTime.Parse(reader[12].ToString())

                        });
                    }
                }
                reader.Close();
            }
            dbConnection.Close();

            return Json(dbConnModel.Employees, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllUserAccess()
        {
            dbConnModel.EmployeeUserAccessDropdowns = new List<EmployeeAccess>();

            //string userID = user_ID;
            //userID = Session["EmployeeID"].ToString();

            dbConnection.Open();

            string storedProcName;
            storedProcName = "GetAllUserAccess";

            using (MySqlCommand command = new MySqlCommand(storedProcName, dbConnection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                //command.Parameters.AddWithValue("@pUserID", userID);

                MySqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {

                        dbConnModel.EmployeeUserAccessDropdowns.Add(new EmployeeAccess()
                        {
                            UserAccessID = int.Parse(reader[0].ToString()),
                            UserAccess = reader[1].ToString(),
                        });
                    }
                }
                reader.Close();
            }
            dbConnection.Close();
            return Json(dbConnModel.EmployeeUserAccessDropdowns, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertEmployee(string pFirstName, string pLastName, string pEmailAddress, string pUserAccess, string pUserName, int pHiddenField)
        {
            if (ModelState.IsValid)
            {
                if (pHiddenField < 0)
                {
                    if (!IsUserExist(pUserName, pEmailAddress))

                    {
                        MySqlCommand com = new MySqlCommand("InsertUser", dbConnection);
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@p_FirstName", pFirstName);
                        com.Parameters.AddWithValue("@p_LastName", pLastName);
                        com.Parameters.AddWithValue("@p_EmailAddress", pEmailAddress);
                        com.Parameters.AddWithValue("@p_UserAccess", pUserAccess);
                        com.Parameters.AddWithValue("@p_UserName", pUserName);
                        //com.Parameters.AddWithValue("@pPassword", pPassword);

                        dbConnection.Open();
                        com.ExecuteNonQuery();
                        dbConnection.Close();

                        return Json(new { success = true, responseText = "registration successful" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, responseText = "user already exist" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    MySqlCommand com = new MySqlCommand("UpdateUserByUserID", dbConnection);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@p_FirstName", pFirstName);
                    com.Parameters.AddWithValue("@p_LastName", pLastName);
                    com.Parameters.AddWithValue("@p_EmailAddress", pEmailAddress);
                    com.Parameters.AddWithValue("@p_UserAccess", pUserAccess);
                    com.Parameters.AddWithValue("@p_UserName", pUserName);
                    //com.Parameters.AddWithValue("@pPassword", pPassword);

                    dbConnection.Open();
                    com.ExecuteNonQuery();
                    dbConnection.Close();

                    return Json(new { success = true, responseText = "registration successful" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateEmployee(EmployeeData model)
        {
            var result = new JsonResultModel();

            try
            {
                result.Collection = await _employeeService.UpdateEmployeeAsync(model);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }

            return Json(result);
        }

        private bool IsUserExist(string username, string email)
        {
            bool IsUserExist = false;
            using (MySqlCommand command = new MySqlCommand("GetUserExist", dbConnection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_UserName", username);
                command.Parameters.AddWithValue("@p_EmailAddress", email);
                MySqlDataAdapter sda = new MySqlDataAdapter(command);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                dbConnection.Open();
                int i = command.ExecuteNonQuery();
                dbConnection.Close();
                if (dt.Rows.Count > 0)
                {
                    IsUserExist = true;
                }
            }
            return IsUserExist;
        }

        //[HttpGet]
        //public ActionResult UpdateEmployee(int? EmployeeID)
        //{
        //    dbConnModel.Employees = new List<EmployeeData>();

        //    //string userID = user_ID;
        //    //userID = Session["EmployeeID"].ToString();

        //    dbConnection.Open();

        //    string storedProcName;
        //    storedProcName = "GetUserByUserID";

        //    using (MySqlCommand command = new MySqlCommand(storedProcName, dbConnection))
        //    {
        //        command.CommandType = System.Data.CommandType.StoredProcedure;
        //        command.Parameters.AddWithValue("@p_UserID", EmployeeID);

        //        MySqlDataReader reader = command.ExecuteReader();
        //        if (reader.HasRows)
        //        {
        //            while (reader.Read())
        //            {

        //                dbConnModel.Employees.Add(new EmployeeData()
        //                {
        //                    EmployeeID = int.Parse(reader[0].ToString()),
        //                    UserAccess = reader[1].ToString(),
        //                    CreatedDate = DateTime.Parse(reader[2].ToString()),
        //                    UserName = reader[3].ToString(),
        //                    FirstName = reader[4].ToString(),
        //                    LastName = reader[5].ToString(),
        //                    EmailAddress = reader[6].ToString(),
        //                    isManager = int.Parse(reader[7].ToString()),
        //                    isEditorialContact = int.Parse(reader[8].ToString()),
        //                    isEmailList = int.Parse(reader[9].ToString()),
        //                    isMandatoryRecepient = int.Parse(reader[10].ToString()),
        //                    isShowUser = int.Parse(reader[11].ToString()),
        //                    PasswordUpdateDate = DateTime.Parse(reader[12].ToString())

        //                });
        //            }
        //        }
        //        reader.Close();
        //    }
        //    dbConnection.Close();

        //    return Json(dbConnModel.Employees, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult AddNewProductDatabase()
        {
            ProductDatabaseData mdata = new ProductDatabaseData();
            //LastManuscriptID mid = new LastManuscriptID();
            //this.ViewBag.Service = new SelectList(mid.GetLastManuscriptID(), "service_id", "service_no");
            try
            {
                //TempData["ManuscriptTier"] = new SelectList(GetAllPubschedTier(), "PubSchedTier", "PubSchedTier");
                //TempData["BPSProductID"] = new SelectList(GetAllPubschedBPSProductID(), "PubschedBPSProductID", "PubschedBPSProductID");
                //TempData["UpdateType"] = new SelectList(GetAllTurnAroundTime(), "TurnAroundTimeID", "UpdateType");
                return PartialView(mdata);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ErrorMessage", ex.Message);
                mdata.ErrorMessage = ex.Message;
                return PartialView(mdata);
            }
        }

        public ActionResult EditProductDatabase(int PubSchedID)
        {
            ProductDatabaseData mdata = new ProductDatabaseData();
            try
            {
                //TempData["ManuscriptTier"] = new SelectList(GetAllPubschedTier(), "PubSchedTier", "PubSchedTier");
                //TempData["BPSProductID"] = new SelectList(GetAllPubschedBPSProductID(), "PubschedBPSProductID", "PubschedBPSProductID");
                //TempData["UpdateType"] = new SelectList(GetAllTurnAroundTime(), "TurnAroundTimeID", "UpdateType");
                DataTable dt = new DataTable();

                cmd = new MySqlCommand("GetProductDatabaseByID", dbConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@p_PubSchedID", PubSchedID);
                adp = new MySqlDataAdapter(cmd);
                adp.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {

                    mdata.isSPI = dr["isSPI"].ToString();
                    mdata.OrderNumber = Convert.ToInt32(dr["OrderNumber"].ToString());
                    mdata.BudgetPressMonth = dr["BudgetPressMonth"].ToString();
                    mdata.PubSchedTier = dr["PubSchedTier"].ToString();
                    mdata.PubSchedTeam = dr["PubSchedTeam"].ToString();
                    mdata.BPSProductID = dr["BPSProductID"].ToString();
                    mdata.LegalEditor = dr["LegalEditor"].ToString();
                    mdata.ChargeType = dr["ChargeType"].ToString();
                    mdata.ProductChargeCode = dr["ProductChargeCode"].ToString();
                    mdata.BPSProductIDMaster = dr["BPSProductIDMaster"].ToString();
                    mdata.BPSSublist = dr["BPSSublist"].ToString();
                    mdata.ServiceUpdate = dr["ServiceUpdate"].ToString();
                    mdata.LastManuscriptHandover = Convert.ToDateTime(dr["LastManuscriptHandover"].ToString());
                    mdata.BudgetPressDate = Convert.ToDateTime(dr["BudgetPressDate"].ToString());
                    mdata.RevisedPressDate = Convert.ToDateTime(dr["RevisedPressDate"].ToString());
                    mdata.ReasonForRevisedPressDate = dr["ReasonForRevisedPressDate"].ToString();
                    mdata.ServiceNumber = dr["ServiceNumber"].ToString();
                    mdata.ForecastPages = Convert.ToInt32(dr["ForecastPages"].ToString());
                    mdata.ActualPages = Convert.ToInt32(dr["ActualPages"].ToString());
                    mdata.DataFromLE = Convert.ToDateTime(dr["DataFromLE"].ToString());
                    mdata.DataFromLEG = Convert.ToDateTime(dr["DataFromLEG"].ToString());
                    mdata.DataFromCoding = Convert.ToDateTime(dr["DataFromCoding"].ToString());
                    mdata.isReceived = Convert.ToInt32(dr["isReceived"].ToString());
                    mdata.isCompleted = Convert.ToInt32(dr["isCompleted"].ToString());
                    mdata.AheadOnTime = Convert.ToInt32(dr["AheadOnTime"].ToString());
                    mdata.WithRevisedPressDate = dr["WithRevisedPressDate"].ToString();
                    mdata.ActualPressDate = Convert.ToDateTime(dr["ActualPressDate"].ToString());
                    mdata.ServiceAndBPSProductID = dr["ServiceAndBPSProductID"].ToString();
                    mdata.PubSchedRemarks = dr["PubSchedRemarks"].ToString();

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

        public async Task<string> GetPublicationAssignments()
        {
            var result = new JsonResultModel();

            try
            {
                result.Collection = await _publicationAssignService.GetAllPublicationAssignmentModelAsync();
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }

            return JsonConvert.SerializeObject(result);
        }

        public async Task<ActionResult> UpdatePublicationAssignment(PublicationAssignmentModel model)
        {
            var result = new JsonResultModel();

            try
            {
                result.Collection = await _publicationAssignService.UpdatePublicationAssignmentAsync(model);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }

            return Json(result);
        }
    }
}