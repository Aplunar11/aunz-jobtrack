using JobTrack.Models.Enums;
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
        Task<List<JobData>> GetJobdataByUserNameLEorPEAsync(string userName, UserAccessEnum userAccess);

        Task<List<JobData>> GetJobdataByUserAsync(string userName, UserAccessEnum userAccess);

        Task<JobData> UpdateJobData(JobData model, string username);
    }
}
