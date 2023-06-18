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

        public int Evaluate(StudentDemands demands, IEnumerable<string> currentCoursesIds)
        {
            if (Schedule.Count == 0)
            {
                return int.MinValue;
            }
            int count = 0;
            HashSet<int> days = new();
            foreach (Period period in Schedule.Keys)
            {
                if (period.IsPeriodOverlap(demands.UnavailableTimes))
                {
                    count -= 2;
                }
                days.Add(period.Day);
            }
            count += (demands.PreferedNumberOfLearningDays - days.Count());

            count += 10 * (currentCoursesIds.Count() - demands.WantedCourses.Count());

            return count;
        }
    }
}
