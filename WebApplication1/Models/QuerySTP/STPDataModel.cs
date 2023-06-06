using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobTrack.Models.QuerySTP
{
    public class STPDataModel
    {
        public int? ID { get; set; }

        public string SendToPrintNumber { get; set; }

        public int? OwnerUserID { get; set; }

        public string CoversheetIDs { get; set; }

        public string PathOfInputFiles { get; set; }

        public bool IsConsoHighlight { get; set; }

        public bool IsFilingInstruction { get; set; }

        public bool IsDummyFiling1 { get; set; }

        public bool IsDummyFiling2 { get; set; }

        public bool IsUECJ { get; set; }

        public bool IsPC1PC2 { get; set; }

        public bool IsReadyToPrint { get; set; }

        public bool IsSendingFinalPages { get; set; }

        public bool IsPostBack { get; set; }

        public bool IsUpdateEBinder { get; set; }

        public DateTime? ConsoleHighlightStartDate { get; set; }

        public DateTime? ConsoleHighlightEndDate { get; set; }        

        public DateTime? FilingInstructionStartDate { get; set; }

        public DateTime? FilingInstructionEndDate { get; set; }        

        public DateTime? DummyFiling1StartDate { get; set; }

        public DateTime? DummyFiling1EndDate { get; set; }        

        public DateTime? DummyFiling2StartDate { get; set; }

        public DateTime? DummyFiling2EndDate { get; set; }        

        public DateTime? UECJStartDate { get; set; }

        public DateTime? UECJEndDate { get; set; }        

        public DateTime? PC1PC2StartDate { get; set; }

        public DateTime? PC1PC2EndDate { get; set; }        

        public DateTime? ReadyToPrintStartDate { get; set; }

        public DateTime? ReadyToPrintEndDate { get; set; }

        public string ReadyToPrintEmailTemplate { get; set; }        

        public DateTime? SendingFinalPagesStartDate { get; set; }

        public DateTime? SendingFinalPagesEndDate { get; set; }

        public string SendingFinalPagesEmailTemplate { get; set; }        

        public DateTime? PostBackStartDate { get; set; }

        public DateTime? PostBackEndDate { get; set; }        

        public DateTime? UpdateEBinderStartDate { get; set; }

        public DateTime? UpdateEBinderEndDate { get; set; }

        public string SpecialInstruction { get; set; }

        public string SendToPrintTier { get; set; }

        public string BPSProductID { get; set; }

        public string ServiceNumber { get; set; }

        public string CurrentTask { get; set; }

        public string SendToPrintStatus { get; set; }

        public DateTime? TargetPressDate { get; set; }

        public DateTime? ActualPressDate { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public bool IsCodingSTP { get; set; }
    }
}