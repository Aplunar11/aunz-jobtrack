using JobTrack.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobTrack.Controllers
{
    public class QueryLEController : Controller
    {
        private readonly IQueryLEService _queryService;

        public QueryLEController()
        {

        }

        // GET: QueryLE
        public ActionResult Index()
        {
            return View();
        }
    }
}