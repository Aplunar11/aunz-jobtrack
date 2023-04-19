using JobTrack.Models;
using JobTrack.Models.Coversheet;
using JobTrack.Models.Extensions;
using JobTrack.Models.JobCoversheet;
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
    public class JobCoversheetService : IJobCoversheetService
    {
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        public JobCoversheetService() { }

        public async Task<List<JobCoversheetData>> GetAllJobCoversheetDataAsync()
        {
            var storedProcedure = "GetAllJobCoversheetData";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<JobCoversheetData>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list);
        }

        public async Task<List<JobCoversheetData>> GetAllJobCoversheetDataByUserNameLEorPEAsync(string username)
        {
            var storedProcedure = "GetAllJobCoversheetDataByUserNameLEorPE";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_Username", username);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<JobCoversheetData>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list);
        }

        public async Task<JobCoversheetData> GetJobCoversheetDataByProductAndServiceAsync(JobCoversheetData model)
        {
            var storedProcedure = "GetJobCoversheetDataByProductAndService";
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

        public async Task<JsonResultModel> InsertJobCoversheetAsync(CoversheetData model, string username)
        {
            var result = new JsonResultModel();
            var storedProcedure = "InsertJobCoversheet";
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
                    command.Parameters.AddWithValue("@p_IsOnline", model.IsOnline);
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

        public async Task<JobCoversheetData> IsJobExists(string bpsproductid, string servicenumber)
        {
            //var result = await GetJobCoversheetDataByProductAndServiceAsync(new JobCoversheetData
            //{
            //    BPSProductID = bpsproductid,
            //    ServiceNumber = servicenumber
            //});

            var result = await GetAllJobCoversheetDataAsync();
            return result.FirstOrDefault(x => x.BPSProductID == bpsproductid && x.ServiceNumber == servicenumber);
        }
    }
}