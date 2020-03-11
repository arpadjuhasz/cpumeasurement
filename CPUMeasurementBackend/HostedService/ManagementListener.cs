using CPUMeasurementCommon.DataObjects;
using CPUMeasurementCommon.Management;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
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
    public class ManagementListener : BackgroundService//IHostedService
    {
        private readonly IPAddress _ipAddress;
        private readonly short _managementPort;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly Management _management;
        private readonly TcpListener _tcpListener;
        

        public ManagementListener(IConfiguration configuration, ILogger<ManagementListener> logger, Management management)
        {

            this._managementPort = configuration.GetValue<short>("ManagementPort");
            this._ipAddress = IPAddress.Parse(configuration.GetValue<string>("IPAddress"));
            this._configuration = configuration;
            this._logger = logger;
            this._management = management;
            this._tcpListener = new TcpListener(this._ipAddress, this._managementPort);
            
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
                    while (message != null)
                    {
                        byte[] buffer = new byte[1024];
                        int responseBytes = clientTask.Result.GetStream().Read(buffer, 0, buffer.Length);
                        message = Encoding.ASCII.GetString(buffer);
                        var clientIPAddress = ((IPEndPoint)clientTask.Result.Client.RemoteEndPoint).Address;
                        try
                        {
                            var jobject = JObject.Parse(message);
                            ManagementPacket clientPacket = jobject.ToObject<ManagementPacket>();
                            var clientData = new ManagementData
                            {
                                IPv4Address = clientIPAddress.ToString(),
                                CPUName = clientPacket.CPUName,
                                MeasurementInterval = clientPacket.MeasurementIntervalInSeconds,
                                ComputerName = clientPacket.ComputerName,
                                LastUpdate = DateTime.UtcNow,
                                UpdateRequested = false
                            };

                            var response = string.Empty;
                            var responsePacket = this._management.UpdateClientData(clientIPAddress, clientData);
                            if (responsePacket != null)
                            {
                                response = JObject.FromObject(responsePacket).ToString();
                            }

                            var messageBytes = Encoding.ASCII.GetBytes(response);
                            await clientTask.Result.GetStream().WriteAsync(messageBytes, 0, messageBytes.Length);
                        }
                        catch (Exception)
                        {

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