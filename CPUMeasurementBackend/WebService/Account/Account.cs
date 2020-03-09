﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CPUMeasurementBackend.WebService.Account
{
    public class Account
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }

        public static Account Create(string username, string rawPassword, string name, string salt)
        {
            var provider = MD5.Create();
        
            byte[] bytes = provider.ComputeHash(Encoding.ASCII.GetBytes(salt + rawPassword));
            string computedHash = BitConverter.ToString(bytes);
            return new Account { Deleted = false, Name = name, Password = computedHash, Username = username };
                
        }

        public static string GetSaltedPassword(string rawPassword, string salt)
        {
            var provider = MD5.Create();
            byte[] bytes = provider.ComputeHash(Encoding.ASCII.GetBytes(salt + rawPassword));
            return BitConverter.ToString(bytes);
        }
    }
}