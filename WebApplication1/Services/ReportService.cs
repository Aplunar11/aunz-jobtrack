using JobTrack.Models;
using JobTrack.Models.Coversheet;
using JobTrack.Models.Enums;
using JobTrack.Models.Extensions;
using JobTrack.Models.JobCoversheet;
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

        public async Task<List<ManuscriptData>> GetAllManuscriptDataAsync(ManuscriptData model, ReportFilterManuscriptEnum filterBy)
        {
            var storedProcedure = "GetAllManuscriptData";
            var dataTable = new DataTable();

            dbConnection.Open();

            try
            {
                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_filterBy", (int)filterBy);
                    command.Parameters.AddWithValue("@p_BPSProductID", model.BPSProductID);
                    command.Parameters.AddWithValue("@p_ServiceNumber", model.ServiceNumber);
                    command.Parameters.AddWithValue("@p_UpdateType", model.UpdateType);
                    command.Parameters.AddWithValue("@p_CopyEditDueDate", model.CopyEditDueDate);

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

        public async Task<List<CoversheetData>> GetAllCoversheetDataAsync(CoversheetData model, ReportFilterCoversheetEnum filterBy)
        {
            var storedProcedure = "GetAllCoversheetData";
            var dataTable = new DataTable();

            dbConnection.Open();

            try
            {
                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_filterBy", (int)filterBy);
                    command.Parameters.AddWithValue("@p_CodingDueDate", model.CodingDueDate);
                    command.Parameters.AddWithValue("@p_OnlineDueDate", model.OnlineDueDate);
                    command.Parameters.AddWithValue("@p_OnlineTimeliness", model.OnlineTimeliness);

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

            var list = JsonConvert.DeserializeObject<List<CoversheetData>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list);
        }

        public async Task<List<STPDataModel>> GetAllSTPDataAsync(STPDataModel model, ReportFilterSTPEnum filterBy)
        {
            var storedProcedure = "GetAllSTPData";
            var dataTable = new DataTable();

            dbConnection.Open();

            try
            {
                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_filterBy", (int)filterBy);
                    command.Parameters.AddWithValue("@p_BPSProductID", model.BPSProductID);
                    command.Parameters.AddWithValue("@p_ServiceNumber", model.ServiceNumber);
                    command.Parameters.AddWithValue("@p_SendToPrintStatus", model.SendToPrintStatus);

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
            return await Task.FromResult(list);
        }

        public async Task<List<FilterModel>> GetFilterParameterManuscriptAsync()
        {
            return await Task.FromResult(new List<FilterModel>
            {
                new FilterModel { Name = "Select", Value = (int)ReportFilterManuscriptEnum.Select },
                new FilterModel { Name = "Product", Value = (int)ReportFilterManuscriptEnum.BPSProductID },
                new FilterModel { Name = "Service", Value = (int)ReportFilterManuscriptEnum.ServiceNumber },
                new FilterModel { Name = "Update Type", Value = (int)ReportFilterManuscriptEnum.UpdateType },
                new FilterModel { Name = "Copyedit Due Date", Value = (int)ReportFilterManuscriptEnum.CopyEditDueDate },
            });
        }

        public async Task<List<FilterModel>> GetFilterParameterCoversheetAsync()
        {
            return await Task.FromResult(new List<FilterModel>
            {
                new FilterModel { Name = "Select", Value = (int)ReportFilterCoversheetEnum.Select },
                new FilterModel { Name = "Coding Due Date", Value = (int)ReportFilterCoversheetEnum.CodingDueDate },
                new FilterModel { Name = "Online Due Date", Value = (int)ReportFilterCoversheetEnum.OnlineDueDate },
                new FilterModel { Name = "Online Timeliness", Value = (int)ReportFilterCoversheetEnum.OnlineTimeliness }
            });
        }

        public async Task<List<FilterModel>> GetFilterParameterSTPAsync()
        {
            return await Task.FromResult(new List<FilterModel>
            {
                new FilterModel { Name = "Select", Value = (int)ReportFilterSTPEnum.Select },
                new FilterModel { Name = "Product", Value = (int)ReportFilterSTPEnum.BPSProductID },
                new FilterModel { Name = "Service", Value = (int)ReportFilterSTPEnum.ServiceNumber },
                new FilterModel { Name = "Status", Value = (int)ReportFilterSTPEnum.SendToPrintStatus },
            });
        }

        public async Task<List<FilterModel>> GetReportFilterManuscriptAsync(ReportFilterManuscriptEnum filterBy)
        {
            var storedProcedure = "GetReportFilterManuscript";
            var dataTable = new DataTable();

            dbConnection.Open();

            try
            {
                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_filterBy", (int)filterBy);

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

            var list = new List<FilterModel> { new FilterModel { FilterValue = "Select" } };
            var result = JsonConvert.DeserializeObject<List<FilterModel>>(JsonConvert.SerializeObject(dataTable));

            if (filterBy == ReportFilterManuscriptEnum.CopyEditDueDate)
            {
                list.AddRange(result.Select(i => i.ToDateModel()).ToList());
            }
            else
                list.AddRange(result);

            return await Task.FromResult(list);
        }

        public async Task<List<FilterModel>> GetReportFilterCoversheetAsync(ReportFilterCoversheetEnum filterBy)
        {
            var storedProcedure = "GetReportFilterCoversheet";
            var dataTable = new DataTable();

            dbConnection.Open();

            try
            {
                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_filterBy", (int)filterBy);

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

            var list = new List<FilterModel> { new FilterModel { FilterValue = "Select" } };
            var result = JsonConvert.DeserializeObject<List<FilterModel>>(JsonConvert.SerializeObject(dataTable));

            if (filterBy == ReportFilterCoversheetEnum.CodingDueDate || filterBy == ReportFilterCoversheetEnum.OnlineDueDate)
            {
                list.AddRange(result.Select(i => i.ToDateModel()).ToList());
            }
            else
                list.AddRange(result);

            return await Task.FromResult(list);
        }

        public async Task<List<FilterModel>> GetReportFilterSTPAsync(ReportFilterSTPEnum filterBy)
        {
            var storedProcedure = "GetReportFilterSTP";
            var dataTable = new DataTable();

            dbConnection.Open();

            try
            {
                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_filterBy", (int)filterBy);

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

            var list = new List<FilterModel> { new FilterModel { FilterValue = "Select" } };
            var result = JsonConvert.DeserializeObject<List<FilterModel>>(JsonConvert.SerializeObject(dataTable));

            list.AddRange(result);

            return await Task.FromResult(list);
        }
    }
}