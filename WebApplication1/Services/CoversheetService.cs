using JobTrack.Models;
using JobTrack.Models.Coversheet;
using JobTrack.Models.Enums;
using JobTrack.Models.Extensions;
using JobTrack.Services.Interfaces;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace JobTrack.Services
{
    public class CoversheetService : ICoversheetService
    {
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        public CoversheetService() { }

        public async Task<List<CoversheetData>> GetCoversheetDataByProductAndServiceAsync(string bpsproductid, string servicenumber, string username)
        {
            var storedProcedure = string.IsNullOrEmpty(username) ? "GetAllCoversheetDataByProductAndService" : "GetAllCoversheetDataByProductAndServiceByUser";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_BPSProductID", bpsproductid);
                command.Parameters.AddWithValue("@p_ServiceNumber", servicenumber);

                if (!string.IsNullOrEmpty(username))
                    command.Parameters.AddWithValue("@p_Username", username);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<CoversheetData>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list);
        }

        public async Task<List<CoversheetData>> GetAllProductAndServiceByUsernameAsync(string userName, UserAccessEnum userAccess)
        {
            var storedProcedure = string.Empty;
            switch (userAccess)
            {
                case UserAccessEnum.Straive_PE:
                    storedProcedure = "GetAllProductAndServiceByUsername";
                    break;

                case UserAccessEnum.Client_LE:
                    storedProcedure = "GetAllProductAndServiceByUserNameLE";
                    break;

                case UserAccessEnum.CodingSTP:
                    storedProcedure = "GetAllProductAndServiceByUsernameSTP";
                    break;
            }
            
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_Username", userName);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<CoversheetData>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list);
        }

        public async Task<CoversheetData> GetCoversheetDataByIdAsync(int id)
        {
            var storedProcedure = "GetCoversheetDataById";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_id", id);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<CoversheetData>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list.FirstOrDefault());
        }

        public async Task<JsonResultModel> InsertCoversheetDataAsync(CoversheetData model, string username)
        {
            var result = new JsonResultModel();
            var storedProcedure = "InsertCoversheet";
            var dataTable = new DataTable();

            try
            {
                dbConnection.Open();

                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_Username", username);
                    command.Parameters.AddWithValue("@p_CoversheetNumber", model.CoversheetNumber);
                    command.Parameters.AddWithValue("@p_BPSProductID", model.BPSProductID);
                    command.Parameters.AddWithValue("@p_ServiceNumber", model.ServiceNumber);
                    command.Parameters.AddWithValue("@p_TaskNumber", model.TaskNumber);
                    command.Parameters.AddWithValue("@p_CoversheetTier", model.CoversheetTier);
                    command.Parameters.AddWithValue("@p_Editor", model.Editor);
                    command.Parameters.AddWithValue("@p_ChargeCode", model.ChargeCode);
                    command.Parameters.AddWithValue("@p_TargetPressDate", model.TargetPressDate);
                    command.Parameters.AddWithValue("@p_TaskType", model.TaskType);
                    command.Parameters.AddWithValue("@p_GuideCard", model.GuideCard);
                    command.Parameters.AddWithValue("@p_LocationOfManuscript", model.LocationOfManuscript);
                    command.Parameters.AddWithValue("@p_UpdateType", model.UpdateType);
                    command.Parameters.AddWithValue("@p_GeneralLegRefCheck", model.GeneralLegRefCheck);
                    command.Parameters.AddWithValue("@p_GeneralTOC", model.GeneralTOC);
                    command.Parameters.AddWithValue("@p_GeneralTOS", model.GeneralTOS);
                    command.Parameters.AddWithValue("@p_GeneralReprints", model.GeneralReprints);
                    command.Parameters.AddWithValue("@p_GeneralFascicleInsertion", model.GeneralFascicleInsertion);
                    command.Parameters.AddWithValue("@p_GeneralGraphicLink", model.GeneralGraphicLink);
                    command.Parameters.AddWithValue("@p_GeneralGraphicEmbed", model.GeneralGraphicEmbed);
                    command.Parameters.AddWithValue("@p_GeneralHandtooling", model.GeneralHandtooling);
                    command.Parameters.AddWithValue("@p_GeneralNonContent", model.GeneralNonContent);
                    command.Parameters.AddWithValue("@p_GeneralSamplePages", model.GeneralSamplePages);
                    command.Parameters.AddWithValue("@p_GeneralComplexTask", model.GeneralComplexTask);
                    command.Parameters.AddWithValue("@p_FurtherInstruction", model.FurtherInstruction);
                    command.Parameters.AddWithValue("@p_CodingDueDate", model.CodingDueDate);
                    command.Parameters.AddWithValue("@p_IsXMLEditing", model.IsXMLEditing);
                    command.Parameters.AddWithValue("@p_OnlineDueDate", model.OnlineDueDate);
                    command.Parameters.AddWithValue("@p_IsOnline", true);
                    command.Parameters.AddWithValue("@p_ManuscriptID", model.ManuscriptID);

                    var reader = command.ExecuteReader();
                    dataTable.Load(reader);
                    reader.Close();
                }                

                dbConnection.Close();

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }

            return await Task.FromResult(result);
        }

        public async Task<JsonResultModel> UpdateCoversheetDataAsync(CoversheetData model, string userName, int userAccess)
        {
            var result = new JsonResultModel();
            var storedProcedure = "UpdateCoversheetData";
            var dataTable = new DataTable();

            try
            {
                dbConnection.Open();

                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_CoversheetID", model.CoversheetID);
                    command.Parameters.AddWithValue("@p_Username", userName);
                    command.Parameters.AddWithValue("@p_UserAccess", userAccess);
                    command.Parameters.AddWithValue("@p_LocationOfManuscript", model.LocationOfManuscript);
                    command.Parameters.AddWithValue("@p_FurtherInstruction", model.FurtherInstruction);
                    command.Parameters.AddWithValue("@p_GuideCard", model.GuideCard);
                    command.Parameters.AddWithValue("@p_GeneralLegRefCheck", model.GeneralLegRefCheck);
                    command.Parameters.AddWithValue("@p_GeneralTOC", model.GeneralTOC);
                    command.Parameters.AddWithValue("@p_GeneralTOS", model.GeneralTOS);
                    command.Parameters.AddWithValue("@p_GeneralReprints", model.GeneralReprints);
                    command.Parameters.AddWithValue("@p_GeneralFascicleInsertion", model.GeneralFascicleInsertion);
                    command.Parameters.AddWithValue("@p_GeneralGraphicLink", model.GeneralGraphicLink);
                    command.Parameters.AddWithValue("@p_GeneralGraphicEmbed", model.GeneralGraphicEmbed);
                    command.Parameters.AddWithValue("@p_GeneralHandtooling", model.GeneralHandtooling);
                    command.Parameters.AddWithValue("@p_GeneralNonContent", model.GeneralNonContent);
                    command.Parameters.AddWithValue("@p_GeneralSamplePages", model.GeneralSamplePages);
                    command.Parameters.AddWithValue("@p_GeneralComplexTask", model.GeneralComplexTask);
                    command.Parameters.AddWithValue("@p_RevisedOnlineDueDate", model.RevisedOnlineDueDate);
                    command.Parameters.AddWithValue("@p_Remarks", model.Remarks);
                    command.Parameters.AddWithValue("@p_IsXMLEditing", model.IsXMLEditing);
                    command.Parameters.AddWithValue("@p_IsOnline", model.IsOnline);
                    command.Parameters.AddWithValue("@p_JobOwner", model.JobOwner);
                    command.Parameters.AddWithValue("@p_JobOwnerID", model.JobOwnerID);
                    command.Parameters.AddWithValue("@p_CompletionEmailTemplate", model.CompletionEmailTemplate);
                    command.Parameters.AddWithValue("@p_CodingStartDate", model.CodingStartDate);
                    command.Parameters.AddWithValue("@p_CodingDoneDate", model.CodingDoneDate);
                    command.Parameters.AddWithValue("@p_PDFQCStartDate", model.PDFQCStartDate);
                    command.Parameters.AddWithValue("@p_PDFQCDoneDate", model.PDFQCDoneDate);
                    command.Parameters.AddWithValue("@p_OnlineStartDate", model.OnlineStartDate);
                    command.Parameters.AddWithValue("@p_OnlineDoneDate", model.OnlineDoneDate);
                    command.Parameters.AddWithValue("@p_AttachmentFile", model.AttachmentFile);

                    var reader = command.ExecuteReader();
                    dataTable.Load(reader);
                    reader.Close();
                }

                dbConnection.Close();

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }

            return await Task.FromResult(result);
        }

        public async Task<JsonResultModel> UpdateSubsequentPassAsync(CoversheetData model, string userName)
        {
            var result = new JsonResultModel();
            var storedProcedure = "UpdateSubsequentPass";
            var dataTable = new DataTable();

            try
            {
                dbConnection.Open();

                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_UserName", userName);
                    command.Parameters.AddWithValue("@p_CoversheetID", model.CoversheetID);
                    command.Parameters.AddWithValue("@p_SubsequentPass", model.SubsequentPass);

                    var reader = command.ExecuteReader();
                    dataTable.Load(reader);
                    reader.Close();
                }

                dbConnection.Close();
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }

            return await Task.FromResult(result);
        }
    }
}