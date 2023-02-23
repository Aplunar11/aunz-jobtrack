using JobTrack.Models.Enums;
using JobTrack.Models.QueryManuscript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTrack.Services.Interfaces
{
    public interface IQueryManuscriptService
    {
        Task<List<QueryManuscriptModel>> GetQueryManuscriptAsync(int jobId, FilterEnum filterBy);

        Task<QueryManuscriptModel> GetQueryManuscriptByIdAsync(int id);

        Task<QueryManuscriptModel> UpdateQueryManuscriptAsync(QueryManuscriptModel model);

        Task<bool> DeleteQueryManuscriptAsync(QueryManuscriptModel model);

        Task<bool> UpdateQueryReplyAsync(ReplyModel model);

        Task<List<ReplyModel>> GetQueryRepliesAsync(int queryID);

        Task<ManuscriptDataModel> GetManuscriptDetailsByIdAsync(int id);

        Task<bool> UpdateQueryManuscriptStatusAsync(ReplyModel model);
    }
}
