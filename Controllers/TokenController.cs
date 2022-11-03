using angularapi.Models;
using angularapi.MyTools;
using AngularApi.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace angularapi.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private IUserService _userService;

        public TokenController(IUserService userService)
        {
            _userService = userService;
        }
        [Authorize(AuthenticationSchemes = "refresh")]
        [HttpGet("refreshToken", Name = "refresh")]
        public IActionResult RefreshToken()
        {
            Claim refreshToken = User.Claims.FirstOrDefault(x => x.Type == "refresh");
            Claim username = User.Claims.FirstOrDefault(x => x.Type == "user");
            try
            {
                Tokens tokens = _userService.Refresh(username, refreshToken);
                return Ok(new
                {
                    AccessToken = tokens.AccessToken,
                    RefreshToken = tokens.RefreshToken
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
