namespace Learning_App.Models.Response;

public class CourseEnrollment
{
    public int CourseEnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public double Grade { get; set; }

    public int? LastCompletedLessonId { get; set; }  
    public virtual User Student { get; set; }
    public virtual Course Course { get; set; }
}