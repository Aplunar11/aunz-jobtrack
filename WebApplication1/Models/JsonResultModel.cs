using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobTrack.Models
{
    public class JsonResultModel
    {
        public object Collection { get; set; }

        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; }
    }
}