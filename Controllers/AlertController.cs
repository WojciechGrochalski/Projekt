using angularapi.Models;
using AngularApi.DataBase;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace angularapi.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class AlertController : ControllerBase
    {
        private readonly CashDBContext _context;
        private readonly ILogger<AlertController> _logger;

        public AlertController(CashDBContext context, ILogger<AlertController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("alerts/{userID}")]
        public List<Remainder> GetsUserAlerts(int userID)
        {
            var alerts = _context.Remainders.Where(p => p.UserID == userID).ToList();
            return alerts;
        }
        [HttpPost("addAlert")]
        public IActionResult AddAlert([FromBody] Remainder remainder)
        {
            try
            {
                var user = _context.userDBModels.FirstOrDefault(s => s.ID == remainder.UserID);
                if (user != null)
                {
                    _context.Remainders.Add(remainder);
                    _context.SaveChanges();
                    return Ok(new { message = "Pomyślnie ustawiono alert" });
                }
                return BadRequest(new { message = "Nie udało się ustawić alertu, spróbuj ponownie" });
            }
            catch (ApplicationException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(new { message = "Nie udało się ustawić alertu, spróbuj ponownie" });
            }

        }
    }
}
