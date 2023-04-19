using JobTrack.Models;
using JobTrack.Models.Coversheet;
using JobTrack.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTrack.Services.Interfaces
{
    public interface ICoversheetService
    {
        Task<List<CoversheetData>> GetCoversheetDataByProductAndServiceAsync(string bpsproductid, string servicenumber, string username);

        Task<List<CoversheetData>> GetAllProductAndServiceByUsernameAsync(string userName, UserAccessEnum userAccess);

        Task<CoversheetData> GetCoversheetDataByIdAsync(int id);

        Task<JsonResultModel> InsertCoversheetDataAsync(CoversheetData model, string username);

        Task<JsonResultModel> UpdateCoversheetByPE(CoversheetData model, string userName, int userAccess);

        Task UpdateCoversheetByCodingTL(CoversheetData model);

        Task UpdateCoversheetByCoding(CoversheetData model);
    }
}
