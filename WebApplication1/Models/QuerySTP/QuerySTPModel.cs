using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobTrack.Models.QuerySTP
{
    public class QuerySTPModel
    {
        public int ID { get; set; }

        public int? StpID { get; set; }

        public int? STPStatusID { get; set; }

        public string STPStatus { get; set; }

        public int? STPTopicID { get; set; }

        public string STPTopicTitle { get; set; }

        public int? STPTypeID { get; set; }

        public string STPType { get; set; }

        public DateTime? DatePosted { get; set; }

        public string DateText { get { return DatePosted.HasValue ? DatePosted.Value.ToShortDateString() : null; } }

        public string PostedBy { get; set; }

        public string RepliedBy { get; set; }

        public string Message { get; set; }

        public string FilePath { get; set; }

        public HttpPostedFileBase FileToUpload { get; set; }
    }
}