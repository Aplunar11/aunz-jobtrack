using JobTrack.Models.CoversheetDetails;
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
    public class JobCoversheetService : IJobCoversheetService
    {
        // CONNECTION STRING
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        public JobCoversheetService() { }

        public async Task<CoversheetDataModel> GetCoversheetDataByIdAsync(int id)
        {
            var storedProcedure = "GetCoversheetDataById";
            var dataTable = new DataTable();

            dbConnection.Open();

            try
            {
                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_id", id);

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

            var list = JsonConvert.DeserializeObject<List<CoversheetDataModel>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list.FirstOrDefault());
        }
    
        public async Task<List<QueryCoversheetModel>> GetQueryCoversheetAsync(int id, FilterEnum filterBy)
        {
            var list = new List<QueryCoversheetModel>();
            var storedProcedure = "GetAllQueryCoversheet";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_coversheet_id", id);
                command.Parameters.AddWithValue("@p_filterBy", filterBy);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            list = JsonConvert.DeserializeObject<List<QueryCoversheetModel>>(JsonConvert.SerializeObject(dataTable));

            return await Task.FromResult(list);
        }
    }
}