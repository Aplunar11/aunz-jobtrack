using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobTrack.Models.QuerySTP
{
    public class STPDataModel
    {
        public int ID { get; set; }

        public string STPNo { get; set; }

        public int? OwnerUserID { get; set; }

        public string PathOfInputFiles { get; set; }

        public int IsConsoHighlight { get; set; }

        public DateTime? ConsoleHighlightStartDate { get; set; }

        public DateTime? ConsoleHighlightEndDate { get; set; }

        public int IsFilingInstruction { get; set; }

        public DateTime? FilingInstructionStartDate { get; set; }

        public DateTime? FilingInstructionEndDate { get; set; }

        public int IsDummyFiling1 { get; set; }

        public DateTime? DummyFiling1StartDate { get; set; }

        public DateTime? DummyFiling1EndDate { get; set; }

        public int IsDummyFiling2 { get; set; }

        public DateTime? DummyFiling2StartDate { get; set; }

        public DateTime? DummyFiling2EndDate { get; set; }

        public int IsUECJ { get; set; }

        public DateTime? UECJStartDate { get; set; }

        public DateTime? UECJEndDate { get; set; }

        public int IsPC1PC2 { get; set; }

        public DateTime? PC1PC2StartDate { get; set; }

        public DateTime? PC1PC2EndDate { get; set; }

        public int IsReadyToPrint { get; set; }

        public DateTime? ReadyToPrintStartDate { get; set; }

        public DateTime? ReadyToPrintEndDate { get; set; }

        public string ReadyToPrintEmailTemplate { get; set; }

        public int IsSendingFinalPages { get; set; }

        public DateTime? SendingFinalPagesStartDate { get; set; }

        public DateTime? SendingFinalPagesEndDate { get; set; }

        public string SendingFinalPagesEmailTemplate { get; set; }

        public int IsPostBack { get; set; }

        public DateTime? PostBackStartDate { get; set; }

        public DateTime? PostBackEndDate { get; set; }

        public int IsUpdateEBinder { get; set; }

        public DateTime? UpdateEBinderStartDate { get; set; }

        public DateTime? UpdateEBinderEndDate { get; set; }

        public string SpecialInstruction { get; set; }

        public string Tier { get; set; }

        public string Product { get; set; }

        public int ServiceNumber { get; set; }

        public string CurrentTask { get; set; }

        public string SendToPrintStatus { get; set; }

        public DateTime? TargetPressDate { get; set; }

        public DateTime? ActualPressDate { get; set; }
    }
}