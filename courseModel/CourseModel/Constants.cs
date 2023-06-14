using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseModel
{
    public class Constants
    {
        public static readonly string[] Semesters = { "A", "B", "Summer" };
        public static readonly string LecturerRole = "lecturer";
        public static readonly string TARole = "TA";
        public static readonly string Lecture = "lecture";
        public static readonly string TA = "TA";
        public static readonly string OverlapFail = "Failed to prevent overlapping, you can cry or change the bundles. Your call";
        public static readonly string OverlapSuccess = "Yey";
        public static readonly int MinHour = 9;
        public static readonly int MaxHour = 22;
        public static List<Period> AvailablePeriods;

        static Constants()
        {
            // initiate AvailablePeriods
            AvailablePeriods = new List<Period>();
            foreach (var semester in Semesters)
            {
                // Initialize with one-hour periods for each hour in the week
                for (int day = 1; day <= 6; day++)
                {
                    for (int hour = MinHour; hour < MaxHour; hour++)
                    {
                        AvailablePeriods.Add(new Period(day, semester, hour, hour + 1));
                    }
                }
            }
        }
    }
}
