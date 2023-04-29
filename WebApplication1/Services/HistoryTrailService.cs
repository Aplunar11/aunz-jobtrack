using JobTrack.Models;
using JobTrack.Models.Enums;
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
    public class HistoryTrailService : IHistoryTrailService
    {
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        public HistoryTrailService() { }

        public async Task<List<HistoryTrailModel>> GetAllHistoryTrailAsync()
        {
            var storedProcedure = "GetAllHistoryTrail";
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

            var list = JsonConvert.DeserializeObject<List<HistoryTrailModel>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list);
        }

        public async Task<List<HistoryTrailModel>> GetAllHistoryTrailByJobAsync(HistoryTrailModel model, JobTypeEnum jobType)
        {
            var storedProcedure = "GetAllHistoryTrailByJob";
            var dataTable = new DataTable();

            try
            {
                dbConnection.Open();

                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_JobID", model.ID);
                    command.Parameters.AddWithValue("@p_JobTypeID", (int)jobType);

                    var reader = command.ExecuteReader();
                    dataTable.Load(reader);
                    reader.Close();
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            var list = JsonConvert.DeserializeObject<List<HistoryTrailModel>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list);
        }
    }
}