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

        public async Task<bool> IsJobExists(string bpsproductid, string servicenumber)
        {
            var result = await GetJobCoversheetDataByProductAndServiceAsync(new JobCoversheetData
            {
                BPSProductID = bpsproductid,
                ServiceNumber = servicenumber
            });

            return result.JobCoversheetID > 0;
        }
    }
}