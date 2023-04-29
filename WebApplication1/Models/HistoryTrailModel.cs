using JobTrack.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobTrack.Models
{
    public class HistoryTrailModel
    {
        public int ID { get; set; }

        public string JobNumber { get; set; }

        public DateTime TransactionDate { get; set; }

        public string Transactions { get; set; }

        public string NewValue { get; set; }

        public DateTime DateCreated { get; set; }

        public int CreatedEmployeeID { get; set; }

        public JobTypeEnum JobType { get; set; }
    }
}