using JobTrack.Models.Enums;
using JobTrack.Models.QuerySTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTrack.Services.Interfaces
{
    public interface IQuerySTPService
    {
        Task<List<STPDataModel>> GetAllSTPDataAsync();

        Task<STPDataModel> GetSTPDataByIdAsync(int id);

        Task<List<QuerySTPModel>> GetQuerySTPAsync(int id, FilterEnum filterBy);
    }
}
