using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseModel
{
    public class SchedulerAlgorithm
    {
        public List<CourseScheduling> SuperCourses { get; set; }
        public InstitutionInput Input { get; set; }

        public SchedulerAlgorithm(InstitutionInput input)
        {
            Input = input;
            InputParser(input);
            SuperCourses = new List<CourseScheduling>();
            foreach (var course in input.CourseList)
            {
                SuperCourses.Add(new CourseScheduling(course));
            }
        }

        public void Shchedule()
        {
            foreach (var superCourse in SuperCourses)
            {
                for (int lecOccu = 0; lecOccu < superCourse.Course.LectureOccurrences; lecOccu++)
                {
                    for (int lecPart = 0; lecPart < superCourse.Course.LectureParts; lecPart++)
                    {
                        LectureSchedule(superCourse, lecOccu, lecPart);
                    }
                }
            }
        }

        private void InputParser(InstitutionInput input)
        {
            foreach (var course in input.CourseList)
            {
                foreach (var student in input.Students)
                {
                    course.UpdateStudentProblems(student);
                }
                foreach (var major in input.Majors)
                {
                    course.UpdateOverlappingCourses(major);
                }
            }
        }

        private void LectureSchedule(CourseScheduling superCourse, int lecOccu, int lecPart)
        {

        }
    }
}
