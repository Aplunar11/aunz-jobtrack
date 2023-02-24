using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobTrack.Models.QueryCoversheet
{
    public class CoversheetDataModel
    {
        public int CoversheetID { get; set; }

        public string CoversheetTier { get; set; }

        public string CoversheetName { get; set; }

        public string BPSProductID { get; set; }

        public string ServiceNumber { get; set; }

        public string ManuscriptFile { get; set; }

        public string LatupAttribution { get; set; }

        public DateTime DateReceivedFromAuthor { get; set; }

        public DateTime DateEnteredIntoTracker { get; set; }

        public string UpdateType { get; set; }

        public string GuideCard { get; set; }

        public string TaskNumber { get; set; }

        public DateTime RevisedOnlineDueDate { get; set; }

        public string DepositedBy { get; set; }

        public string LEInstructions { get; set; }

        public string PickUpBy { get; set; }

        public DateTime PickUpDate { get; set; }

        public string QABy { get; set; }

        public DateTime QADate { get; set; }

        public DateTime QACompletionDate { get; set; }

        public string QueryLog { get; set; }

        public DateTime QueryForApprovalStartDate { get; set; }

        public DateTime QueryForApprovalEndDate { get; set; }

        public int QueryForApprovalAge { get; set; }

        public string Process { get; set; }

        public DateTime PETargetCompletion { get; set; }

        public DateTime LatupTargetCompletion { get; set; }

        public DateTime EndingDueDate { get; set; }

        public DateTime PEActualCompletion { get; set; }

        public DateTime CodingDueDate { get; set; }

        public DateTime CodingActualCompletion { get; set; }

        public int ActualPages { get; set; }

        public DateTime OnlineDueDate { get; set; }

        public DateTime OnlineActualCompletion { get; set; }

        public DateTime LNRedCheckingActualCompletion { get; set; }

        public int AffectedPages { get; set; }

        public int NoOfMSSFile { get; set; }

        public int ActualTAT { get; set; }

        public string BenchmarkMET { get; set; }

        public string FilePath { get; set; }

        public string PEStatus { get; set; }

        public string TaskType { get; set; }

        public DateTime TaskReadyDate { get; set; }

        public string PDFQA_PE { get; set; }

        public int QMSID { get; set; }

        public string CodingStatus { get; set; }

        public string CoversheetRemarks { get; set; }

        public DateTime DateCreated { get; set; }

        public int CreatedEmployeeID { get; set; }

        public DateTime DateUpdated { get; set; }

        public int UpdateEmployeeID { get; set; }
    }
}