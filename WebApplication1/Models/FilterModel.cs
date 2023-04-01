using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobTrack.Models
{
    public class FilterModel
    {
        public object FilterValue { get; set; }

        public string Name { get; set; }

        public int Value { get; set; }
    }
}