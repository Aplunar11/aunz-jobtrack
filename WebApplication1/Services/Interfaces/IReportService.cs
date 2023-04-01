using JobTrack.Models;
using JobTrack.Models.Coversheet;
using JobTrack.Models.Enums;
using JobTrack.Models.Manuscript;
using JobTrack.Models.QuerySTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTrack.Services.Interfaces
{
    public interface IReportService
    {
        Task<List<ManuscriptData>> GetAllManuscriptDataAsync(ManuscriptData model, ReportFilterManuscriptEnum reportFilterManuscriptEnum);
        Task<List<CoversheetData>> GetAllCoversheetDataAsync(CoversheetData model, ReportFilterCoversheetEnum filterBy);
        Task<List<STPDataModel>> GetAllSTPDataAsync(STPDataModel model, ReportFilterSTPEnum filterBy);
        Task<List<FilterModel>> GetFilterParameterManuscriptAsync();
        Task<List<FilterModel>> GetFilterParameterCoversheetAsync();
        Task<List<FilterModel>> GetFilterParameterSTPAsync();
        Task<List<FilterModel>> GetReportFilterManuscriptAsync(ReportFilterManuscriptEnum reportFilterManuscriptEnum);
        Task<List<FilterModel>> GetReportFilterCoversheetAsync(ReportFilterCoversheetEnum reportFilterCoversheetEnum);
        Task<List<FilterModel>> GetReportFilterSTPAsync(ReportFilterSTPEnum reportFilterSTPEnum);
    }
}
