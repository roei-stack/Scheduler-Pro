using CourseModel;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using server.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace server.Models
{
    public class InstitutionData
    {
        [JsonIgnore]
        public string? InstitutionName { get; set; }

        // must be initialized
        [Key]
        public String? Username { get; set; }

        public String? StaffFormId { get; set; }

        public String? StudentFormId { get; set; }

        public string? SetupViewModelJson { get; set; }

        public string? ResultJson { get; set; }

        public Dictionary<string, Dictionary<int, ScheduledCourseGroupData>>? GetResults()
        {
            if (ResultJson == null) return null;
            return JsonSerializer.Deserialize<Dictionary<string, Dictionary<int, ScheduledCourseGroupData>>>(ResultJson);
        }

        public void SetResults(Dictionary<string, Dictionary<int, ScheduledCourseGroupData>>? results)
        {
            if (results == null) return;
            this.ResultJson = JsonSerializer.Serialize(results);
        }

        public InstitutionSetupViewModel? FromJson()
        {
            return JsonSerializer.Deserialize<InstitutionSetupViewModel>(SetupViewModelJson);
        }

        public void SetupViewModelJsonFromObject(InstitutionSetupViewModel viewModel)
        {
            this.SetupViewModelJson = JsonSerializer.Serialize(viewModel);
        }

        public static InstitutionInput BuildInputFromViewModel(string institutionName, InstitutionSetupViewModel viewModel)
        {
            if (viewModel == null || viewModel.CourseList == null || viewModel.LoneUniStaffList == null || viewModel.MultipleUniStaffList == null || viewModel.CourseBundlesList == null || viewModel.MajorList == null)
            {
                return null;
            }

            List<Course> courseList = viewModel.CourseList.Select(cvm => new Course
            (
                cvm.Id,
                cvm.Name,
                cvm.Lecture_points,
                cvm.TA_points,
                cvm.Lecture_occurrences,
                cvm.Ta_occurrences,
                cvm.TA_after_lecture,
                cvm.Lecture_parts
            )).ToList();
            List<LoneUniStaff> loneUniStaffList = viewModel.LoneUniStaffList.Select(item => new LoneUniStaff
            (
                item.Id,
                item.Name,
                ConvertCourseRolesOccurances(item.CoursesRolesOccurrences, courseList)
            )).ToList();

            List<MultipleUniStaff> multipleUniStaffList = viewModel.MultipleUniStaffList.Select(item => new MultipleUniStaff
            (
                courseList.Where(c => item.SharedCourses.Contains(c.CourseId)).ToList(),
                loneUniStaffList.Where(s => item.StaffList.Contains(s.ID)).OfType<UniStaff>().ToList()
            )).ToList();
            List<UniStaff> uniStaffList = loneUniStaffList.Cast<UniStaff>().Concat(multipleUniStaffList.Cast<UniStaff>()).ToList();

            List<CourseBundle> courseBundles = viewModel.CourseBundlesList.Select(item => new CourseBundle
            (
                item.Id,
                item.MinCreditPoints,
                item.MaxCreditPoints,
                courseList.Where(c => item.Courses.Contains(c.CourseId)).ToList()
            )).ToList();

            List<Major> majorsList = new List<Major>();

            foreach (var majorVm in viewModel.MajorList)
            {
                var majorName = majorVm.MajorName;
                var bundles = new Dictionary<int, List<CourseBundle>>();
                foreach (var bundleId in majorVm.Bundles)
                {
                    var bundle = courseBundles.FirstOrDefault(b => b.ID == bundleId);
                    var year = viewModel.CourseBundlesList.FirstOrDefault(bvm => bvm.Id == bundleId).Year;

                    if (!bundles.ContainsKey(year))
                    {
                        bundles[year] = new List<CourseBundle>();
                    }

                    bundles[year].Add(bundle);
                }
                var major = new Major(majorName, bundles);
                majorsList.Add(major);
            }

            var input = new InstitutionInput(institutionName, courseList, uniStaffList, null, majorsList);
            return input;
        }

        private static Dictionary<Course, Dictionary<string, int>> ConvertCourseRolesOccurances(Dictionary<string, Dictionary<string, int>> dictionaryByIds, List<Course> courseList)
        {
            Dictionary<Course, Dictionary<string, int>> convertedDictionary = new Dictionary<Course, Dictionary<string, int>>();

            foreach (var kvp in dictionaryByIds)
            {
                string courseId = kvp.Key;
                Dictionary<string, int> courseRoles = kvp.Value;

                Course course = courseList.FirstOrDefault(c => c.CourseId == courseId);
                if (course != null)
                {
                    convertedDictionary.Add(course, courseRoles);
                }
            }

            return convertedDictionary;
        }

        [NotMapped]
        public Dictionary<Course, CourseScheduling>? SuperCourses { get; set; }

        [NotMapped]
        public static Dictionary<string, Dictionary<Course, CourseScheduling>> Results;

        static InstitutionData()
        {
            Results = new Dictionary<string, Dictionary<Course, CourseScheduling>>();
        }
    }
}
