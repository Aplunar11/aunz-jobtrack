using JobTrack.Models;
using JobTrack.Models.JobCoversheet;
using JobTrack.Models.Manuscript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTrack.Services.Interfaces
{
    public interface IManuscriptDataService
    {
        Task<List<ManuscriptData>> GetManuscriptDataByProductAndServiceAsync(string bpsProductid, string serviceNumber);

        Task<JobCoversheetData> GetManuscriptDataByProductAndServiceAsync(JobCoversheetData model);

        Task<ManuscriptData> GetManuscriptDataMaxTurnAroundTimeAsync(JobCoversheetData model, string manuscriptIds);

        Task<ManuscriptData> GetManuscriptByIdAsync(int id);

        Task<JsonResultModel> UpdateManuscriptDataAsync(ManuscriptData model, string userName);
    }
}
