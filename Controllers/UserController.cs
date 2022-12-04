using angularapi.Models;
using angularapi.MyTools;
using AngularApi.DataBase;
using AngularApi.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace angularapi.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly CashDBContext _context;
        private readonly ILogger<UserController> _logger;
        private IUserService _userService;
        public static string ClientBaseUrl = "http://localhost:5001";
        private IMailService _mailService;
        public UserController(CashDBContext context, IUserService userService,
            ILogger<UserController> logger, IMailService mailService)
        {
            _context = context;
            _userService = userService;
            _logger = logger;
            _mailService = mailService;
        }


        [HttpPost("resetPassword")]
        [AllowAnonymous]
        public IActionResult ResetPassword([FromBody] NewPassword newPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = _context.userDBModels.FirstOrDefault(s => s.Email == newPassword.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Nie istnieje użytkownik o takim emailu" });
            }
            user.ResetPasswordToken = TokenManager.RandomTokenString();

            string resetPasswordtoken = TokenManager.GenerateResetPassToken(user.ResetPasswordToken, user.Email);
            _context.userDBModels.Update(user);
            _context.SaveChanges();
            string message = $@"<p>Wysłano powiadomienie o zresetowaniu hasła</p>
                                <p>Kliknij link aby dokończyć resetowanie</p>
                             <p> <a href=""{ClientBaseUrl}new-password?token={resetPasswordtoken}""> link <a/> </p>";
            _mailService.SendMail(newPassword.Email, "Resetowanie Hasła", message);
            return Ok(new { message = "Link do zresetowania hasło został wysłany na twój e-mail" });

        }
        [HttpPost("verify-resetpassword")]
        [AllowAnonymous]
        public IActionResult VerifyPaswordToken([FromBody] VerifyEmailRequest token)
        {
            bool result = _userService.VerifyPasswordToken(token.Token);
            string email = TokenManager.ValidateJwtToken(token.Token, "email");

            if (result)
            {
                return Ok(new { email = email });
            }
            return BadRequest(new { message = "Token wygasł lub jest nieprawidłowy" });
        }
        [HttpPost("setPassword")]
        [AllowAnonymous]
        public IActionResult SetNewPassword([FromBody] NewPassword data)
        {
            var user = _context.userDBModels.FirstOrDefault(s => s.Email == data.Email);
            if (user != null)
            {
                string passwordHash = SecurePasswordHasher.Hash(data.Password);
                user.Pass = passwordHash;
                _context.userDBModels.Update(user);
                _context.SaveChanges();
                return Ok(new { message = "Zmieniono hasło" });
            }
            return BadRequest(new { message = "Coś poszło nie tak, spróbuj ponownie" });
        }
        [HttpPost("verify-email")]
        [AllowAnonymous]
        public IActionResult VerifyEmail([FromBody] VerifyEmailRequest jwtToken)
        {
            bool result = _userService.VerifyEmail(jwtToken.Token);
            if (result)
            {
                return Ok(new { message = "Verification successful, you can now login" });
            }
            return BadRequest(new { message = "Invalid access token" });
        }

        [HttpPost("baseUrl")]
        [AllowAnonymous]
        public IActionResult GetBaseUrl(string url)
        {
            ClientBaseUrl = url;
            return Ok();
        }

        [HttpGet("sub/{userID}")]
        public IActionResult AddSubscriptions(int userID)
        {
            var user = _context.userDBModels.FirstOrDefault(s => s.ID == userID);
            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
            user.Subscriptions = true;
            _context.Entry(user).CurrentValues.SetValues(user);
            _context.SaveChanges();
            return Ok(new { message = "Zostałeś dodany do subskrybcji" });
        }
        [HttpGet("remove/sub/{userID}")]
        public IActionResult RemoveSubscriptions(int userID)
        {
            var user = _context.userDBModels.FirstOrDefault(s => s.ID == userID);
            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
            user.Subscriptions = false;
            _context.Entry(user).CurrentValues.SetValues(user);
            _context.SaveChanges();
            return Ok(new { message = "Usunięto cię z subskrybcji" });
        }

    }
}
