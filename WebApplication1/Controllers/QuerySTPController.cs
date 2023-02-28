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
    public class QuerySTPController : Controller
    {
        private readonly IQuerySTPService _querySTPService;

        public QuerySTPController(IQuerySTPService querySTPService)
        {
            _querySTPService = querySTPService;
        }

        public async Task<ActionResult> Index(int id)
        {
            ViewBag.StpID = id;
            var viewModel = await _querySTPService.GetSTPDataByIdAsync(id);
            return View(viewModel);
        }

        public async Task<string> GetQuerySTP(int stpId, int filterBy)
        {
            var result = new JsonResultModel();
            var filter = (FilterEnum)filterBy;

            try
            {
                result.Collection = await _querySTPService.GetQuerySTPAsync(stpId, filter);
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