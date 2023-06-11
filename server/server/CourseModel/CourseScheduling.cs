﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseModel
{
    public class CourseScheduling
    {
        public Course Course { get; set; }
        // int - for the group ID
        public Dictionary<int, (UniStaff, List<Period>)> CourseGroups { get; set; }

        public CourseScheduling(Course course)
        {
            Course = course;
            CourseGroups = new Dictionary<int, (UniStaff, List<Period>)>();
        }
    }
}
