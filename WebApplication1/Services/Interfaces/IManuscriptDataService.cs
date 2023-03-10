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
        Task<List<ManuscriptData>> GetManuscriptDataByIdAsync(string bpsProductid, string serviceNumber);

        Task<JobCoversheetData> GetManuscriptDataByProductAndServiceAsync(JobCoversheetData model);
    }
}
