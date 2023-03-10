using JobTrack.Models.JobCoversheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTrack.Services.Interfaces
{
    public interface IPubSchedService
    {
        Task<JobCoversheetData> GetPubSchedDataByProductAndServiceAsync(JobCoversheetData model);
    }
}
