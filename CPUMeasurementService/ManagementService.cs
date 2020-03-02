using CPUMeasurementCommon.Management;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
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
        private readonly CancelService _cancelService;

        public ManagementService(ILogger<ManagementService> logger, ComputerDiagnostic computerDiagnostic, ClientConfigurationReader configurationReader, CancelService cancelService)
        {
            this._cancelService = cancelService;
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

            try
            {
                string message = JObject.FromObject(packet).ToString();
                TcpClient client = new TcpClient(this._serverIPAddress.ToString(), this._serverManagementPort);
                NetworkStream stream = client.GetStream();

                Byte[] data = Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);

                byte[] responseData = new byte[256];
                int responseBytes = stream.Read(responseData, 0, responseData.Length);
                if (responseBytes > 1)
                {
                    var responseMessage = Encoding.ASCII.GetString(responseData);
                    try
                    {
                        MeasurementIntervalUpdatePacket updatePacket = JObject.Parse(responseMessage).ToObject<MeasurementIntervalUpdatePacket>();
                        this._configuratoinReader.SetMeasurementInterval(updatePacket.MeasurementIntervalInSeconds);
                        this._cancelService.CancelationToken.ThrowIfCancellationRequested();
                    }
                    catch (Exception e)
                    {
                        this._logger.LogError(e.Message);
                    }
                }
                client.Dispose();
            }
            catch (Exception)
            {
                this._logger.LogError($"Connection failed. Host: {_serverIPAddress.ToString()}:{_serverManagementPort}");
            }
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
