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

        public async Task<List<ManuscriptData>> GetManuscriptDataByIdAsync(string bpsProductid, string serviceNumber)
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
    }
}