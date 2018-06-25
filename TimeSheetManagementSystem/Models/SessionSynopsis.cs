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
        [Required]
        public string SessionSynopsisName { get; set; }

        public int CreatedById { get; set; }
        public UserInfo CreatedBy { get; set; }
        public int UpdatedById { get; set; }
        public UserInfo UpdatedBy { get; set; }
        //IsVisible shares similar meaning as IsEnabled in the AccountDetail domain class definition.
        public bool IsVisible { get; set; }
        
    }
}
