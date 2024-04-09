namespace Learning_App.Models.Response
{
    public class ListUserResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public UserRoleTypeEnum Role { get; set; }  
    }
}
