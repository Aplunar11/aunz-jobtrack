using JobTrack.Models.Employee;
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
    public class EmployeeService : IEmployeeService
    {
        // CONNECTION STRING
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        public EmployeeService() { }

        public async Task<EmployeeData> UpdateEmployeeAsync(EmployeeData model)
        {
            var storedProcedure = "UpdateEmployee";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_ID", model.ID);
                command.Parameters.AddWithValue("@p_UserAccessID", model.UserAccess);
                command.Parameters.AddWithValue("@p_UserName", model.UserName);
                command.Parameters.AddWithValue("@p_Password", model.UserName);
                command.Parameters.AddWithValue("@p_FirstName", model.FirstName);
                command.Parameters.AddWithValue("@p_LastName", model.LastName);
                command.Parameters.AddWithValue("@p_EmailAddress", model.EmailAddress);
                command.Parameters.AddWithValue("@p_IsManager", model.isManager);
                command.Parameters.AddWithValue("@p_IsEditorialContact", model.isEditorialContact);
                command.Parameters.AddWithValue("@p_IsEmailList", model.isEmailList);
                command.Parameters.AddWithValue("@p_IsMandatoryRecepient", model.isMandatoryRecepient);
                command.Parameters.AddWithValue("@p_IsShowUser", model.isShowUser);
                command.Parameters.AddWithValue("@p_User", model.User);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<EmployeeData>>(JsonConvert.SerializeObject(dataTable)).OrderByDescending(p => p.ID).ToList();
            return await Task.FromResult(list.FirstOrDefault());
        }
    }
}