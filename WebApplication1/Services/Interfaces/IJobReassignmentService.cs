using JobTrack.Models.Enums;
using JobTrack.Models.JobReassignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTrack.Services.Interfaces
{
    public interface IJobReassignmentService
    {
        Task<List<JobReassignmentModel>> GetJobReassignmentByRole(UserAccessEnum userAccess);

        Task<List<JobReassignmentModel>> GetAllJobReassignment();

        Task<List<JobReassignmentModel>> GetAllJobReassignmentByUser(string userName, UserAccessEnum userAccess);

        Task<bool> UpdateJobReassignment(JobReassignmentModel model, string userName);
    }
}
