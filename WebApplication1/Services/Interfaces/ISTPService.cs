using JobTrack.Models.QuerySTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTrack.Services.Interfaces
{
    public interface ISTPService
    {
        Task<STPDataModel> GetSTPDataByIDAsync(int id);

        Task<int> GetSTPMaxIDAsync();

        Task<STPDataModel> InsertSendToPrintAsync(STPDataModel model, string username);
    }
}
