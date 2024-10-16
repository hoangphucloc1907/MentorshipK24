using Microsoft.AspNetCore.Mvc;
using NewsAggregator.Dto;
using NewsAggregator.Entity;
using NewsAggregator.Repository;
using NewsAggregator.Service;


namespace NewsAggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration), "Connection string cannot be null");
            var userRepository = new UserRepository(connectionString);
            var passwordHasher = new PasswordHasher();
            _userService = new UserService(userRepository, passwordHasher);
        }

        // POST api/<UserController>/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegisterRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Invalid user data.");
            }

            var user = new User
            {
                Email = request.Email,
                Username = request.Username,
                Password = request.Password,
                Role = UserRole.User // Default role
            };

            _userService.InsertUser(user);
            return Ok("User registered successfully.");
        }

        // POST api/<UserController>/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Invalid login data.");
            }

            var userId = _userService.AuthenticateAndGetUserId(request.Email, request.Password);
            if (userId == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(new { UserId = userId });
        }
    }
}
