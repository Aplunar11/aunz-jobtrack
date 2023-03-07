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
        Task<List<JobData>> GetJobdataByUserNameLEAsync(string userName);
    }
}
