using CPUMeasurementBackend.WebService.Account;
using CPUMeasurementCommon.DataObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.Repository
{
    public class AccountRepository : IRepository
    {
        public string ConnectionString { get; set; }
        private readonly ILogger<AccountRepository> _logger;

        public AccountRepository(IConfiguration configuration, ILogger<AccountRepository> logger)
        {
            this._logger = logger;
            this.ConnectionString = configuration.GetValue<string>("CPUMeasurementConnectionString");
        }

        public async Task Delete(int id)
        {
            string sql = @"BEGIN TRAN Tr1;
                        UPDATE account SET Deleted = 1 WHERE Id = @Id;
                        DELETE FROM Token WHERE AccountId = @Id;
                        COMMIT TRAN Tr1;";
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                await command.ExecuteNonQueryAsync();
                command.Clone();
            }
        }

        public async Task<int> AddAccount(Account account)
        {
            string sql = "INSERT INTO account (Username, Password, Name, Deleted) VALUES (@Username, @Password, @Name, @Deleted)";
            var id = 0;
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("Username", account.Username);
                command.Parameters.AddWithValue("Password", account.Password);
                command.Parameters.AddWithValue("Name", (string.IsNullOrEmpty(account.Name) ? DBNull.Value : (object)account.Name));
                command.Parameters.AddWithValue("Deleted", false);
                connection.Open();
                id = await command.ExecuteNonQueryAsync();
            }
            return id;
        }

        public async Task UpdatePassword(Account account)
        {
            var sql = "UPDATE account SET Password = @Password WHERE Id = @Id";
            using (var connection = new SqlConnection(ConnectionString))
            {
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("Password", account.Password);
                command.Parameters.AddWithValue("Id", account.Id);
                connection.Open();
                await command.ExecuteNonQueryAsync();
                command.Clone();
            }
        }

        public async Task UpdateName(Account account)
        {
            var sql = "UPDATE [account] SET Name = @Name SET Id = @Id";
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("Name", account.Name);
                command.Parameters.AddWithValue("Id", account.Id);
                connection.Open();
                await command.ExecuteNonQueryAsync();
                command.Clone();
            }
        }

        public async Task<List<AccountList>> GetAccounts()
        {
            var sql = @"SELECT Id, Username, Name FROM account WHERE Deleted = 0 ORDER BY Name";
            var accounts = new List<AccountList>();

            using (var connection = new SqlConnection(this.ConnectionString))
            {
                var command = new SqlCommand(sql, connection);
                connection.Open();
                var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    accounts.Add(new AccountList { Id = reader.GetInt32(0), Username = reader.GetString(1), Name = reader.GetString(2) });
                }
                command.Clone();
            }
            return accounts;

        }

        public async Task<bool> CheckUsernameExists(string username)
        {
            var sql = @"SELECT TOP(1) Id FROM account Where Username = @Username AND Deleted = 0";
            var exists = false;
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("Username", username);
                connection.Open();
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    exists = (reader.GetInt32(0) > 0 ? true : false);
                }
            }
            return exists;
        }

        public async Task<Account> GetAccountByUsername(string username)
        {
            var sql = @"SELECT TOP(1) Id, Username, Password, Name FROM account WHERE Username = @Username AND Deleted = 0";
            var account = new Account();
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("Username", username);
                connection.Open();
                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    await reader.ReadAsync();
                    
                        account.Id = reader.GetInt32(0);
                        account.Username = reader.GetString(1);
                        account.Password = reader.GetString(2);
                    account.Name = reader.GetNullableFieldValue<string>(3);
                    
                }
                else
                {
                    account = null;
                }
                await reader.DisposeAsync();
                await command.DisposeAsync();
            }
            return account;
        }

        public Account GetAccountById(int id)
        {
            var sql = @"SELECT Id, Username, Password, Name FROM account WHERE Id = @Id AND Deleted = 0";
            var account = new Account();
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("Id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        account.Id = reader.GetInt32(0);
                        account.Username = reader.GetString(1);
                        account.Password = reader.GetString(2);
                        account.Name = reader.GetNullableFieldValue<string>(3);
                    }
                }
                else
                {
                    account = null;
                }
                reader.Dispose();
                command.Dispose();
            }
            return account;
        }

        public async Task AddToken(AccessToken accessToken)
        {
            var sql = "INSERT INTO Token (Token, AccountId) VALUES (@Token, @AccountId)";
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("Token", accessToken.Token);
                command.Parameters.AddWithValue("AccountId", accessToken.Id);
                connection.Open();
                await command.ExecuteNonQueryAsync();
                command.Clone();
            }
        }

        public async Task DeleteToken(string token)
        {
            var sql = @"DELETE FROM Token WHERE Token = @Token";
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("Token", token);
                connection.Open();
                await command.ExecuteNonQueryAsync();
                command.Clone();
            }
        }

        public void DeleteTokenByAccountId(int accountId)
        {
            var sql = @"DELETE FROM Token WHERE AccountId = @AccountId";
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("AccountId", accountId);
                connection.Open();
                command.ExecuteNonQuery();
                command.Clone();
            }
        }

            public List<string> GetTokensByAccountId(int id)
        {
            var sql = @"SELECT Token FROM [dbo].[Token] WHERE AccountId = @AccountId";
            var tokens = new List<string>();
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("AccountId", id);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tokens.Add(reader.GetString(0));  
                    }
                }
                reader.Close();
                command.Clone();
            }
            return tokens;
        }
    }
}
