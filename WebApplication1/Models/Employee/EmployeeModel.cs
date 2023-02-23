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

namespace JobTrack.Models.Employee
{
    public class EmployeeModel
    {
        // CREATE A LIST VARIABLE
        // LIST <CONTENT AS CLASS>
        public List<EmployeeData> Employees { get; set; }

        public List<EmployeeAccess> EmployeeUserAccessDropdowns { get; set; }

        public List<EmployeeRegister> EmployeeSignUp { get; set; }
    }
    public class EmployeeData
    {
        public int EmployeeID { get; set; }
        public string UserAccess { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserName { get; set; }
        //public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public int isManager { get; set; }
        public int isEditorialContact { get; set; }
        public int isEmailList { get; set; }
        public int isMandatoryRecepient { get; set; }
        public int isShowUser { get; set; }
        public DateTime PasswordUpdateDate { get; set; }
    }

    public class EmployeeAccess
    {
        public int UserAccessID { get; set; }
        public string UserAccess { get; set; }

    }

    public class EmployeeValidate
    {
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
    }

    public class EmployeeRegister
    {
        public int EmployeeID { get; set; }

        [Display(Name = "User Access")]
        [Required(ErrorMessage = "Please select atleast one option")]
        public string UserAccess { get; set; }

        [Display(Prompt = "First Name")]
        [Required(ErrorMessage = "First name required.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last name required.")]
        public string LastName { get; set; }

        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email Address required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string EmailAddress { get; set; }

        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username required.")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Password required.")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}