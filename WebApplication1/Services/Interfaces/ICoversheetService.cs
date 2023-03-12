using JobTrack.Models.Coversheet;
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

        Task<bool> InsertCoversheetAsync(CoversheetData model, string username);
    }
}
