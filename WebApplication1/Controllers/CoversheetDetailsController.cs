using JobTrack.Models;
using JobTrack.Models.CoversheetDetails;
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
    public class CoversheetDetailsController : Controller
    {
        private readonly IJobCoversheetService _jobCoversheetService;

        public CoversheetDetailsController(IJobCoversheetService jobCoversheetService)
        {
            _jobCoversheetService = jobCoversheetService;
        }

        // GET: CoversheetDetails
        public async Task<ActionResult> Index(int id)
        {
            ViewBag.CoversheetID = id;
            var viewModel = await _jobCoversheetService.GetCoversheetDataByIdAsync(id);
            return View(viewModel);
        }

        public async Task<string> GetQueryCoversheet(int coversheetId, int filterBy)
        {
            var result = new JsonResultModel();
            var filter = (FilterEnum)filterBy;

            try
            {
                result.Collection = await _jobCoversheetService.GetQueryCoversheetAsync(coversheetId, filter);
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