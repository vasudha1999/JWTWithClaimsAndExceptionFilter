using JWTExample.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTExample.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[ExceptioResultFilter]
	public class LoginController : ControllerBase
	{
		private IConfiguration _config;
		public LoginController(IConfiguration config)
		{
			_config = config;
		}
		[HttpGet]
		[Route("Index")]
		//[ExceptioResultFilter]
		public IActionResult Index()
		{
			int[] a = { 1, 2, 3 };
			int index = 3;
			if (index >= a.Length)
				throw new IndexOutOfRangeException();
			else
				return Content(a[index].ToString());
			
		}
		[HttpGet]
		[Authorize]
		public ActionResult<IEnumerable<string>> get()
		{
			var currentUser = HttpContext.User;
		
			var usernameClaim = currentUser.FindFirst("Emails");
			if (usernameClaim != null)
			{
				string email = usernameClaim.Value;
				Console.WriteLine($"Email: {email}");
			}
			else
				throw new NullReferenceException();


			return new string[] { "value1", "value2", "value3", "value4", "value5" };
		}
		[AllowAnonymous]
		[HttpPost]
		public IActionResult Login([FromBody] UserModel login)
		{
			IActionResult response = Unauthorized();
			var user = AuthenticateUser(login);

			if (user != null)
			{
				var tokenString = GenerateJSONWebToken(user);
				response = Ok(new { token = tokenString });
			}

			return response;
		}
		//private string GenerateJSONWebToken(UserModel userInfo)
		//{
		//	var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
		//	var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

		//	var token = new JwtSecurityToken(_config["Jwt:Issuer"],
		//	  _config["Jwt:Issuer"],
		//	  null,
		//	  expires: DateTime.Now.AddMinutes(120),
		//	  signingCredentials: credentials);

		//	return new JwtSecurityTokenHandler().WriteToken(token);
		//}
		private string GenerateJSONWebToken(UserModel userInfo)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var claims = new[] 
			{
				new Claim("Username", userInfo.Username),
				new Claim("Email", userInfo.EmailAddress),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			var token = new JwtSecurityToken(_config["Jwt:Issuer"],
				_config["Jwt:Issuer"],
				claims,
				expires: DateTime.Now.AddMinutes(120),
				signingCredentials: credentials);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
		private UserModel AuthenticateUser(UserModel login)
		{
			UserModel user = null;
			if (login.Username == "Jignesh")
			{
				user = new UserModel { Username = "Jignesh11", EmailAddress = "test.btest@gmail.com" };
			}
			return user;
		}
	

	}
}
