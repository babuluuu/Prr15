namespace Learning_App.Models;

public class CourseResource
{
    public int ResourceId { get; set; }
    public int CourseId { get; set; }
    public ResourceTypeEnum ResourceType { get; set; }
    public string ContentInfo { get; set; }
    public int LessonId { get; set; }
    public virtual Course Course { get; set; }
    public virtual Lesson Lesson { get; set; }
}