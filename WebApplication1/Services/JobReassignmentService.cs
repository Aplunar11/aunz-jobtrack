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
    public class JobReassignmentService : IJobReassignmentService
    {
        // CONNECTION STRING
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        private readonly IPublicationAssignService _publicationAssignService;

        public JobReassignmentService(IPublicationAssignService publicationAssignService)
        {
            _publicationAssignService = publicationAssignService;
        }

        public async Task<List<JobReassignmentModel>> GetJobReassignmentByRole(UserAccessEnum userAccess)
        {
            return await _publicationAssignService.GetAllPublicationAssignmentByRole(userAccess);
        }

        public async Task<List<JobReassignmentModel>> GetAllJobReassignment()
        {
            var storedProcedure = "GetAllJobReassignment";
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

            var list = JsonConvert.DeserializeObject<List<JobReassignmentModel>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list);
        }

        public async Task<List<JobReassignmentModel>> GetAllJobReassignmentByUser(string userName, UserAccessEnum userAccess)
        {
            var storedProcedure = "GetAllJobReassignmentByUser";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_UserName", userName);
                command.Parameters.AddWithValue("@p_UserAccess", (int)userAccess);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<JobReassignmentModel>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list);
        }

        public async Task<bool> UpdateJobReassignment(JobReassignmentModel model, string userName)
        {
            var storedProcedure = "UpdateJobReassignment";
            var isSuccess = false;

            try
            {
                dbConnection.Open();

                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_UserName", userName);
                    command.Parameters.AddWithValue("@p_TransactionLogID", model.TransactionLogID);
                    command.Parameters.AddWithValue("@p_ValueBefore", model.ValueAfter);
                    command.Parameters.AddWithValue("@p_ValueAfter", model.NewOwner);

                    int rowAffected = command.ExecuteNonQuery();
                }

                dbConnection.Close();
                isSuccess = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return await Task.FromResult(isSuccess);
        }
    }
}