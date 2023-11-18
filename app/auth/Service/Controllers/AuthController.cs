using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Service.Services;

namespace Service.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly JwtService JWT;
        public AuthController(JwtService jwt)
        {
            this.JWT = jwt;
        }

        [AllowAnonymous]
        [HttpGet]
        public string demo()
        {
            return JWT.GenerateToken("demo-user");
        }

        [AllowAnonymous]
        [HttpPost("[action]/{username}")]
        public string Token(string username)
        {
            Console.WriteLine("> Token call " + username);
            return JWT.GenerateToken(username);
        }

        [HttpPost("[action]")]
        public string Check()
        {
            try {
              Console.WriteLine("> Check call " + Request.Headers["Authorization"]);
              User.Claims.ToList().ForEach(p => Console.WriteLine($"> {p.Type} - {p.Value}"));
              Console.WriteLine("> Claims.jti = " + User.Claims.FirstOrDefault(p => p.Type == "jti").Value);
            } catch (Exception e) {
                Console.WriteLine($"Processing failed: {e.Message}");
                return e.Message;
            }
            return User.Identity.Name;
        }

        [Authorize(Roles = "Root")]
        [HttpGet("[action]")]
        public string RootRole()
        {
            return "Auth check : Token has root roles";
        }
    }
}
