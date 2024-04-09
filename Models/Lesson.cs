namespace Learning_App.Models;

public class Lesson
{
    public int LessonId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int CourseId { get; set; }
    public virtual Course Course { get; set; }
    public virtual ICollection<CourseResource> Resources { get; set; }
}