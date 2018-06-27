using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TimeSheetManagementSystem.Models
{
    public class AccountRate
    {//https://youtu.be/qqfvw6vN1n4 - Appreciating purpose of AccountRate
        public int AccountRateId { get; set; }
        public int CustomerAccountId { get; set; }
        public CustomerAccount CustomerAccount { get; set; }
        [RegularExpression("([0-9]+)")]
        [Required(ErrorMessage = "Please enter your rate per hour.")]
        public decimal RatePerHour { get; set; }
        [Required(ErrorMessage = "Please enter an effective start date.")]
        public DateTime EffectiveStartDate { get; set; }
        //Let the EffectiveEndDate nullable
        public DateTime? EffectiveEndDate { get; set; }
    }
}
