using Learning_App.Models.Response;

namespace Learning_App.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public UserRoleTypeEnum Role { get; set; }
        public virtual ICollection<Course> InstructorCourses { get; set; }
        public virtual ICollection<CourseEnrollment> StudentCourses { get; set; }
    }
}
