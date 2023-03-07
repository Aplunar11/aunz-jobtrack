﻿using JobTrack.Models.Employee;
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

        Task<PublicationAssignmentModel> UpdatePublicationAssignmentAsync(PublicationAssignmentModel model);
    }
}
