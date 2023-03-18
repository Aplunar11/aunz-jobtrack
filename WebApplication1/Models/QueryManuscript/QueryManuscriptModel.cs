using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobTrack.Models.QueryManuscript
{
    public class QueryManuscriptModel
    {
        public int ID { get; set; }

        public int? JobID { get; set; }

        public int? TaskID { get; set; }

        public int QueryStatusID { get; set; }

        public string QueryStatus { get; set; }

        public int QueryTopicID { get; set; }

        public string QueryTopicTitle { get; set; }

        public int QueryTypeID { get; set; }

        public string QueryType { get; set; }

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