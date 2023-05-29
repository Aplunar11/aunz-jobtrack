using JobTrack.Models.Enums;
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
    public class QueryManuscriptService : IQueryManuscriptService
    {
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        public QueryManuscriptService()
        {

        }

        public async Task<List<QueryManuscriptModel>> GetQueryManuscriptAsync(int jobId, FilterEnum filterBy)
        {
            var list = new List<QueryManuscriptModel>();
            var storedProcedure = "GetAllQueryManuscript";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_id", jobId);
                command.Parameters.AddWithValue("@p_filterBy", (int)filterBy);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            list = JsonConvert.DeserializeObject<List<QueryManuscriptModel>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list);
        }

        public async Task<QueryManuscriptModel> GetQueryManuscriptByIdAsync(int id)
        {
            var list = new List<QueryManuscriptModel>();
            var storedProcedure = "GetQueryManuscriptById";

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_id", id);

                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        list.Add(new QueryManuscriptModel
                        {
                            ID = int.Parse(reader[0].ToString()),
                            JobID = int.Parse(reader[1].ToString()),
                            //TaskID = null,
                            QueryStatusID = int.Parse(reader[2].ToString()),
                            QueryStatus = reader[3].ToString(),
                            QueryTopicID = int.Parse(reader[4].ToString()),
                            QueryTopicTitle = reader[5].ToString(),
                            QueryTypeID = Convert.ToInt32(reader[6]),
                            QueryType = reader[7].ToString(),
                            DatePosted = Convert.ToDateTime(reader[8]),
                            PostedBy = reader[9].ToString()
                        });
                    }
                }

                reader.Close();
            }

            dbConnection.Close();

            return await Task.FromResult(list.FirstOrDefault());
        }

        public async Task<QueryManuscriptModel> UpdateQueryManuscriptAsync(QueryManuscriptModel model)
        {
            var list = new List<QueryManuscriptModel>();
            var storedProcedure = "UpdateQueryManuscript";
            var dataTable = new DataTable();

            try
            {
                dbConnection.Open();

                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_id", model.ID);
                    command.Parameters.AddWithValue("@p_job_id", model.JobID);
                    command.Parameters.AddWithValue("@p_querystatus_id", model.QueryStatusID);
                    command.Parameters.AddWithValue("@p_querytopic_id", model.QueryTopicID);
                    command.Parameters.AddWithValue("@p_querytype_id", model.QueryTypeID);
                    command.Parameters.AddWithValue("@p_filepath", model.FilePath);
                    command.Parameters.AddWithValue("@p_user", model.PostedBy);

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

            list = JsonConvert.DeserializeObject<List<QueryManuscriptModel>>(JsonConvert.SerializeObject(dataTable)).OrderByDescending(p => p.ID).ToList();

            return await Task.FromResult(list.FirstOrDefault());
        }
    
        public async Task<bool> DeleteQueryManuscriptAsync(QueryManuscriptModel model)
        {
            var isSuccess = false;
            var storedProcedure = "DeleteQueryManuscript";

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

        public async Task<bool> UpdateQueryReplyAsync(ReplyModel model)
        {
            var isSuccess = false;
            var storedProcedure = "UpdateReplyManuscript";

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
                    command.Parameters.AddWithValue("@p_dateposted", DateTime.Now);

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

        public async Task<List<ReplyModel>> GetQueryRepliesAsync(int queryID)
        {
            var list = new List<ReplyModel>();
            var storedProcedure = "GetAllQueryReplies";

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

        public async Task<ManuscriptDataModel> GetManuscriptDetailsByIdAsync(int id)
        {
            var list = new List<ManuscriptDataModel>();
            var storedProcedure = "GetManuscriptDetailsById";

            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_manuscript_id", id);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            list = JsonConvert.DeserializeObject<List<ManuscriptDataModel>>(JsonConvert.SerializeObject(dataTable));

            return await Task.FromResult(list.FirstOrDefault());
        }

        public async Task<bool> UpdateQueryManuscriptStatusAsync(ReplyModel model, bool isStatusChanged)
        {
            var isSuccess = false;
            var storedProcedure = "UpdateQueryManuscriptStatus";

            try
            {
                dbConnection.Open();

                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_id", model.QueryID);
                    command.Parameters.AddWithValue("@p_job_id", model.ManuscriptID);
                    command.Parameters.AddWithValue("@p_querystatus_id", model.QueryStatusID);
                    command.Parameters.AddWithValue("@p_repliedby", model.PostedBy);
                    command.Parameters.AddWithValue("@p_is_statuschanged", isStatusChanged);
                    command.Parameters.AddWithValue("@p_close_date", DateTime.Now);

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