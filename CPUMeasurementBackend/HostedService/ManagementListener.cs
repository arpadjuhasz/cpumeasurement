using CPUMeasurementCommon.DataObjects;
using CPUMeasurementCommon.Management;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using CPUMeasurementCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.HostedService
{
    public class ManagementListener : BackgroundService
    {
        private readonly IPAddress _ipAddress;
        private readonly short _managementPort;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly Management _management;
        private readonly TcpListener _tcpListener;
        
        public ManagementListener(IConfiguration configuration, ILogger<ManagementListener> logger, Management management)
        {
            this._logger = logger;
            this._configuration = configuration;
            
            try
            {
                this._managementPort = configuration.GetValue<short>("ManagementPort");
                this._ipAddress = IPAddress.Parse(configuration.GetValue<string>("IPAddress"));
                this._management = management;
                this._tcpListener = new TcpListener(this._ipAddress, this._managementPort);
                this._logger.LogInformation($"Listener started for management on {_ipAddress}:{_managementPort}");
            }
            catch (Exception e)
            {
                this._logger.LogError($"Failed to start listener for management! Please check the configuration file (appsettings). Exception: \n{e.Message}");
            }
        }
        
        public async Task ReceiveClientPacket()
        {
            this._tcpListener.Start();
            while (true)
            {
                var clientTask = _tcpListener.AcceptTcpClientAsync();
                string message = String.Empty;
                if (clientTask.Result != null)
                {
                    var stream = clientTask.Result.GetStream();
                    while (message != null)
                    {
                        byte[] buffer = new byte[1024];
                        int responseBytes = stream.Read(buffer, 0, buffer.Length);
                        message = Encoding.ASCII.GetString(buffer);
                        var clientIPAddress = ((IPEndPoint)clientTask.Result.Client.RemoteEndPoint).Address;
                        try
                        {
                            var jObject = JObject.Parse(message);
                            ManagementPacket clientPacket = jObject.ToObject<ManagementPacket>();
                            var clientData = ManagementData.Create(clientPacket, clientIPAddress);

                            var response = string.Empty;
                            var responsePacket = this._management.UpdateClientData(clientIPAddress, clientData);
                            if (responsePacket != null)
                            {
                                response = JObject.FromObject(responsePacket).ToString();
                            }
                            await stream.WriteStringAsync(response);
                        }
                        catch (Exception e)
                        {
                            this._logger.LogError($"Failed to digest request! {e.Message}");
                        }
                        clientTask.Result.GetStream().Dispose();
                    }

                }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
                await Task.Run(() => { this.ReceiveClientPacket(); });
            
        }
    }
}