using JobTrack.Models;
using JobTrack.Models.Enums;
using JobTrack.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JobTrack.Controllers
{
    public class HistoryTrailController : Controller
    {
        private readonly IHistoryTrailService _historyTrailService;

        public HistoryTrailController(IHistoryTrailService historyTrailService)
        {
            _historyTrailService = historyTrailService;
        }

        public async Task<string> GetHistoryTrailByJob(HistoryTrailModel viewModel)
        {
            var result = new JsonResultModel();

            try
            {
                result.Collection = await _historyTrailService.GetAllHistoryTrailByJobAsync(viewModel, viewModel.JobType);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }

            return JsonConvert.SerializeObject(result);
        }
    }
}