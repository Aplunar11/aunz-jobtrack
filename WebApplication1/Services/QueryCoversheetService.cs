using JobTrack.Models.Enums;
using JobTrack.Models.QueryCoversheet;
using JobTrack.Models.QueryManuscript;
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
    public class QueryCoversheetService : IQueryCoversheetService
    {
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        public QueryCoversheetService() { }

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

            var list = JsonConvert.DeserializeObject<List<QueryCoversheetModel>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list);
        }

        public async Task<QueryCoversheetModel> UpdateQueryCoversheetAsync(QueryCoversheetModel model)
        {
            var storedProcedure = "UpdateQueryCoversheet";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_id", model.ID);
                command.Parameters.AddWithValue("@p_coversheet_id", model.CoversheetID);
                command.Parameters.AddWithValue("@p_coverstatus_id", model.CoverStatusID);
                command.Parameters.AddWithValue("@p_covertopic_id", model.CoverTopicID);
                command.Parameters.AddWithValue("@p_covertype_id", model.CoverTypeID);
                command.Parameters.AddWithValue("@p_filepath", model.FilePath);
                command.Parameters.AddWithValue("@p_user", model.PostedBy);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<QueryCoversheetModel>>(JsonConvert.SerializeObject(dataTable)).OrderByDescending(p => p.ID).ToList();
            return await Task.FromResult(list.FirstOrDefault());
        }
    
        public async Task<QueryCoversheetModel> GetQueryCoversheetByIdAsync(int id)
        {
            var storedProcedure = "GetQueryCoversheetById";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_id", id);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<QueryCoversheetModel>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list.FirstOrDefault());
        }

        public async Task<bool> DeleteQueryCoversheetAsync(QueryCoversheetModel model)
        {
            var isSuccess = false;
            var storedProcedure = "DeleteQueryCoversheet";

            try
            {
                dbConnection.Open();

                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_id", model.ID);

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
    
        public async Task<List<ReplyModel>> GetCoverRepliesAsync(int queryID)
        {
            var list = new List<ReplyModel>();
            var storedProcedure = "GetAllCoverReplies";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_query_id", queryID);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            list = JsonConvert.DeserializeObject<List<ReplyModel>>(JsonConvert.SerializeObject(dataTable)).OrderByDescending(p => p.ID).ToList();

            return await Task.FromResult(list);
        }
    
        public async Task<bool> UpdateCoverReplyAsync(ReplyModel model)
        {
            var isSuccess = false;
            var storedProcedure = "UpdateReplyCoversheet";

            try
            {
                dbConnection.Open();

                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_id", model.ID);
                    command.Parameters.AddWithValue("@p_query_id", model.QueryID);
                    command.Parameters.AddWithValue("@p_message", string.IsNullOrEmpty(model.Message) ? string.Empty : model.Message);
                    command.Parameters.AddWithValue("@p_user", model.PostedBy);

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
    
        public async Task<bool> UpdateQueryCoversheetStatusAsync(ReplyModel model)
        {
            var isSuccess = false;
            var storedProcedure = "UpdateQueryCoversheetStatus";

            try
            {
                dbConnection.Open();

                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_id", model.QueryID);
                    command.Parameters.AddWithValue("@p_coversheet_id", model.CoversheetID);
                    command.Parameters.AddWithValue("@p_coverstatus_id", model.CoverStatusID);
                    command.Parameters.AddWithValue("@p_repliedby", model.PostedBy);

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