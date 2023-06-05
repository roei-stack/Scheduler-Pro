using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseModel
{
    public class LoneUniStaff : UniStaff
    {
        public string? Role { get; set; }
        public string? Name { get; set; }
        public List<Period> UnavailableTimes { get; set; }
        public List<(Course, int)> CourseOccurrences { get; set; }

        public LoneUniStaff()
        {
            UnavailableTimes = new List<Period>();
            CourseOccurrences = new List<(Course, int)>();
        }

        public override string ToString()
        {
            return $"Role: {Role}\nUnavailable Times: {string.Join(", ", UnavailableTimes)}\nCourse Occurrences: {string.Join(", ", CourseOccurrences)}";
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
