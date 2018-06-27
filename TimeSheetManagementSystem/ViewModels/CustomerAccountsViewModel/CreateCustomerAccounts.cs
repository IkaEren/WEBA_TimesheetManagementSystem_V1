using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TimeSheetManagementSystem.ViewModels.CustomerAccountsViewModel
{
    public class CreateCustomerAccounts
    {
        [RegularExpression(@"^[a-zA-Z0-9''-'\s]{1,100}$", ErrorMessage = "Please do not enter special characters.")]
        [Required(ErrorMessage = "Please enter a name for the session synopsis.")]
        [Remote(action: "Verify", controller: "CustomerAccounts")]
        public string AccountName { get; set; }
        [Required(ErrorMessage = "Please enter an effective start date.")]
        public DateTime EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        [RegularExpression("([0-9]+)")]
        [Required(ErrorMessage = "Please enter your rate per hour")]
        public decimal RatePerHour { get; set; }
        public string Comments { get; set; }
        public bool isVisible { get; set; }
        public string createdBy { get; set; }
        public string updatedBy { get; set; }
    }
}
