﻿using JobTrack.Models;
using JobTrack.Models.Coversheet;
using JobTrack.Models.JobCoversheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTrack.Services.Interfaces
{
    public interface IJobCoversheetService
    {
        Task<List<JobCoversheetData>> GetAllJobCoversheetDataAsync();

        Task<List<JobCoversheetData>> GetAllJobCoversheetDataByUserNameLEorPEAsync(string username);

        Task<JobCoversheetData> GetJobCoversheetDataByProductAndServiceAsync(JobCoversheetData model);

        Task<JsonResultModel> InsertJobCoversheetAsync(CoversheetData model, string username);

        Task<JobCoversheetData> IsJobExists(string bpsproductid, string servicenumber);
    }
}
