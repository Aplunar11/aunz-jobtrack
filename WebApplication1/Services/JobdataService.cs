using JobTrack.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MySql.Data.MySqlClient;
using System.Configuration;
using Newtonsoft.Json;
using JobTrack.Models.Job;

namespace JobTrack.Services
{
    public class JobdataService : IJobdataService
    {
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        public JobdataService() { }

        public async Task<List<JobData>> GetJobdataByUserNameLEorPEAsync(string userName)
        {
            var storedProcedure = "GetAllJobDataByUserNameLE";
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

            var list = JsonConvert.DeserializeObject<List<JobData>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list);
        }

        public async Task<JobData> UpdateJobData(JobData model, string username)
        {
            var storedProcedure = "UpdateJobData";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_id", model.JobID);
                command.Parameters.AddWithValue("@p_TargetPressDate", model.TargetPressDate);
                command.Parameters.AddWithValue("@p_ActualPressDate", model.ActualPressDate);
                command.Parameters.AddWithValue("@p_CopyEditStatus", model.CopyEditStatus);
                command.Parameters.AddWithValue("@p_CodingStatus", model.CodingStatus);
                command.Parameters.AddWithValue("@p_OnlineStatus", model.OnlineStatus);
                command.Parameters.AddWithValue("@p_STPStatus", model.STPStatus);
                command.Parameters.AddWithValue("@p_Username", username);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<JobData>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list.FirstOrDefault());
        }
    }
}