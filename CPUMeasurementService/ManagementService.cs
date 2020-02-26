﻿using CPUMeasurementCommon;
using CPUMeasurementCommon.DataObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPUMeasurementService
{
    class ManagementService : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ManagementService> _logger;
        private readonly IPAddress _serverIPAddress;
        private readonly short _serverPort;

        public ManagementService(IConfiguration configuration, ILogger<ManagementService> logger)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._serverPort = this._configuration.GetValue<short>("serverManagementPort");
            try
            {
                this._serverIPAddress = IPAddress.Parse(this._configuration.GetValue<string>("serverIPAddress"));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            this._logger.LogInformation("Started successfully!");
            
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (true && !cancellationToken.IsCancellationRequested)
            {
                await Task.Run(() => { this.SendClientPacket(); });
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void SendClientPacket()
        {
            var packet =  (new ComputerDiagnostic()).GetCPUDataPacket();
            try
            {
                string message = JObject.FromObject(packet).ToString();
                TcpClient client = new TcpClient(this._serverIPAddress.ToString(), this._serverPort);
                NetworkStream stream = client.GetStream();

                Byte[] data = Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);

                byte[] responseData = new byte[256];
                int responseBytes = stream.Read(responseData, 0, responseData.Length);
                string responseMessage = Encoding.ASCII.GetString(responseData, 0, responseData.Length);
                ResponseStatusCode responseStatusCode = ResponseStatus.GetReponseStatusCode(responseMessage);
                switch (responseStatusCode)
                {
                    case ResponseStatusCode.REPONSEFORMATERROR: this._logger.LogError("Unknows response code!"); break;
                    case ResponseStatusCode.ERROR: this._logger.LogError("Error occured!"); break;
                }
                client.Dispose();
            }
            catch (Exception)
            {
                this._logger.LogError($"Connection failed. Host: {_serverIPAddress}:{_serverPort}", this._serverIPAddress.ToString(), this._serverPort);
            }
        }


    }
}