﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TimeSheetManagementSystem.Models
{
    public class CustomerAccount
    {
        public CustomerAccount()
        {  //Require a constructor to initialize this List property first.  
            this.AccountRates = new List<AccountRate>();
        }

        public int CustomerAccountId { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9''-'\s]{1,100}$", ErrorMessage = "Please do not enter special characters.")]
        [Required(ErrorMessage = "Please enter a name for the session synopsis.")]
        public string AccountName { get; set; }

        //-- The following property, IsCurrent is more useful for product price use cases.
        //-- It is not suitable for timesheet usecase because it is dependant on when the administrator
        //-- logon to generate invoice.
        //IsCurrent property with a value of true describes a customer account which is current (from the perspective of the user)
        //public bool IsCurrent { get; set; } //http://stackoverflow.com/questions/32056999/best-way-to-store-days-of-the-week-with-time-spans

        public string Comments { get; set; }
        public bool IsVisible { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dddd, dd MMMM yyyy}")]
        public DateTime CreatedAt { get; set; }
        public int CreatedById { get; set; }
        public UserInfo CreatedBy { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dddd, dd MMMM yyyy}")]
        public DateTime UpdatedAt { get; set; }
        public int UpdatedById { get; set; }
        public UserInfo UpdatedBy { get; set; }
        public List<InstructorAccount> InstructorAccounts { get; set; }
        public List<AccountDetail> AccountDetails { get; set; }
        public List<AccountRate> AccountRates { get; set; }
    }
}
