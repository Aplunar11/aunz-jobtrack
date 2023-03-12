using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Web.Mvc;
using JobTrack.Models;
using System.ComponentModel.DataAnnotations;


namespace JobTrack.Models.Coversheet
{
    public class CoversheetViewModel
    {
        public List<CoversheetData> ListCoversheet { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class UpdateTypeData
    {
        public int UpdateTypeID { get; set; }
        public string UpdateType { get; set; }
        public string TaskType { get; set; }
        public int CopyEditDays { get; set; }
        public int ProcessDays { get; set; }
        public int OnlineDays { get; set; }
        public int PDFQADays { get; set; }
        public int BenchmarkDays { get; set; }
        public int IsEdit { get; set; }
    }

    public class EditCoversheetViewModelBase
    {
        public CoversheetData model1 { get; set; }
        public SubsequentPassData model2 { get; set; }
    }

    public class EditCoversheetViewModel: EditCoversheetViewModelBase
    {

    }
    public class SubsequentPassData
    {
        public int SubsequentPassID { get; set; }
        public int CoversheetID { get; set; }
        [Display(Name = "Product")]
        public string BPSProductID { get; set; }
        [Display(Name = "Service Number")]
        public string ServiceNumber { get; set; }
        public DateTime? SubsequentPassDueDate { get; set; }
        public DateTime? SubsequentPassStartDate { get; set; }
        public DateTime? SubsequentPassCompletionDate { get; set; }
        [Display(Name = "Completion Email Template")]
        public string AttachmentBody { get; set; }
        [Display(Name = "Attachment[pdf, word, excel, csv, jpeg, png, txt]")]
        public string AttachmentName { get; set; }
        public int AttachmentSize { get; set; }
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        public int CreatedEmployeeID { get; set; }
        public DateTime DateUpdated { get; set; }
        public int UpdateEmployeeID { get; set; }
    }
    public class CoversheetData
    {
        public int CoversheetID { get; set; }
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
        [Display(Name = "Location of Manuscript")]
        public string LocationOfManuscript { get; set; }
        [Display(Name = "Further Instructions")]
        public string FurtherInstructions { get; set; }
        [Display(Name = "Update Type")]
        //[Required(ErrorMessage = "Please select update type")]
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

        //[Display(Name = "Attachment/s")]
        //public string Attachment { get; set; }

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
        public int RowCount { get; set; }
        public string Response { get; set; }
        public string ErrorMessage { get; set; }

        public bool Commentary { get; set; }

        public bool LegAmmendments { get; set; }

        public bool NewLeg { get; set; }

        public bool Predicaments { get; set; }

        public bool LegRefCheck { get; set; }

        public bool TOC { get; set; }

        public bool TOS { get; set; }

        public bool Reprints { get; set; }

        public bool FascicleInsertion { get; set; }

        public bool GraphicLink { get; set; }

        public bool GraphicEmbed { get; set; }

        public bool Handtooling { get; set; }

        public bool NonContent { get; set; }

        public bool SamplePages { get; set; }

        public bool ComplexTask { get; set; }
    }
}