using Learning_App.Models.Response;

namespace Learning_App.Models;

public class Course
{
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }//instructor
        public string ImageUrl { get; set; }
        public virtual User Instructor { get; set; }
        public ICollection<Lesson> Lessons { get; set; }
        public ICollection<CourseAssignments> Assignments { get; set; }
}