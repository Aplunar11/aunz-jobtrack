using JobTrack.Models.Coversheet;
using JobTrack.Models.Manuscript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTrack.Services.Interfaces
{
    public interface IReportService
    {
        Task<List<ManuscriptData>> GetAllManuscriptDataAsync();
        Task<List<CoversheetData>> GetAllCoversheetDataAsync();
        Task<List<STPData>> GetAllSTPDataAsync();


    }
}
