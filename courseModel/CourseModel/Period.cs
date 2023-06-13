using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseModel
{
    public class Period
    {
        public int Day { get; set; }
        public string Semester { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }

        public Period(int day, string semester, int startTime, int endTime)
        {
            Day = day;
            Semester = semester;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
