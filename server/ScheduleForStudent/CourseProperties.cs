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
        /// map(type {lecture / exercise}) -> map(group number) -> times
        /// </summary>
        public Dictionary<string, Dictionary<int, List<Period>>> Groups;
        /// <summary>
        /// map(type {lecture / exercise}) -> duration of the class
        /// </summary>
        public Dictionary<string, int> Duration { get; set; }

        // lectureDuration = lecturePoints / lectureParts
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

        public CourseProperties(CourseScheduling courseScheduling)
        {
            Name = courseScheduling.Course.CourseName;
            ID = courseScheduling.Course.CourseId;
            Duration = new Dictionary<string, int>
            {
                [Constants.Lecture] = courseScheduling.Course.LecturePoints
                                     / courseScheduling.Course.LectureParts,
                [Constants.Exercise] = courseScheduling.Course.TAPoints
            };
            Groups = new Dictionary<string, Dictionary<int, List<Period>>>();
            Groups[Constants.Lecture] = new Dictionary<int, List<Period>>();
            Groups[Constants.Exercise] = new Dictionary<int, List<Period>>();
            for (int i = 1; i <= courseScheduling.Course.tLo; i++) {
                Groups[Constants.Lecture][i] = courseScheduling.CourseGroups[i].Item2;
            }
            for (int i = courseScheduling.Course.tLo + 1;
                i <= courseScheduling.Course.tLo + courseScheduling.Course.tTAo; i++)
            {
                Groups[Constants.Lecture][i] = courseScheduling.CourseGroups[i].Item2;
            }
        }
        public CourseProperties(Course course)
        {
            Name = course.CourseName;
            ID = course.CourseId;
        }

        /// <returns>the group number if found, otherwise -1</returns>
        public int FindGroupByType(string courseType, Period period)
        {
            if (Groups == null || Duration[courseType] == 0)
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
