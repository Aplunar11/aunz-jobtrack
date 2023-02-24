using JobTrack.Models.Enums;
using JobTrack.Models.QueryCoversheet;
using JobTrack.Models.QueryManuscript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTrack.Services.Interfaces
{
    public interface IQueryCoversheetService
    {
        Task<CoversheetDataModel> GetCoversheetDataByIdAsync(int id);

        Task<List<QueryCoversheetModel>> GetQueryCoversheetAsync(int id, FilterEnum filterBy);

        Task<QueryCoversheetModel> UpdateQueryCoversheetAsync(QueryCoversheetModel model);

        Task<bool> UpdateQueryReplyAsync(ReplyModel model);

        Task<bool> DeleteQueryCoversheetAsync(QueryCoversheetModel model);
    }
}
