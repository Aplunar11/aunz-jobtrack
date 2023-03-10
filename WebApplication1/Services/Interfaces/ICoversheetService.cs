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
        Task<List<CoversheetData>> GetCoversheetDataByProductAndServiceAsync(string bpsproductid, string servicenumber);

        Task<List<CoversheetData>> GetAllProductAndServiceByUsernameAsync(string userName, UserAccessEnum userAccess);

        Task<JsonResultModel> InsertCoversheetAsync(CoversheetData model, string username);
    }
}
