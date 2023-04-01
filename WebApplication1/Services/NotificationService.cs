using JobTrack.Models;
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
    public class NotificationService : INotificationService
    {
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        public NotificationService () { }

        public async Task<List<NotificationModel>> GetNoticationByUserAsync(string username)
        {
            var storedProcedure = "GetNotificationByUser";
            var dataTable = new DataTable();

            dbConnection.Open();

            try
            {
                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_User", username);

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

            var list = JsonConvert.DeserializeObject<List<NotificationModel>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list);
        }

        public async Task<NotificationModel> UpdateNotificationUserRecordAsync(NotificationModel model)
        {
            var storedProcedure = "UpdateNotificationUserRecord";
            var dataTable = new DataTable();

            dbConnection.Open();

            try
            {
                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_NotificationPostID", model.ID);
                    command.Parameters.AddWithValue("@p_EmployeeID", model.EmployeeID);

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

            var list = JsonConvert.DeserializeObject<List<NotificationModel>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list.FirstOrDefault());
        }
    }
}