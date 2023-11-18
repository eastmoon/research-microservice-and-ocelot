using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CheckController : ControllerBase
    {
        //[Authorize(Roles = "Root")]
        [HttpGet("[action]")]
        public string RootRole()
        {
            return "Token has root roles";
        }

        //[Authorize(UserType = "Admin")]
        [HttpGet("[action]")]
        public string AdminRole()
        {
            return "Token has admin UserType";
        }

        //[Authorize(UserType = "Developer")]
        [HttpGet("[action]")]
        public string DeveloperRole()
        {
            return "Token has developer UserType";
        }

        //Authorize(UserType = "User")]
        [HttpGet("[action]")]
        public string UserRole()
        {
            return "Token has users UserType";
        }

        //[Authorize(Policy = "AccessLevel-1")]
        [HttpGet("[action]")]
        public string Level1()
        {
            return "Token has AccessLevel 1";
        }

        //[Authorize(Policy = "AccessLevel-2")]
        [HttpGet("[action]")]
        public string Level2()
        {
            return "Token has AccessLevel 2";
        }

        //[Authorize(Policy = "AccessLevel-1")]
        //[Authorize(UserType = "Admin")]
        [HttpGet("[action]")]
        public string MultiRule1()
        {
            return "Token has AccessLevel 1 and UserType Admin";
        }

        //[Authorize(Policy = "AccessLevel-2")]
        //[Authorize(UserType = "Admin")]
        [HttpGet("[action]")]
        public string MultiRule2()
        {
            return "Token has AccessLevel 2 and UserType Admin";
        }

        //[Authorize(Policy = "AccessLevel-1")]
        //[Authorize(UserType = "Developer")]
        [HttpGet("[action]")]
        public string MultiRule3()
        {
            return "Token has AccessLevel 1 and UserType Developer";
        }
    }
}
