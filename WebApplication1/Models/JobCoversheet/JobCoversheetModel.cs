using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JobTrack.Models.JobCoversheet
{

    public class JobCoversheetModel
    {
        public List<JobCoversheetData> ListJob { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class JobCoversheetData
    {

        public int JobCoversheetID { get; set; }
        public string ManuscriptID { get; set; }
        [Display(Name = "Coversheet Number")]
        public string CoversheetNumber { get; set; }
        [Display(Name = "Product")]
        public string BPSProductID { get; set; }
        [Display(Name = "Service Number")]
        public string ServiceNumber { get; set; }
        [Display(Name = "Task Number")]
        public string TaskNumber { get; set; }
        public string CoversheetTier { get; set; }
        public string Editor { get; set; }
        [Display(Name = "Charge Code")]
        public string ChargeCode { get; set; }
        [Display(Name = "Target Press Date")]
        public DateTime? TargetPressDate { get; set; }
        [Display(Name = "Actual Press Date")]
        public DateTime? ActualPressDate { get; set; }
        [Display(Name = "Current Task")]
        public string CurrentTask { get; set; }
        [Display(Name = "Status")]
        public string TaskStatus { get; set; }
        [Display(Name = "Task Type")]
        public string TaskType { get; set; }
        [Display(Name = "Guide Cards")]
        public string GuideCard { get; set; }
        [Display(Name = "Further Instructions")]
        public string FurtherInstructions { get; set; }
        [Display(Name = "Update Type")]
        [Required(ErrorMessage = "Please select update type")]
        public string UpdateType { get; set; }
        [Display(Name = "General")]
        public string GeneralData { get; set; }

        [Display(Name = "Special Instruction")]
        public string SpecialInstruction { get; set; }

        [Display(Name = "Accepted Date")]
        public DateTime? AcceptedDate { get; set; }
        [Display(Name = "Job Owner")]
        public string JobOwner { get; set; }
        [Display(Name = "Update Email CC")]
        public string UpdateEmailCC { get; set; }

        [Display(Name = "XML Editing")]
        public bool XMLEditing { get; set; }

        [Display(Name = "Coding - Due Date")]
        public DateTime? CodingDueDate { get; set; }
        [Display(Name = "Coding - Start Date")]
        public DateTime? CodingStartDate { get; set; }
        [Display(Name = "Coding - Done Date")]
        public DateTime? CodingDoneDate { get; set; }

        [Display(Name = "Subsequent Pass")]
        public string SubsequentPass { get; set; }
        [Display(Name = "Subtask")]
        public string SubTask { get; set; }
        [Display(Name = "PDF QC")]
        public string PDFQCStatus { get; set; }
        [Display(Name = "PDF QC - Start Date")]
        public string PDFQCStartDate { get; set; }
        [Display(Name = "PDF QC - Done Date")]
        public string PDFQCDoneDate { get; set; }

        [Display(Name = "Online - Due Date")]
        public DateTime? OnlineDueDate { get; set; }

        [Display(Name = "Online - Start Date")]
        public DateTime? OnlineStartDate { get; set; }
        [Display(Name = "Online - Done Date")]
        public DateTime? OnlineDoneDate { get; set; }
        [Display(Name = "Online - Status")]
        public string OnlineStatus { get; set; }

        [Display(Name = "Attachment/s")]
        public string Attachment { get; set; }

        [Display(Name = "Corrections - Due Date")]
        public DateTime? CorrectionDueDate { get; set; }
        [Display(Name = "Corrections")]
        public string CorrectionData { get; set; }
        [Display(Name = "Online Timeliness")]
        public string OnlineTimeliness { get; set; }

        [Display(Name = "Reason If Late")]
        public string ReasonIfLate { get; set; }

        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        public int CreatedEmployeeID { get; set; }
        public DateTime DateUpdated { get; set; }
        public int UpdateEmployeeID { get; set; }
        //additional columns
        public string LatestTaskNumber { get; set; }
        public string CodingStatus { get; set; }
        public string PDFQAStatus { get; set; }
        public string OnlineStatus1 { get; set; }

        public int RowCount { get; set; }
        public string Response { get; set; }
        public string ErrorMessage { get; set; }

    }
    //public class GetPubSchedTier
    //{
    //    public string PubSchedTier { get; set; }

    //}
    //public class GetPubschedBPSProductID
    //{
    //    public string PubschedBPSProductID { get; set; }
    //    //public string ServiceNumber { get; set; }

    //}
    //public class GetAllPubschedServiceNumber
    //{
    //    public string ServiceNumber { get; set; }

    //}
    //public class GetAllTurnAroundTime
    //{
    //    public int TurnAroundTimeID { get; set; }
    //    public string UpdateType { get; set; }
    //    public string TaskType { get; set; }
    //    public int TATCopyEdit { get; set; }
    //    public int TATCoding { get; set; }
    //    public int TATOnline { get; set; }
    //    public int TATPDFQA { get; set; }
    //    public int BenchMarkDays { get; set; }

    //}

}