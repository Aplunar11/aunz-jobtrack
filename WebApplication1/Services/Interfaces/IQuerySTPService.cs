using JobTrack.Models.Enums;
using JobTrack.Models.QueryManuscript;
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
        Task<List<STPDataModel>> GetAllSTPDataAsync(string userName, UserAccessEnum userAccess);

        Task<STPDataModel> GetSTPDataByIdAsync(int id);

        Task<List<QuerySTPModel>> GetQuerySTPAsync(int id, FilterEnum filterBy);

        Task<QuerySTPModel> UpdateQuerySTPAsync(QuerySTPModel model);

        Task<bool> UpdateSTPReplyAsync(ReplyModel model);

        Task<List<ReplyModel>> GetSTPRepliesAsync(int queryID);

        Task<QuerySTPModel> GetQuerySTPByIdAsync(int id);

        Task<bool> UpdateQuerySTPStatusAsync(ReplyModel model, bool isStatusChanged);
    }
}
