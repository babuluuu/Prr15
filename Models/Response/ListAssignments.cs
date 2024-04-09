namespace Learning_App.Models.Response
{
    public class ListAssignments
    {
        public int StudentCourseAssignmentId { get; set; }
        public int UserId { get; set; }
        public int AssignmentId { get; set; }
        public double Score { get; set; }
        public string FileUrl { get; set; }
        
    }
}
