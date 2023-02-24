using JobTrack.Models.CoversheetDetails;
using JobTrack.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTrack.Services.Interfaces
{
    public interface IJobCoversheetService
    {
        Task<CoversheetDataModel> GetCoversheetDataByIdAsync(int id);

        Task<List<QueryCoversheetModel>> GetQueryCoversheetAsync(int id, FilterEnum filterBy);
    }
}
