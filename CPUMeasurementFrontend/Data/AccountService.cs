using CPUMeasurementCommon.DataObjects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CPUMeasurementFrontend.Data
{
    public class AccountService
    {
        private readonly HttpClient _httpClient;

        public AccountService(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public async Task<AccountGet> AddAccount(string username, string password, string passwordAgain, string name)
        {
            var account = new AccountPost { Name = name, Username = username, Password = password, PasswordAgain = passwordAgain };
            var createdAccount = await this._httpClient.PostJsonAsync<AccountGet>("/account/register", account);
            return createdAccount;
        }

        public async Task Login(string username, string password)
        {
            
            LoginPost dto = new LoginPost { Password = password, Username = username };
            try
            {
                AccessToken accessToken = await this._httpClient.PostJsonAsync<AccessToken>("/account/login", dto);
               
                if (accessToken != null)
                {
                    this._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken.Token);
                }
            }
            catch (Exception)
            { 
                
            }
        }

        public async Task Logout()
        {
            await this._httpClient.PostJsonAsync("/account/logout", null);
            this._httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public bool IsLoggedIn()
        {
            if (this._httpClient.DefaultRequestHeaders.Authorization != null && !string.IsNullOrEmpty(this._httpClient.DefaultRequestHeaders.Authorization.Parameter))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
