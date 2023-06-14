namespace server.ViewModels
{
    public class InstitutionSetupViewModel
    {
        public List<CourseViewModel>? CourseList { get; set; }

        public List<LoneUniStaffViewModel>? LoneUniStaffList { get; set; }

        public List<MultipleUniStaffViewModel>? MultipleUniStaffList { get; set; }

        public List<CourseBundlesViewModel>? CourseBundlesList { get; set; }

        public List<MajorViewModel>? MajorList { get; set; }
    }
}
