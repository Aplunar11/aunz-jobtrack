﻿using System;
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


namespace JobTrack.Models.Admin
{
    public class ProductDatabaseModel
    {
        public List<ProductDatabaseData> ProductDatabase { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ProductDatabaseData
    {
        [Display(Name = "Order Number")]
        public int PubSchedID { get; set; }

        [Display(Name = "SPI?")]
        [Required(ErrorMessage = "Please specify if SPI")]
        public string isSPI { get; set; }
        public int OrderNumber { get; set; }

        [Display(Name = "Month")]
        [Required(ErrorMessage = "Please enter month")]
        public string BudgetPressMonth { get; set; }

        [Display(Name = "Tier")]
        [Required(ErrorMessage = "Please select tier")]
        public string PubSchedTier { get; set; }
        [Display(Name = "Team")]
        [Required(ErrorMessage = "Please select team")]
        public string PubSchedTeam { get; set; }
        [Display(Name = "BPS Product ID")]
        [Required(ErrorMessage = "Please select product")]
        public string BPSProductID { get; set; }
        [Display(Name = "Legal Editor")]
        [Required(ErrorMessage = "Please select legal editor")]
        public string LegalEditor { get; set; }
        [Display(Name = "Charge Type")]
        [Required(ErrorMessage = "Please select charge type")]
        public string ChargeType { get; set; }
        [Display(Name = "Product Charge Code")]
        [Required(ErrorMessage = "Please select product charge code")]
        public string ProductChargeCode { get; set; }
        public string BPSProductIDMaster { get; set; }
        [Display(Name = "BPS Sublist")]
        [Required(ErrorMessage = "Please select bps sublist")]
        public string BPSSublist { get; set; }
        [Display(Name = "Service Update")]
        [Required(ErrorMessage = "Please select service update")]
        public string ServiceUpdate { get; set; }

        [Display(Name = "Last Manuscript Handover Date to SPI")]
        [Required(ErrorMessage = "Please select handover date")]
        public DateTime LastManuscriptHandover { get; set; }

        [Display(Name = "Budget Press Date")]
        [Required(ErrorMessage = "Please select budget press date")]
        public DateTime BudgetPressDate { get; set; }
        public DateTime RevisedPressDate { get; set; }
        public string ReasonForRevisedPressDate { get; set; }

        [Display(Name = "Service Number")]
        [Required(ErrorMessage = "Please select service number")]
        public string ServiceNumber { get; set; }
        public int ForecastPages { get; set; }
        public int ActualPages { get; set; }
        public DateTime DataFromLE { get; set; }
        public DateTime DataFromLEG { get; set; }
        public DateTime DataFromCoding { get; set; }
        public int isReceived { get; set; }
        public int isCompleted { get; set; }
        public int AheadOnTime { get; set; }
        public string WithRevisedPressDate { get; set; }
        public DateTime ActualPressDate { get; set; }
        public string ServiceAndBPSProductID { get; set; }
        public string PubSchedRemarks { get; set; }
        public int RowCount { get; set; }
        public string Response { get; set; }
        public string ErrorMessage { get; set; }
    }
}