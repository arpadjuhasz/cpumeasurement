using CPUMeasurementCommon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPUMeasurementService
{
    public class MeasurementService : BackgroundService //IHostedService
    {
        private Timer _timer;

        private readonly IConfiguration _configuration;
        private IPAddress _serverIPAddress;
        private short _serverPort;
        private readonly int _runIntervalInMinutes;
        private readonly ILogger<MeasurementService> _logger;
        private readonly ComputerDiagnostic _computerDiagnostic;

        public MeasurementService(IConfiguration configuration, ILogger<MeasurementService> logger, ComputerDiagnostic computerDiagnostic)
        {
            this._configuration = configuration;
            this._runIntervalInMinutes = this._configuration.GetValue<int>("runIntervalInMinutes");
            this._serverPort = this._configuration.GetValue<short>("serverPort");
            this._logger = logger;
            this._computerDiagnostic = computerDiagnostic;
            try
            {
                this._serverIPAddress = IPAddress.Parse(this._configuration.GetValue<string>("serverIPAddress"));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            if (this._runIntervalInMinutes <= 0)
            {
                this._runIntervalInMinutes = 5;
            }
        }

        //public async Task StartAsync(CancellationToken cancellationToken)
        //{
        //    //this._timer = new Timer(SendCPUDataPacket, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

        //    //await Task.CompletedTask;
        //    await this.SendCPUDataPacketAsync();
        //        await Task.Delay(5000);
            

        //}

        //public Task StopAsync(CancellationToken cancellationToken)
        //{
        //    return Task.CompletedTask;
        //}

        private void SendCPUDataPacket(object state)
        {
            var packet = this._computerDiagnostic.Update().CPUDataPacket;
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
                    case ResponseStatusCode.REPONSEFORMATERROR: this._logger.LogError("Unknown response code!"); break;
                    case ResponseStatusCode.ERROR: this._logger.LogError("Error occured!"); break;
                }
                client.Dispose();
            }
            catch (Exception)
            {
                this._logger.LogError($"Connection failed. Host: {_serverIPAddress}:{_serverPort}", this._serverIPAddress.ToString(), this._serverPort);
            }
            
        }

        public async Task SendCPUDataPacketAsync()
        {
            var packet = this._computerDiagnostic.Update().CPUDataPacket;
            try
            {
                string message = JObject.FromObject(packet).ToString();
                TcpClient client = new TcpClient(this._serverIPAddress.ToString(), this._serverPort);
                NetworkStream stream = client.GetStream();

                Byte[] data = Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);

                
                byte[] responseBuffer = new byte[1024];
                int responseBytes = client.GetStream().Read(responseBuffer, 0, responseBuffer.Length);

                byte[] filteredBytes = responseBuffer[0..responseBytes];

                string responseMessage = Encoding.ASCII.GetString(filteredBytes);
                ResponseStatusCode responseStatusCode = ResponseStatus.GetReponseStatusCode(responseMessage);
                switch (responseStatusCode)
                {
                    case ResponseStatusCode.REPONSEFORMATERROR: this._logger.LogError("Unknown response code!"); break;
                    case ResponseStatusCode.ERROR: this._logger.LogError("Error occured!"); break;
                }
                client.Dispose();
            }
            catch (Exception)
            {
                this._logger.LogError($"Connection failed. Host: {_serverIPAddress}:{_serverPort}", this._serverIPAddress.ToString(), this._serverPort);
            }
            

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                await this.SendCPUDataPacketAsync();
                await Task.Delay(5000);
            }
        }
    }
}
