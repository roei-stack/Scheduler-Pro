using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseModel
{
    public class SchedulerAlgorithm
    {
        public Dictionary<Course, CourseScheduling> SuperCourses { get; set; }
        public InstitutionInput Input { get; set; }

        public SchedulerAlgorithm(InstitutionInput input)
        {
            Input = input;
            InputParser(input);
            SuperCourses = new Dictionary<Course, CourseScheduling>();
            foreach (var course in input.CourseList)
            {
                SuperCourses.Add(course, new CourseScheduling(course));
            }
        }

        public void Shchedule()
        {
            // fisrt make sure that every course has no overlapping
            foreach (var course in SuperCourses.Keys)
            {
                for (int lecOccu = 0; lecOccu < course.tlo; lecOccu++)
                {
                    for (int lecPart = 0; lecPart < course.LectureParts; lecPart++)
                    {
                        LectureSchedule(SuperCourses[course], lecOccu, lecPart);
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
                    if (student.WantedCourses.Contains(course))
                    {
                        course.UpdateStudentProblems(student);
                    }
                }
                foreach (var major in input.Majors)
                {
                    course.UpdateOverlappingCourses(major);
                }
                course.CleanThisFromOverlappingCourses();
                foreach (var employee in input.Staff)
                {
                    if (employee.IsTeachingCourse(course))
                    {
                        course.UpdateCourseStaff(employee);
                    }
                }
            }
        }

        private void LectureSchedule(CourseScheduling superCourse, int lecOccu, int lecPart)
        {

        }
    }
}
