using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TimeSheetManagementSystem.Models.AccountDetailViewModels
{
    public class IndexAccountDetails
    {
        public string DayOfWeek { get; set; }
        public string StartTimeInMinutes { get; set; }
        public string EndTimeInMinutes { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dddd, dd MMMM yyyy}")]
        public DateTime EffectiveStartDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dddd, dd MMMM yyyy}")]
        public DateTime? EffectiveEndDate { get; set; }
        public bool IsVisible { get; set; }
        public int AccountDetailId { get; set; }
    }
}
