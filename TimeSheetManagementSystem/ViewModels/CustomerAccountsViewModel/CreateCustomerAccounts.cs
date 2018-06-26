using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeSheetManagementSystem.ViewModels.CustomerAccountsViewModel
{
    public class CreateCustomerAccounts
    {
        public string AccountName { get; set; }
        public DateTime EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        public decimal RatePerHour { get; set; }
        public string Comments { get; set; }
        public bool isVisible { get; set; }
        public string createdBy { get; set; }
        public string updatedBy { get; set; }
    }
}
