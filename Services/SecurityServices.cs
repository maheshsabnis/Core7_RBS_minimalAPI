
using Core7_RBS_minimalAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace Core7_RBS_minimalAPI.Services
{
    public class SecurityServices
    {
        /// <summary>
        /// For Creating and Managing Users
        /// </summary>
        UserManager<IdentityUser> _userManager;
        /// <summary>
        /// Managing USer Logins
        /// </summary>
        SignInManager<IdentityUser> _signInManager;
        /// <summary>
        /// Create an Manage Roles
        /// </summary>
        RoleManager<IdentityRole> _roleManager;
        /// <summary>
        /// USed to read onfiguration from the appsettings.json
        /// </summary>
        IConfiguration _config;

        /// <summary>
        /// Inject THe UserManager and SignInManager in DI Container 
        /// These dependencies will be resolved using 
        /// The 'AddIdentityService<IdentityUser, IdentityRole>();
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        public SecurityServices(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager,
             IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
        }

        public async Task<SecureResponse> RegisterUserAsync(RegisterUser user)
        { 
            SecureResponse response = new SecureResponse();
            if (user == null)
            {
                response.StatucCode = 500;
                response.Message = "User Details are not passed";
            }
            else
            {
                // CHeck if USer Already Exists
                var identityUser = await  _userManager.FindByEmailAsync(user.Email);
                if (identityUser != null)
                {
                    response.StatucCode = 500;
                    response.Message = $"User {user.Email} is already exist";
                }
                else
                {
                    // Create user
                    var newUser = new IdentityUser()
                    {
                        Email = user.Email,
                        UserName = user.Email
                    };
                    // Create a user by hashing password 
                    var result = await _userManager.CreateAsync(newUser,user.Password);
                    if (result.Succeeded)
                    {
                        response.StatucCode = 201;
                        response.Message = $"User {user.Email} is created successfully";
                    }
                    else
                    {
                        response.StatucCode = 500;
                        response.Message = $"Some error occurred while creating the user";
                    }
                   
                }
            }
            return response;
        }
        public async Task<SecureResponse> AuthUser(LoginUser user)
        {
            SecureResponse response = new SecureResponse();
            if (user == null)
            {
                response.StatucCode = 500;
                response.Message = "User login Details are not passed";
            }
            else
            {
                // Check if User Already does not Exists
                var identityUser = await _userManager.FindByEmailAsync(user.Email);
                if (identityUser == null)
                {
                    response.StatucCode = 500;
                    response.Message = $"User {user.Email} is not present";
                }
                else
                {
                    // Autenticate the user
                   
                    // Get the LIst of ROles assigned to user
                    var roles = await _userManager.GetRolesAsync(identityUser);
                    if (roles.Count == 0)
                    {
                        response.StatucCode = 500;
                        response.Message = $"The user {user.Email} does not belong to any role, ad hence the user cannot access the application";
                    }
                    else
                    {
                        // Paramater 1: Login EMail
                        // Parameter 2: PAssword
                        // Parameter 3: Creaing Persistent Cookie on Browser, set it to false for API
                        // Parameter 4: Invalid login attempts will lock the user from login (5 attempts default)
                        var authStatus = await _signInManager.PasswordSignInAsync(user.Email, user.Password, false, lockoutOnFailure: true);

                        if (authStatus.Succeeded)
                        {
                            response.StatucCode = 200;
                            response.Message = $"User {user.Email} Logged in successfuly";
                        }
                        else
                        {
                            response.StatucCode = 500;
                            response.Message = $"Error Occurred for User {user.Email} Login";
                        }

                    }

                   
                }
            }
            return response;
        }


        public async Task<SecureResponse> CreateRoleAsync(RoleData role)
        { 
            SecureResponse response = new SecureResponse();
            if (role == null)
            {
                response.StatucCode = 500;
                response.Message = "Not a valid data";
            }
            else
            {
                // Check is role exist
                var roleInfo = await _roleManager.FindByNameAsync(role.RoleName);
                if (roleInfo != null)
                {
                    response.StatucCode = 500;
                    response.Message = $"Role {role.RoleName} is already exist";
                }
                else
                {
                    var identityRole = new IdentityRole() { Name = role.RoleName, NormalizedName = role.RoleName};
                    // Create a role
                    var result = await _roleManager.CreateAsync(identityRole);
                    if (result.Succeeded)
                    {
                        response.StatucCode = 200;
                        response.Message = $"Role {role.RoleName} is created successfully";
                    }
                    else
                    {
                        response.StatucCode = 500;
                        response.Message = "Error Occurred while creating role";
                    }
                }
            }
            return response;
        }

        public async Task<SecureResponse> AddRoleToUserAsync(UserRole userRole)
        { 
            SecureResponse response = new SecureResponse();
            if (userRole == null)
            {
                response.StatucCode = 500;
                response.Message = "No valid inrmation is available";
            }
            else
            {
                // 1. Check for role
                var role = await _roleManager.FindByNameAsync(userRole.RoleName);
                // 2. Check for user
                var user = await _userManager.FindByEmailAsync(userRole.UserName);

                if (role == null || user == null)
                {
                    response.StatucCode = 500;
                    response.Message = $"Either Role {userRole.RoleName} or User {userRole.UserName} is not available";
                }
                else
                {
                    // assing role to user
                    
                    var result = await _userManager.AddToRoleAsync(user, role.Name);
                    if (result.Succeeded)
                    {
                        response.StatucCode = 200;
                        response.Message = $"The User : {user.Email} is assgned to Role: {role.Name}";
                    }
                    else
                    {
                        response.StatucCode = 500;
                        response.Message = "Some error occurred while processing the user assignment to role request.";
                    }
                }
            }
            return response;
            
        }
    }
}
