using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseModel
{
    public class MultipleUniStaff : UniStaff
    {
        public List<UniStaff> UniStaffList { get; set; }
        public List<Course> SharedCourses { get; set; }

        public MultipleUniStaff(List<Course> sharedCourses, List<UniStaff> staff)
        {
            UniStaffList = staff;
            SharedCourses = sharedCourses;
        }

        public bool IsPeriodAvailable(Period period)
        {
            foreach (var staff in UniStaffList)
            {
                if(!staff.IsPeriodAvailable(period))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsTeachingCourse(Course course)
        {
            return SharedCourses.Contains(course);
        }

        public bool IsLecturer(Course course)
        {
            return true;
        }

        public bool IsTA(Course course)
        {
            return false;
        }
    }
}
