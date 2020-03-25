using CPUMeasurementBackend.Repository;
using CPUMeasurementCommon.DataObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.WebService.Account
{
    public class AccountService
    {
        private readonly AccountRepository _accountRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountService> _logger;
        private readonly string _salt;

        public AccountService(IConfiguration configuration, ILogger<AccountService> logger, AccountRepository accountRepository)
        {
            this._configuration = configuration;
            this._accountRepository = accountRepository;
            this._salt = this._configuration.GetValue<string>("Salt");
            this._logger = logger;
        }

        public List<AccountList> GetAccounts()
        {
            return this._accountRepository.GetAccounts();
        }

        public AccountGet AddAccount(AccountPost dto)
        {
            var usernameExists = this._accountRepository.CheckUsernameExists(dto.Username);
            if (!usernameExists)
            {
                var account = Account.Create(dto.Username, dto.Password, dto.Name, this._salt);
                var id = this._accountRepository.AddAccount(account);
                return new AccountGet { Id = id, Name = account.Name, Username = account.Username };
            }
            else
            {
                return null;
            }
        }

        public void DeleteAccount()
        {
            var accountId = AuthorizationService.AccountId;
            this._accountRepository.Delete(accountId);
        }

        public async Task Logout()
        {
            string token = AuthorizationService.Token;
            this._accountRepository.DeleteToken(token);   
        }

        public AccessToken Login(LoginPost dto)
        {
            var saltedPassword = Account.GetSaltedPassword(dto.Password, this._salt);
            var account = this._accountRepository.GetAccountByUsername(dto.Username);
            if (account != null && saltedPassword == account.Password)
            {
                var accessToken = AccessTokenGenerator.Create(account, this._salt);
                this._accountRepository.AddToken(accessToken);
                return accessToken;
            }
            else
            {
                return null;
            }
        }

        

        public Account GetAccountById(int id)
        {
            var account = this._accountRepository.GetAccountById(id);
            return account;
        }

        public  List<string> GetTokensByUserId(int accountId)
        {
            return this._accountRepository.GetTokensByAccountId(accountId);
        }

        internal void DeleteAccessToken(int accountId)
        {
            this._accountRepository.DeleteTokenByAccountId(accountId);
        }
    }
}
