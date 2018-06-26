using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeSheetManagementSystem.ViewModels.CustomerAccountsViewModel
{
    public class IndexCustomerAccounts
    {
        // Used a viewmodel so that my customer account view can be binded to two models which is AccountRate and Customer Account.
        public int customerAccountId { get; set; }
        public string accountName { get; set; }
        public string Comments { get; set; }
        public bool isVisible { get; set; }
        public string createdBy { get; set; }
        public string updatedBy { get; set; }
    }
}
