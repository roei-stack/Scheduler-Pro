using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseModel
{
    public class LoneUniStaff : UniStaff
    {
        public string? Name { get; set; }
        public string? ID { get; set; }
        // staff's input
        public List<Period> UnavailableTimes { get; set; }
        // institution's input
        // map(course) -> [course ccurrences, role]
        public Dictionary<Course, (int, string)> CoursesOccurrencesRoles { get; set; }

        public LoneUniStaff()
        {
            UnavailableTimes = new List<Period>();
            CoursesOccurrencesRoles = new Dictionary<Course, (int, string)>();
        }


        public bool IsPeriodAvailable(Period period)
        {
            foreach (var unavailablePeriod in UnavailableTimes)
            {
                // Check if the periods overlap
                if (period.Day == unavailablePeriod.Day &&
                   (period.StartTime < unavailablePeriod.EndTime &&
                    period.EndTime > unavailablePeriod.StartTime))
                {
                    // Overlapping periods found
                    return false;
                }
            }
            // No overlapping periods found
            return true;
        }
    }
}
