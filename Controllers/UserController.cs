using System;
using System.Linq;
using ShoppingifyAPI.Models;
using ShoppingifyAPI.Context;
using Microsoft.AspNetCore.Mvc;
using static BCrypt.Net.BCrypt;
using System.Text.RegularExpressions;
using ShoppingifyAPI.Services;

namespace ShoppingifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly ApiContext _context;

        public UserController(ApiContext context) => _context = context;

        [HttpPost("signup")]
        public ActionResult SignUp([FromBody] User user)
        {
            try
            {
                if (!IsValidEmail(user.Email))
                    throw new ArgumentException("Invalid email");

                if (user.Name.Trim().Length < 3)
                    throw new ArgumentException("Name must have at least 3 letters");

                if (!IsValidPassword(user.Password))
                    throw new ArgumentException("Password must have a lowercase letter, an uppercase letter, a number and a special character, minimum 8 characters");

                var emailUsed = _context.Users.Any(x => x.Email.ToLower() == user.Email.ToLower());

                if (emailUsed)
                    throw new ArgumentException("Email already used");

                user.Password = HashPassword(user.Password);

                _context.Users.Add(user);
                _context.SaveChanges();

                var token = TokenService.GenerateToken(user);

                return Created("", new { token });
            }
            catch (ArgumentException e)
            {
                return BadRequest(new { error = e.Message });
            }

        }

        [HttpPost("signin")]
        public ActionResult SignIn([FromBody] User user, [FromQuery] bool keepConnected)
        {
            try
            {
                if (!IsValidEmail(user.Email))
                    throw new ArgumentException("Invalid email");

                if (!IsValidPassword(user.Password))
                    throw new ArgumentException("Password must have a lowercase letter, an uppercase letter, a number and a special character");

                var userDb = _context.Users.FirstOrDefault(x => x.Email.ToLower() == user.Email.ToLower());

                if (userDb is null || !Verify(user.Password, userDb.Password))
                    throw new ArgumentException("Email or password invalid");

                var token = TokenService.GenerateToken(userDb);

                if (!keepConnected)
                    return Ok(new { token });

                var userHash = HashPassword(userDb.Email);
                _context.KeepUserLogged.Add(new KeepUserLogged { UserId = userDb.Id, UserHash = userHash });
                _context.SaveChanges();

                return Ok(new { token, userHash });

            }
            catch (ArgumentException e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpGet("connected/{userHash}")]
        public ActionResult GetConnectedUser([FromRoute] string userHash)
        {
            try
            {
                var userId = _context.KeepUserLogged.FirstOrDefault(x => x.UserHash == userHash)?.UserId;

                if (userId is null)
                    throw new ArgumentException("No user found with given hash");

                var user = _context.Users.FirstOrDefault(x => x.Id == userId);

                if (user is null)
                    throw new ArgumentException("Login again");

                var token = TokenService.GenerateToken(user);

                return Ok(new { token });
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        private bool IsValidEmail(string email)
        {
            var regex = new Regex(@"[^@ \t\r\n]+@[^@ \t\r\n]+\.[^@ \t\r\n]+");
            return regex.Match(email).Success;
        }

        private bool IsValidPassword(string password)
        {
            var regex = new Regex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$ %^&*-]).{8,}$");
            return regex.Match(password).Success;
        }
    }
}
