using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobTrack.Models.QueryCoversheet
{
    public class QueryCoversheetModel
    {
        public int ID { get; set; }

        public int? CoversheetID { get; set; }

        public int? CoverStatusID { get; set; }

        public string CoverStatus { get; set; }

        public int? CoverTopicID { get; set; }

        public string CoverTopicTitle { get; set; }

        public int? CoverTypeID { get; set; }

        public string CoverType { get; set; }

        public DateTime? DatePosted { get; set; }

        public string DateText { get { return DatePosted.HasValue ? DatePosted.Value.ToShortDateString() : null; } }

        public DateTime? ClosedDate { get; set; }

        public string ClosedDateText { get { return ClosedDate.HasValue ? ClosedDate.Value.ToShortDateString() : "TBD"; } }

        public string PostedBy { get; set; }

        public string RepliedBy { get; set; }

        public string Message { get; set; }

        public string FilePath { get; set; }

        public bool IsReplied { get; set; }

        public HttpPostedFileBase FileToUpload { get; set; }
    }
}