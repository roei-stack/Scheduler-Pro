using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseModel
{
    public interface UniStaff
    {
        public bool IsPeriodAvailable(Period period);
        public bool IsTeachingCourse(Course course);
        public bool IsSomeRole(Course course, string role);
        public void Schedule(Course course, string role, Period period);
    }
}
