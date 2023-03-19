using JobTrack.Models;
using JobTrack.Models.Job;
using JobTrack.Services.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace JobTrack.Services
{
    public class TransactionLogService : ITransactionLogService
    {
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        public TransactionLogService() { }

        public async Task<JsonResultModel> UpdateTransactionLogAsync(JobData model, string userName)
        {
            var result = new JsonResultModel();
            var storedProcedure = "InsertJobTransactionLog";
            var dataTable = new DataTable();

            
            try
            {
                dbConnection.Open();

                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_BPSProductID", model.BPSProductID);
                    command.Parameters.AddWithValue("@p_ServiceNumber", model.ServiceNumber);
                    command.Parameters.AddWithValue("@p_UserName", userName);

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