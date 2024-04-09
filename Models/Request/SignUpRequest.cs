﻿namespace Learning_App.Models.Request
{
    public class SignUpRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserRoleTypeEnum Role { get; set; }
    }
}
