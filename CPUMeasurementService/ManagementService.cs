using CPUMeasurementCommon;
using CPUMeasurementCommon.DataObjects;
using CPUMeasurementCommon.Management;
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
    class ManagementService : IHostedService
    {
        
        private readonly ILogger<ManagementService> _logger;
        private readonly IPAddress _serverIPAddress;
        private Timer _timer { get; set; }
        private readonly int _serverManagementPort;
        private readonly ComputerDiagnostic _computerDiagnostic;
        private readonly ClientConfigurationReader _configuratoinReader;

        public ManagementService(ILogger<ManagementService> logger, ComputerDiagnostic computerDiagnostic, ClientConfigurationReader configurationReader)
        {
            this._logger = logger;this._configuratoinReader = configurationReader;
            this._serverManagementPort = this._configuratoinReader.Configuration.ServerManagementPort;
            this._computerDiagnostic = computerDiagnostic;
            try
            {
                this._serverIPAddress = IPAddress.Parse(this._configuratoinReader.Configuration.ServerIPAddress);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void SendClientPacket(object state)
        {
            ClientPacket packet = this._computerDiagnostic.ClientPacket;
            packet.MeasurementIntervalInSeconds = this._configuratoinReader.Configuration.MeasurementIntervalInSeconds;
            Socket s  = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPAddress broadcast = this._serverIPAddress;
            byte[] sendbuf = Encoding.ASCII.GetBytes(JObject.FromObject(packet).ToString());
            IPEndPoint ep = new IPEndPoint(broadcast, this._serverManagementPort);
            s.SendTo(sendbuf, ep);
            this._logger.LogInformation(JObject.FromObject(packet).ToString());
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(SendClientPacket, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            await Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
