﻿using JobTrack.Models;
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
    public class JobDashboardService : IJobDashboardService
    {
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        public JobDashboardService() { }

        public async Task<int> GetAllMyJobsByProductAndServiceAsync(string bpsProductIds, string serviceNumbers)
        {
            var storedProcedure = "GetAllMyJobsByProductAndService";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_BPSProductIDs", bpsProductIds);
                command.Parameters.AddWithValue("@p_ServiceNumbers", serviceNumbers);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<JobCountModel>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list.FirstOrDefault().JobCount);
        }

        public async Task<int> GetAllJobsByProductAndServiceAndStatusAsync(string bpsProductIds, string serviceNumbers, CodingStatusEnum codingStatus)
        {
            var storedProcedure = "GetAllJobsByProductAndServiceAndStatus";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_BPSProductIDs", bpsProductIds);
                command.Parameters.AddWithValue("@p_ServiceNumbers", serviceNumbers);
                command.Parameters.AddWithValue("@p_Status", codingStatus.ToString());

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<JobCountModel>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list.FirstOrDefault().JobCount);
        }

        public async Task<int> GetAllJobsByProductAndServiceAndDueStatus(string bpsProductIds, string serviceNumbers, CodingStatusEnum codingStatus)
        {
            var storedProcedure = "GetAllJobsByProductAndServiceAndDueStatus";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_BPSProductIDs", bpsProductIds);
                command.Parameters.AddWithValue("@p_ServiceNumbers", serviceNumbers);
                command.Parameters.AddWithValue("@p_DueStatus", codingStatus.ToString());

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<JobCountModel>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list.FirstOrDefault().JobCount);
        }
    }
}