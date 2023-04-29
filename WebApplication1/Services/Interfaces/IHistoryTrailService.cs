using JobTrack.Models;
using JobTrack.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTrack.Services.Interfaces
{
    public interface IHistoryTrailService
    {
        Task<List<HistoryTrailModel>> GetAllHistoryTrailAsync();

        Task<List<HistoryTrailModel>> GetAllHistoryTrailByJobAsync(HistoryTrailModel model, JobTypeEnum jobType);
    }
}
