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

        public bool IsPeriodOverlap(IEnumerable<Period> periods)
        {
            foreach (var period in periods)
            {
                // Check if the periods overlap
                if (Day == period.Day &&
                    Semester == period.Semester &&
                    StartTime < period.EndTime &&
                    EndTime > period.StartTime)
                {
                    // Overlapping periods found
                    return true;
                }
            }
            // No overlapping periods found
            return false;
        }

        public bool IsTheDayEnds()
        {
            return (EndTime > Constants.MaxHour);
        }
    }
}
