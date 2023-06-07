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
        public List<Course> Courses { get; set; }
        public int Year { get; set; }
        public string? Semester { get; set; }

        public CourseBundle(int year, string semester)
        {
            Courses = new List<Course>();
            Year = year;
            Semester = semester;
        }
    }
}
