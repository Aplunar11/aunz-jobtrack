using JobTrack.Models.Employee;
using JobTrack.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTrack.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<EmployeeData>> GetAllEmployeeByAccess(UserAccessEnum userAccess);

        Task<EmployeeData> UpdateEmployeeAsync(EmployeeData model);
    }
}
