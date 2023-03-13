using JobTrack.Models;
using JobTrack.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTrack.Services.Interfaces
{
    public interface IJobDashboardService
    {
        Task<int> GetAllMyJobsByProductAndServiceAsync(string bpsProductIds, string serviceNumbers);

        Task<int> GetAllJobsByProductAndServiceAndStatusAsync(string bpsProductIds, string serviceNumbers, CodingStatusEnum codingStatus);

        Task<int> GetAllJobsByProductAndServiceAndDueStatus(string bpsProductIds, string serviceNumbers, CodingStatusEnum codingStatus);
    }
}
