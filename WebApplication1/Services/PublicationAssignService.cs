using JobTrack.Models.Employee;
using JobTrack.Models.Enums;
using JobTrack.Models.JobReassignment;
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
    public class PublicationAssignService : IPublicationAssignService
    {
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        public PublicationAssignService() { }

        public async Task<List<PublicationAssignmentModel>> GetAllPublicationAssignmentModelAsync()
        {
            var storedProcedure = "GetAllPublicationAssignment";
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

            var list = JsonConvert.DeserializeObject<List<PublicationAssignmentModel>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list);
        }
    
        public async Task<List<JobReassignmentModel>> GetAllPublicationAssignmentByRole(UserAccessEnum userAccess)
        {
            var storedProcedure = "GetAllPublicationAssignmentByRole";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_UserAccess", (int)userAccess);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<JobReassignmentModel>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list);
        }

        public async Task<PublicationAssignmentModel> UpdatePublicationAssignmentAsync(PublicationAssignmentModel model)
        {
            var storedProcedure = "UpdatePublicationAssignment";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_id", model.PublicationAssignmentID);
                command.Parameters.AddWithValue("@p_BPSProductID", model.BPSProductID);
                command.Parameters.AddWithValue("@p_CompleteNameOfPublication", model.CompleteNameOfPublication);
                command.Parameters.AddWithValue("@p_PublicationTier", model.PublicationTier);
                command.Parameters.AddWithValue("@p_PEName", model.PEName);
                command.Parameters.AddWithValue("@p_PEEmail", model.PEEmail);
                command.Parameters.AddWithValue("@p_PEUserName", model.PEUserName);
                command.Parameters.AddWithValue("@p_PEStatus", model.PEStatus);
                command.Parameters.AddWithValue("@p_LEName", model.LEName);
                command.Parameters.AddWithValue("@p_LEEmail", model.LEEmail);
                command.Parameters.AddWithValue("@p_LEUserName", model.LEUserName);
                command.Parameters.AddWithValue("@p_LEStatus", model.LEStatus);
                command.Parameters.AddWithValue("@p_User", model.User);
                command.Parameters.AddWithValue("@p_CreatedEmployeeID", 1);
                command.Parameters.AddWithValue("@p_UpdateEmployeeID", 1);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<PublicationAssignmentModel>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list.FirstOrDefault());
        }
    }
}