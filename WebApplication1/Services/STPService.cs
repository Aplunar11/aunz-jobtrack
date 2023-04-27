using JobTrack.Models.Extensions;
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
    public class STPService : ISTPService
    {
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        public STPService() { }

        public async Task<STPDataModel> GetSTPDataByIDAsync(int id)
        {
            var sp = "GetSTPDataById";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(sp, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_id", id);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<STPDataModel>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list.FirstOrDefault());
        }

        public async Task<int> GetSTPMaxIDAsync()
        {
            var sp = "GetSTPMaxID";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(sp, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<STPDataModel>>(JsonConvert.SerializeObject(dataTable));

            return await Task.FromResult(list.Any() && list.FirstOrDefault().ID != null 
                ? list.FirstOrDefault().ID.Value
                : 0);
        }

        public async Task<STPDataModel> InsertSendToPrintAsync(STPDataModel model, string username)
        {
            var storedProcedure = model.ID > 0 ? "UpdateSendToPrint" : "InsertSendToPrint";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_SendToPrintNumber", model.SendToPrintNumber);
                command.Parameters.AddWithValue("@p_BPSProductID", model.BPSProductID);
                command.Parameters.AddWithValue("@p_ServiceNumber", model.ServiceNumber);
                command.Parameters.AddWithValue("@p_SendToPrintTier", model.SendToPrintTier);
                command.Parameters.AddWithValue("@p_TargetPressDate", model.TargetPressDate);
                command.Parameters.AddWithValue("@p_PathOfInputFiles", model.PathOfInputFiles);
                command.Parameters.AddWithValue("@p_SpecialInstruction", model.SpecialInstruction);
                command.Parameters.AddWithValue("@p_IsConsoHighlight", model.IsConsoHighlight.ToInt());
                command.Parameters.AddWithValue("@p_IsFilingInstruction", model.IsFilingInstruction.ToInt());
                command.Parameters.AddWithValue("@p_IsDummyFiling1", model.IsDummyFiling1.ToInt());
                command.Parameters.AddWithValue("@p_IsDummyFiling2", model.IsDummyFiling2.ToInt());
                command.Parameters.AddWithValue("@p_IsUECJ", model.IsUECJ.ToInt());
                command.Parameters.AddWithValue("@p_IsPC1PC2", model.IsPC1PC2.ToInt());
                command.Parameters.AddWithValue("@p_IsReadyToPrint", model.IsReadyToPrint.ToInt());
                command.Parameters.AddWithValue("@p_IsSendingFinalPages", model.IsSendingFinalPages.ToInt());
                command.Parameters.AddWithValue("@p_IsPostBack", model.IsPostBack.ToInt());
                command.Parameters.AddWithValue("@p_IsUpdateEBinder", model.IsUpdateEBinder.ToInt());
                command.Parameters.AddWithValue("@p_CoversheetIDs", model.CoversheetIDs);
                command.Parameters.AddWithValue("@p_SendToPrintStatus", model.SendToPrintStatus);
                command.Parameters.AddWithValue("@p_Username", username);

                command.Parameters.AddWithValue("@p_ConsoleHighlightStartDate", model.ConsoleHighlightStartDate);
                command.Parameters.AddWithValue("@p_ConsoleHighlightEndDate", model.ConsoleHighlightEndDate);
                command.Parameters.AddWithValue("@p_FilingInstructionStartDate", model.FilingInstructionStartDate);
                command.Parameters.AddWithValue("@p_FilingInstructionEndDate", model.FilingInstructionEndDate);
                command.Parameters.AddWithValue("@p_DummyFiling1StartDate", model.DummyFiling1StartDate);
                command.Parameters.AddWithValue("@p_DummyFiling1EndDate", model.DummyFiling1EndDate);
                command.Parameters.AddWithValue("@p_DummyFiling2StartDate", model.DummyFiling2StartDate);
                command.Parameters.AddWithValue("@p_DummyFiling2EndDate", model.DummyFiling2EndDate);
                command.Parameters.AddWithValue("@p_UECJStartDate", model.UECJStartDate);
                command.Parameters.AddWithValue("@p_UECJEndDate", model.UECJEndDate);
                command.Parameters.AddWithValue("@p_PC1PC2StartDate", model.PC1PC2StartDate);
                command.Parameters.AddWithValue("@p_PC1PC2EndDate", model.PC1PC2EndDate);
                command.Parameters.AddWithValue("@p_PostBackStartDate", model.PostBackStartDate);
                command.Parameters.AddWithValue("@p_PostBackEndDate", model.PostBackEndDate);
                command.Parameters.AddWithValue("@p_UpdateEBinderStartDate", model.UpdateEBinderStartDate);
                command.Parameters.AddWithValue("@p_UpdateEBinderEndDate", model.UpdateEBinderEndDate);

                if (model.ID > 0)
                {
                    command.Parameters.AddWithValue("@p_ID", model.ID);
                    command.Parameters.AddWithValue("@p_JobOwnerID", model.OwnerUserID);
                }

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<STPDataModel>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list.FirstOrDefault());
        }
    }
}