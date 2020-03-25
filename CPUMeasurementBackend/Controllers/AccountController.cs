using CPUMeasurementBackend.WebService.Account;
using CPUMeasurementCommon;
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
            var token = this._accountService.Login(login);
            if (token != null)
            {
                return Ok(token);
            }
            else
            {
                return BadRequest(new ErrorMessage{ Error = "Wrong username or password!"  });
            }
            
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await this._accountService.Logout();
            return NoContent();
        }

        [HttpDelete()]
        public IActionResult DeleteAccount()
        {
            this._accountService.DeleteAccount();
            return NoContent();
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody]AccountPost post)
        {
            var account = this._accountService.AddAccount(post);
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
