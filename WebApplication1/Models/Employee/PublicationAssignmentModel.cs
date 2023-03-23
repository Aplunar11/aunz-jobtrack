using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobTrack.Models.Employee
{
    public class PublicationAssignmentModel
    {
        public int PublicationAssignmentID { get; set; }

        public string BPSProductID { get; set; }

        public string CompleteNameOfPublication { get; set; }

        public string PublicationTier { get; set; }

        public string PEName { get; set; }

        public string PEEmail { get; set; }

        public string PEUserName { get; set; }

        public string PEStatus { get; set; }
        
        public string LEName { get; set; }

        public string LEEmail { get; set; }

        public string LEUserName { get; set; }

        public string LEStatus { get; set; }

        public DateTime? DateCreated { get; set; }

        public int CreatedEmployeeID { get; set; }

        public DateTime? DateUpdated { get; set; }

        public int UpdateEmployeeID { get; set; }

        public string User { get; set; }
    }
}