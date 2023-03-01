using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobTrack.Models.QueryManuscript
{
    public class ReplyModel
    {
        public int ID { get; set; }

        public int ManuscriptID { get; set; }

        public int CoversheetID { get; set; }

        public int StpID { get; set; }

        public int QueryID { get; set; }

        public string Message { get; set; }

        public DateTime DatePosted { get; set; }

        public string PostedBy { get; set; }

        public int QueryStatusID { get; set; }

        public string QueryTopicTitle { get; set; }

        public string QueryType { get; set; }

        public int CoverStatusID { get; set; }

        public string CoverTopicTitle { get; set; }

        public string CoverType { get; set; }

        public int STPStatusID { get; set; }

        public string STPTopicTitle { get; set; }

        public string STPType { get; set; }

        public List<ReplyModel> Replies { get; set; }

        public ReplyModel()
        {
            Replies = new List<ReplyModel>();
        }
    }
}