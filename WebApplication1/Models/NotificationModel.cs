using JobTrack.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobTrack.Models
{
    public class NotificationModel
    {
        public int ID { get; set; }

        public int QueryID { get; set; }

        public int QueryReplyID { get; set; }

        public string FirstName { get; set; }

        public int NotificationQueryTypeID { get; set; }

        public string Message { get; set; }

        public string PostedBy { get; set; }

        public int UserAccessID { get; set; }

        public int EmployeeID { get; set; }

        public string JobType { get; set; }

        public bool IsViewed { get; set; }

        public DateTime DatePosted { get; set; }

        public string DatePostedAgo { get { return DatePosted.AsTimeAgo(); } }
    }
}