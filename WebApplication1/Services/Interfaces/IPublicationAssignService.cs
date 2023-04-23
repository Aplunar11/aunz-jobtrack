using JobTrack.Models.Employee;
using JobTrack.Models.Enums;
using JobTrack.Models.JobReassignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTrack.Services.Interfaces
{
    public interface IPublicationAssignService
    {
        Task<List<PublicationAssignmentModel>> GetAllPublicationAssignmentModelAsync();

        Task<List<JobReassignmentModel>> GetAllPublicationAssignmentByRole(UserAccessEnum userAccess);
        
        Task<PublicationAssignmentModel> UpdatePublicationAssignmentAsync(PublicationAssignmentModel model);
    }
}
