using JobTrack.Models;
using JobTrack.Models.Coversheet;
using JobTrack.Models.Enums;
using JobTrack.Models.Extensions;
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
    public class CoversheetService : ICoversheetService
    {
        public MySqlConnection dbConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["SQLConn"].ConnectionString);
        public MySqlCommand cmd = new MySqlCommand();
        public MySqlDataAdapter adp = new MySqlDataAdapter();

        public CoversheetService() { }

        public async Task<List<CoversheetData>> GetCoversheetDataByProductAndServiceAsync(string bpsproductid, string servicenumber)
        {
            var storedProcedure = "GetAllCoversheetDataByProductAndService";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_BPSProductID", bpsproductid);
                command.Parameters.AddWithValue("@p_ServiceNumber", servicenumber);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<CoversheetData>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list);
        }

        public async Task<List<CoversheetData>> GetAllProductAndServiceByUsernameAsync(string userName, UserAccessEnum userAccess)
        {
            var storedProcedure = userAccess == UserAccessEnum.Straive_PE ? "GetAllProductAndServiceByUsername" : "GetAllProductAndServiceByUserNameLE";
            var dataTable = new DataTable();

            dbConnection.Open();

            using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_Username", userName);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            var list = JsonConvert.DeserializeObject<List<CoversheetData>>(JsonConvert.SerializeObject(dataTable));
            return await Task.FromResult(list);
        }

        public async Task<JsonResultModel> InsertCoversheetAsync(CoversheetData model, string username)
        {
            var result = new JsonResultModel();
            var storedProcedure = "InsertCoversheet";
            var dataTable = new DataTable();

            try
            {
                dbConnection.Open();

                using (MySqlCommand command = new MySqlCommand(storedProcedure, dbConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_Username", username);
                    command.Parameters.AddWithValue("@p_CoversheetNumber", model.CoversheetNumber);
                    command.Parameters.AddWithValue("@p_BPSProductID", model.BPSProductID);
                    command.Parameters.AddWithValue("@p_ServiceNumber", model.ServiceNumber);
                    command.Parameters.AddWithValue("@p_TaskNumber", model.TaskNumber);
                    command.Parameters.AddWithValue("@p_CoversheetTier", model.CoversheetTier);
                    command.Parameters.AddWithValue("@p_Editor", model.Editor);
                    command.Parameters.AddWithValue("@p_ChargeCode", model.ChargeCode);
                    command.Parameters.AddWithValue("@p_TargetPressDate", model.TargetPressDate);
                    command.Parameters.AddWithValue("@p_TaskType", model.TaskType);
                    command.Parameters.AddWithValue("@p_GuideCard", model.GuideCard);
                    command.Parameters.AddWithValue("@p_LocationOfManuscript", model.LocationOfManuscript);
                    command.Parameters.AddWithValue("@p_UpdateType", model.UpdateType);
                    command.Parameters.AddWithValue("@p_GeneralLegRefCheck", model.LegRefCheck.ToText());
                    command.Parameters.AddWithValue("@p_GeneralTOC", model.TOC.ToText());
                    command.Parameters.AddWithValue("@p_GeneralTOS", model.TOS.ToText());
                    command.Parameters.AddWithValue("@p_GeneralReprints", model.Reprints.ToText());
                    command.Parameters.AddWithValue("@p_GeneralFascicleInsertion", model.FascicleInsertion.ToText());
                    command.Parameters.AddWithValue("@p_GeneralGraphicLink", model.GraphicLink.ToText());
                    command.Parameters.AddWithValue("@p_GeneralGraphicEmbed", model.GraphicEmbed.ToText());
                    command.Parameters.AddWithValue("@p_GeneralHandtooling", model.Handtooling.ToText());
                    command.Parameters.AddWithValue("@p_GeneralNonContent", model.NonContent.ToText());
                    command.Parameters.AddWithValue("@p_GeneralSamplePages", model.SamplePages.ToText());
                    command.Parameters.AddWithValue("@p_GeneralComplexTask", model.ComplexTask.ToText());
                    command.Parameters.AddWithValue("@p_FurtherInstruction", model.FurtherInstructions);
                    command.Parameters.AddWithValue("@p_CodingDueDate", model.CodingDueDate);
                    command.Parameters.AddWithValue("@p_IsXMLEditing", model.XMLEditing.ToText());
                    command.Parameters.AddWithValue("@p_OnlineDueDate", model.OnlineDueDate);
                    command.Parameters.AddWithValue("@p_IsOnline", true.ToText());
                    command.Parameters.AddWithValue("@p_ManuscriptID", model.ManuscriptID);

                    var reader = command.ExecuteReader();
                    dataTable.Load(reader);
                    reader.Close();
                }                

                dbConnection.Close();

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }

            return await Task.FromResult(result);
        }
    }
}