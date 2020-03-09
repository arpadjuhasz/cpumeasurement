using CPUMeasurementBackend.WebService.Account;
using CPUMeasurementCommon.DataObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            this._accountService = accountService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]LoginPost login)
        {
            var token = await this._accountService.Login(login);
            if (token != null)
            {
                return Ok(token);
            }
            else
            {
                return BadRequest(new { error = "Wrong username or password!"  });
            }
            
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            StringValues authorization = string.Empty;
            HttpContext.Request.Headers.TryGetValue("Authorization", out authorization);
            await this._accountService.Logout(authorization.ToString().Substring(7));
            return NoContent();
            

        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]AccountPost post)
        {
            var account = await this._accountService.AddAccount(post);
            if (account != null)
            {
                return Created($"{account.Id}", account);
            }
            else
            {
                return BadRequest(new { error = "Username already exists!" });
            }


        }
    }
}
