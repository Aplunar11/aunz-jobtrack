using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobTrack.Models.QueryManuscript
{
    public class ManuscriptDataModel
    {
        public int ManuscriptID { get; set; }

        public int JobNumber { get; set; }

        public string ManuscriptTier { get; set; }

        public string BPSProductID { get; set; }

        public string ServiceNumber { get; set; }

        public string ManuscriptLegTitle { get; set; }

        public string PETaskNumber { get; set; }

        public string JobNumberText { get { return JobNumber.ToString().PadLeft(8, '0'); } }
    }
}