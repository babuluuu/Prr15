namespace Learning_App.Models.Response
{
    public class ListCourses
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public InstructorInfo Instructor { get; set; }
        public string ImageUrl { get; set; }
        public bool IsEnrolled { get; set; }

        public class InstructorInfo
        {
            public string Name { get; set; }
        }
    }
}