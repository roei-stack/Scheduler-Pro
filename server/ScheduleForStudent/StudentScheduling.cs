using CourseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleForStudent
{
    public class StudentScheduling
    {
        /// <summary>
        /// map(time {full hour from Constants}) -> class type, group nubmer, course)
        /// </summary>
        public Dictionary<Period, (string, int, CourseProperties)> Schedule;
    
        public StudentScheduling()
        {
            Schedule = new Dictionary<Period, (string, int, CourseProperties)>();
        }
    }
}
