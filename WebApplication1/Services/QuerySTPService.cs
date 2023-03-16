using JobTrack.Models.Enums;
using JobTrack.Models.QueryManuscript;
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
    public class QuerySTPService : IQuerySTPService
    {
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        public QuerySTPService() { }

        public async Task<List<STPDataModel>> GetAllSTPDataAsync()
        {
            var storedProcedure = "GetAllSTPData";
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

            var list = JsonConvert.DeserializeObject<List<STPDataModel>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list);
        }

        public async Task<STPDataModel> GetSTPDataByIdAsync(int id)
        {
            var storedProcedure = "GetSTPDataById";
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

            var list = JsonConvert.DeserializeObject<List<STPDataModel>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list.FirstOrDefault());
        }

        public async Task<List<QuerySTPModel>> GetQuerySTPAsync(int id, FilterEnum filterBy)
        {
            var storedProcedure = "GetAllQuerySTP";
            var dataTable = new DataTable();

            dbConnection.Open();

            try
            {
                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_stp_id", id);
                    command.Parameters.AddWithValue("@p_filterBy", filterBy);

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

            var list = JsonConvert.DeserializeObject<List<QuerySTPModel>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list);
        }
    
        public async Task<QuerySTPModel> UpdateQuerySTPAsync(QuerySTPModel model)
        {
            var storedProcedure = "UpdateQuerySTP";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_id", model.ID);
                command.Parameters.AddWithValue("@p_stp_id", model.StpID);
                command.Parameters.AddWithValue("@p_stpstatus_id", model.STPStatusID);
                command.Parameters.AddWithValue("@p_stptopic_id", model.STPTopicID);
                command.Parameters.AddWithValue("@p_stptype_id", model.STPTypeID);
                command.Parameters.AddWithValue("@p_filepath", model.FilePath);
                command.Parameters.AddWithValue("@p_user", model.PostedBy);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<QuerySTPModel>>(JsonConvert.SerializeObject(dataTable)).OrderByDescending(p => p.ID).ToList();

            return await Task.FromResult(list.FirstOrDefault());
        }
    
        public async Task<bool> UpdateSTPReplyAsync(ReplyModel model)
        {
            var isSuccess = false;
            var storedProcedure = "UpdateSTPReply";

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

        public async Task<bool> UpdateQuerySTPStatusAsync(ReplyModel model)
        {
            var isSuccess = false;
            var storedProcedure = "UpdateQuerySTPStatus";

            try
            {
                dbConnection.Open();

                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_id", model.QueryID);
                    command.Parameters.AddWithValue("@p_stpstatus_id", model.STPStatusID);

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

        public async Task<List<ReplyModel>> GetSTPRepliesAsync(int queryID)
        {
            var list = new List<ReplyModel>();
            var storedProcedure = "GetAllSTPReplies";
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
    
        public async Task<QuerySTPModel> GetQuerySTPByIdAsync(int id)
        {
            var storedProcedure = "GetQuerySTPById";
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

            var list = JsonConvert.DeserializeObject<List<QuerySTPModel>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list.FirstOrDefault());
        }
    }
}