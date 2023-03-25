using JobTrack.Models.Coversheet;
using JobTrack.Models.Manuscript;
using JobTrack.Models.QuerySTP;
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
    public class ReportService : IReportService
    {
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        public async Task<List<ManuscriptData>> GetAllManuscriptDataAsync()
        {
            var storedProcedure = "GetAllManuscriptData";
            var dataTable = new DataTable();

            dbConnection.Open();

            try
            {
                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;

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

        //public async Task<List<CoversheetData>> GetAllCoversheetDataAsync()
        //{

        //}

        //public async Task<List<STPDataModel>> GetAllSTPDataAsync()
        //{

        //}
    }
}