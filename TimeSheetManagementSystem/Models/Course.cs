﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeSheetManagementSystem.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public String CourseAbbreviation { get; set; }
        public String CourseName { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public int DeletedById { get; set; }

        public UserInfo CreatedBy { get; set; }
        public UserInfo UpdatedBy { get; set; }
        public UserInfo DeletedBy { get; set; }
    }
}
