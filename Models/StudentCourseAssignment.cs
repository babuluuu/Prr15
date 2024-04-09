using Learning_App.Models.Response;

namespace Learning_App.Models;

public class StudentCourseAssignment
{
    public int StudentCourseAssignmentId { get; set; }
    public int UserId { get; set; }
    public int AssignmentId { get; set; }
    public double Score { get; set; }
    public string FileUrl { get; set; }
    public virtual User Student { get; set; }
    public virtual CourseAssignments Assignment { get; set; }
}