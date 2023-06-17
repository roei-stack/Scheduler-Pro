using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseModel
{
    public class Constants
    {
        public static readonly string[] Semesters = { "a", "b", "summer" };
        public static readonly string LecturerRole = "Lecturer";
        public static readonly string TARole = "TA";
        public static readonly string[] Roles = {LecturerRole, TARole};
        public static readonly string Lecture = "Lecture";
        public static readonly string Exercise = "Exercise";
        public static readonly string OverlapFail = "Failed to prevent overlapping, you can cry or change the bundles. Your call";
        public static readonly string OverlapSuccess = "Yey";
        public static readonly int MinHour = 9;
        public static readonly int MaxHour = 22;
        public static readonly int MaxDay = 5;
        public static List<Period> AvailablePeriods;
        // day, string, start time
        public static Dictionary<(int, string, int), Period> AvailablePeriodsFromPeriod;

        static Constants()
        {
            // initiate AvailablePeriods
            AvailablePeriods = new List<Period>();
            AvailablePeriodsFromPeriod = new Dictionary<(int, string, int), Period>();
            foreach (var semester in Semesters)
            {
                // Initialize with one-hour periods for each hour in the week
                for (int day = 1; day <= MaxDay; day++)
                {
                    for (int hour = MinHour; hour < MaxHour; hour++)
                    {
                        Period period = new(day, semester, hour, hour + 1);
                        AvailablePeriods.Add(period);
                        AvailablePeriodsFromPeriod[(day, semester, hour)] = period;
                    }
                }
            }
        }
    }
}
