using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseModel
{
    public class CourseBundle
    {
        public int MinCreditPoints { get; set; }
        public int MaxCreditPoints { get; set; }
        public List<Course> courses { get; set; }

        public CourseBundle()
        {
            courses = new List<Course>();
        }
    }
}
