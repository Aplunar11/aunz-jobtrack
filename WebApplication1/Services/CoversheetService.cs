using JobTrack.Models.Coversheet;
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
            var storedProcedure = "GetCoversheetDataById";
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

        public async Task<bool> InsertCoversheetAsync(CoversheetData model, string username)
        {
            var storedProcedure = "InsertCoversheet";
            var dataTable = new DataTable();

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
                command.Parameters.AddWithValue("@p_GeneralLegRefCheck", ToText(model.LegRefCheck));
                command.Parameters.AddWithValue("@p_GeneralTOC", ToText(model.TOC));
                command.Parameters.AddWithValue("@p_GeneralTOS", ToText(model.TOS));
                command.Parameters.AddWithValue("@p_GeneralReprints", ToText(model.Reprints));
                command.Parameters.AddWithValue("@p_GeneralFascicleInsertion", ToText(model.FascicleInsertion));
                command.Parameters.AddWithValue("@p_GeneralGraphicLink", ToText(model.GraphicLink));
                command.Parameters.AddWithValue("@p_GeneralGraphicEmbed", ToText(model.GraphicEmbed));
                command.Parameters.AddWithValue("@p_GeneralHandtooling", ToText(model.Handtooling));
                command.Parameters.AddWithValue("@p_GeneralNonContent", ToText(model.NonContent));
                command.Parameters.AddWithValue("@p_GeneralSamplePages", ToText(model.SamplePages));
                command.Parameters.AddWithValue("@p_GeneralComplexTask", ToText(model.ComplexTask));
                command.Parameters.AddWithValue("@p_FurtherInstruction", model.FurtherInstructions);
                command.Parameters.AddWithValue("@p_CodingDueDate", model.CodingDueDate);
                command.Parameters.AddWithValue("@p_IsXMLEditing", ToText(model.XMLEditing));
                command.Parameters.AddWithValue("@p_OnlineDueDate", model.OnlineDueDate);
                command.Parameters.AddWithValue("@p_IsOnline", ToText(true));
                command.Parameters.AddWithValue("@p_ManuscriptID", model.ManuscriptID);

                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            dbConnection.Close();

            //var list = JsonConvert.DeserializeObject<List<EmployeeData>>(JsonConvert.SerializeObject(dataTable)).OrderByDescending(p => p.ID).ToList();
            //return await Task.FromResult(list.FirstOrDefault());

            return true;
        }

        private string ToText(bool isCheck)
        {
            return isCheck ? "1" : "0";
        }
    }
}