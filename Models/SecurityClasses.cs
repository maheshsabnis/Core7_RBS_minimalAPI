namespace Core7_RBS_minimalAPI.Models
{
    public class RegisterUser
    {
        /// <summary>
        /// UNique EMail
        /// </summary>
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }

    public class LoginUser
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class SecureResponse
    {
        public string? UserName { get; set; }
        public string? Message { get; set; }
        public int StatucCode { get; set; }
        public string? Token { get; set; }
    }
    /// <summary>
    /// Class to create a new Role
    /// </summary>
    public class RoleData
    {
        public string? RoleName { get; set;}
    }
    /// <summary>
    /// Class to assign Role to User
    /// </summary>
    public class UserRole
    {
        public string? UserName { get; set;}
        public string? RoleName { get; set;}
    }

}
