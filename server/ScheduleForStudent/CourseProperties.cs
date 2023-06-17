using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseModel;

namespace ScheduleForStudent
{
    public class CourseProperties
    {
        public string Name { get; set; }
        public string ID { get; set; }
        /// <summary>
        /// map(type {lecture / TA}) -> map(group number) -> times
        /// </summary>
        public Dictionary<string, Dictionary<int, List<Period>>> Groups;
        /// <summary>
        /// map(type {lecture / TA}) -> duration of the class
        /// </summary>
        public Dictionary<string, int> Duration { get; set; }

        public CourseProperties(string name, string id,
            Dictionary<string, Dictionary<int, List<Period>>> groups,
            int lectureDuration, int exerciseDuration)
        {
            Name = name;
            ID = id;
            Groups = groups;

            Duration = new Dictionary<string, int>
            {
                [Constants.Lecture] = lectureDuration,
                [Constants.Exercise] = exerciseDuration
            };
        }

        /// <returns>the group number if found, otherwise -1</returns>
        public int FindGroupByType(string courseType, Period period)
        {
            if (Groups == null)
            {
                return -1;
            }
            foreach (int groupNumber in Groups[courseType].Keys)
            {
                if (period.IsTherePeriodThatStartsInTheSameTime
                            (Groups[courseType][groupNumber]))
                {
                    return groupNumber;
                }
            }
            return -1;
        }
    }

}
