using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TimeSheetManagementSystem.Models
{
    public class SessionSynopsis
    {
        public int SessionSynopsisId { get; set; }
        // How to use Regex 
        // https://docs.microsoft.com/en-us/previous-versions/msp-n-p/ff650303(v=pandp.10)#common-regular-expressions
        // Client side verification with model validation. 
        // https://docs.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-2.1
        [RegularExpression(@"^[a-zA-Z0-9''-'\s]{1,100}$",ErrorMessage = "Please do not enter special characters.")]
        [Required(ErrorMessage = "Please enter a name for the session synopsis.")]
        [Remote(action:"Verify",controller: "SessionSynopsis",AdditionalFields = nameof(SessionSynopsisId))]
        public string SessionSynopsisName { get; set; }
        public int CreatedById { get; set; }
        public UserInfo CreatedBy { get; set; }
        public int UpdatedById { get; set; }
        public UserInfo UpdatedBy { get; set; }
        //IsVisible shares similar meaning as IsEnabled in the AccountDetail domain class definition.
        public bool IsVisible { get; set; }
        
    }
}
