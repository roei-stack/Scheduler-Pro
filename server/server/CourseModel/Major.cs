using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseModel
{
    public class Major
    {
        public string? MajorName { get; set; }
        // institution input
        // map(year, semester) -> bundle
        public Dictionary<(int, string?), List<CourseBundle>> Bundles { get; set; }

        public Major(string? majorName, Dictionary<(int, string?), List<CourseBundle>> bundles)
        {
            MajorName = majorName;
            Bundles = bundles;
        }

        public bool IsCourseIncluded(Course course)
        {
            var bundleList = Bundles[(course.Year, course.Semester)];
            foreach (var bundle in bundleList)
            {
                if (bundle.Courses.Contains(course))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
