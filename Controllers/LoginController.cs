using angularapi.Models;
using angularapi.MyTools;
using AngularApi.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace angularapi.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult RegisterUser(UserDBModel user)
        {
            try
            {
                _userService.CreateAsync(user);
                return Ok(new { message = "Registration successful, please check your email for verification instructions" });
            }
            catch (ApplicationException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult LoginUser(AuthModel model)
        {
            (var user, string refreshToken) = _userService.AuthenticateLogin(model.Name, model.Pass);
            if (user == null)
            {
                return BadRequest(new { message = "Nazwa użytkownika albo hasło jest niepoprawne" });
            }
            return Ok(new
            {
                ID = user.ID,
                Name = user.Name,
                Sub = user.Subscriptions,
                AccessToken = TokenManager.GenerateAccessToken(user.Name),
                RefreshToken = refreshToken

            });
        }

    }
}
