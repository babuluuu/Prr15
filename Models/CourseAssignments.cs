namespace Learning_App.Models.Response;

public class CourseAssignments
{
    public int AssignmentId { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public float OverallScore { get; set; }
    public virtual Course Course { get; set; }
}