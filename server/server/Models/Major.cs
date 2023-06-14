namespace server.Models
{
    public class Major
    {
        public string? MajorName { get; set; }
        // institution input
        // map(year) -> bundles
        public Dictionary<int, List<CourseBundle>> Bundles { get; set; }

        public Major(string? majorName, Dictionary<int, List<CourseBundle>> bundles)
        {
            MajorName = majorName;
            Bundles = bundles;
        }

        public int InWhatYearIncluded(Course course)
        {
            foreach (var year in Bundles.Keys)
            {
                foreach (var bundle in Bundles[year])
                {
                    if (bundle.Courses.Contains(course))
                    {
                        return year;
                    }
                }
            }
            return -1;
        }
    }
}
