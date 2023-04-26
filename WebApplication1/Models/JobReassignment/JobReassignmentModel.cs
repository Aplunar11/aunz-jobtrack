using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobTrack.Models.JobReassignment
{
    public class JobReassignmentModel
    {
        public int TransactionLogID { get; set; }

        public int TransactionLogIdentity { get; set; }

        public string JobNumber { get; set; }

        public string BPSProductID { get; set; }

        public string ServiceNumber { get; set; }

        public string ValueBefore { get; set; }

        public string ValueAfter { get; set; }

        public string NewOwner { get; set; }

        public string PreviousOwner { get { return ValueBefore; } }

        public string CurrentOwner { get { return ValueAfter; } }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get { return DateCreated; } }
    }
}