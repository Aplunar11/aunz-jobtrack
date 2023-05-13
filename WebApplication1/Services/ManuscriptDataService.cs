using JobTrack.Models;
using JobTrack.Models.Coversheet;
using JobTrack.Models.JobCoversheet;
using JobTrack.Models.Manuscript;
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
    public class ManuscriptDataService : IManuscriptDataService
    {
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        public ManuscriptDataService() { }

        public async Task<List<ManuscriptData>> GetManuscriptDataByProductAndServiceAsync(string bpsProductid, string serviceNumber)
        {
            var storedProcedure = "GetManuscriptDataByID";
            var dataTable = new DataTable();

            dbConnection.Open();

            try
            {
                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_BPSProductID", bpsProductid);
                    command.Parameters.AddWithValue("@p_ServiceNumber", serviceNumber);

                    var reader = command.ExecuteReader();
                    dataTable.Load(reader);
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<ManuscriptData>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list);
        }

        public async Task<ManuscriptData> GetManuscriptByIdAsync(int id)
        {
            var storedProcedure = "GetManuscriptDataByManuscriptID";
            var dataTable = new DataTable();

            dbConnection.Open();

            try
            {
                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_ManuscriptID", id);

                    var reader = command.ExecuteReader();
                    dataTable.Load(reader);
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<ManuscriptData>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list.FirstOrDefault());
        }

        public async Task<JobCoversheetData> GetManuscriptDataByProductAndServiceAsync(JobCoversheetData model)
        {
            var storedProcedure = "GetManuscriptDataByProductAndService";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_BPSProductID", model.BPSProductID);
                command.Parameters.AddWithValue("@p_ServiceNumber", model.ServiceNumber);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<JobCoversheetData>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list.FirstOrDefault());
        }

        public async Task<ManuscriptData> GetManuscriptDataMaxTurnAroundTimeAsync(JobCoversheetData model, string manuscriptIds)
        {
            var storedProcedure = "GetManuscriptDataMaxTurnAroundTime";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_BPSProductID", model.BPSProductID);
                command.Parameters.AddWithValue("@p_ServiceNumber", model.ServiceNumber);
                command.Parameters.AddWithValue("@p_manuscriptids", manuscriptIds);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<ManuscriptData>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list.FirstOrDefault());
        }

        public async Task<JsonResultModel> UpdateManuscriptDataAsync(ManuscriptData model, string userName)
        {
            var storedProcedure = "UpdateManuscriptData";
            var dataTable = new DataTable();
            var result = new JsonResultModel();

            try
            {
                dbConnection.Open();

                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_UserName", userName);
                    command.Parameters.AddWithValue("@p_ManuscriptID", model.ManuscriptID);
                    command.Parameters.AddWithValue("@p_Remarks", model.Remarks);
                    command.Parameters.AddWithValue("@p_GuideCard", model.PEGuideCard);
                    command.Parameters.AddWithValue("@p_LatupAttribution", model.LatupAttribution);
                    command.Parameters.AddWithValue("@p_EstimatedPages", model.EstimatedPages);
                    command.Parameters.AddWithValue("@p_UpdateType", model.UpdateType);
                    command.Parameters.AddWithValue("@p_RevisedOnlineDueDate", model.RevisedOnlineDueDate);
                    command.Parameters.AddWithValue("@p_CopyEditStartDate", model.CopyEditStartDate);
                    command.Parameters.AddWithValue("@p_CopyEditDoneDate", model.CopyEditDoneDate);
                    command.Parameters.AddWithValue("@p_CopyEditStatus", model.CopyEditDoneDate.HasValue ? "Completed" : "New");
                    command.Parameters.AddWithValue("@p_CodingStartDate", model.CodingStartDate);
                    command.Parameters.AddWithValue("@p_CodingDoneDate", model.CodingDoneDate);
                    command.Parameters.AddWithValue("@p_CodingStatus", model.CodingDoneDate.HasValue ? "Completed" : "New");
                    command.Parameters.AddWithValue("@p_OnlineStartDate", model.OnlineStartDate);
                    command.Parameters.AddWithValue("@p_OnlineDoneDate", model.OnlineDoneDate);
                    command.Parameters.AddWithValue("@p_OnlineStatus", model.OnlineDoneDate.HasValue ? "Completed" : "New");

                    var rowAffected = command.ExecuteNonQuery();
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