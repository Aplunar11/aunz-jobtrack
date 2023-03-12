using JobTrack.Models.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTrack.Services.Interfaces
{
    public interface IJobdataService
    {
        Task<List<JobData>> GetJobdataByUserNameLEorPEAsync(string userName);

        Task<JobData> UpdateJobData(JobData model, string username);
    }
}
