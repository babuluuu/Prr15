namespace Learning_App.Models.Response
{
    public class DetailedCourseInfo
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public double Grade { get; set; }
        public double Score { get; set; }
        public List<LessonInfo> Lessons { get; set; }
        public List<AssignmentInfo> Assignments { get; set; }
        
        public class LessonInfo
        {
            public int LessonId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public ResourceInfo Resource { get; set; }
        }
        public class ResourceInfo
        {
            public int ResourceId { get; set; }
            public ResourceTypeEnum Type { get; set; }
            public string Content { get; set; }
        }
        public class AssignmentInfo
        {
            public int AssignmentId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }

            public bool IsSubmitted { get; set; }
        }
    }
}
