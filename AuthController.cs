using Microsoft.AspNetCore.Mvc;
//namespace

namespace FitFusionApi.Controllers
{
    [ApiController]
    [Route("auth")]

    //controller class for authentication
    public class AuthController : ControllerBase
    {
        //login endpoint or login method
        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            // Basic validation logic (in a real app, we'd check against a database)
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email and Password are required.");
            }    
    
            //successful login response
            return Ok("Login successful (demo)");
        }

        [HttpPost("signup")]
        public IActionResult Signup(SignupRequest request) //signup endpoint
        {
            // Basic validation logic (in a real app, we'd check against a database)
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email, Username, and Password are required.");
            }    
    
            //successful signup response
            return Ok("Signup successful (demo)");
        }

        //model for login request
        public class LoginRequest
        {
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
        }
        //model for signup request
        public class SignupRequest
        {
            public string Username { get; set; } = "";
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
        }
    }
}