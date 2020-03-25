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
            this._logger = logger;
            this._configuratoinReader = configurationReader;
            
            this._computerDiagnostic = computerDiagnostic;
            
            try
            {
                _serverIPAddress = IPAddress.Parse(this._configuratoinReader.Configuration.ServerIPAddress);
                _serverManagementPort = this._configuratoinReader.Configuration.ServerManagementPort;
            }
            catch (Exception e)
            {
                _logger.LogError("Invalid server IP address format, port in the client_configuration.json file! IPAddress set to default (127.0.0.1 with port 1400)");
                var defaultClientConfiguration = new ClientConfiguration();
                _serverIPAddress = IPAddress.Parse(defaultClientConfiguration.ServerIPAddress);
                _serverManagementPort = defaultClientConfiguration.ServerManagementPort;
            }
        }

        private void SendClientPacket(object state)
        {
            ManagementPacket packet = this._computerDiagnostic.ClientPacket;
            packet.MeasurementIntervalInSeconds = this._configuratoinReader.Configuration.MeasurementIntervalInSeconds;

            try
            {
                string message = JObject.FromObject(packet).ToString();
                TcpClient client = new TcpClient(this._serverIPAddress.ToString(), this._serverManagementPort);
                NetworkStream stream = client.GetStream();

                byte[] data = Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);

                byte[] responseData = new byte[256];
                int responseBytes = stream.Read(responseData, 0, responseData.Length);
                if (responseBytes > 1)
                {
                    var responseMessage = Encoding.ASCII.GetString(responseData);
                    try
                    {
                        MeasurementIntervalUpdatePacket updatePacket = JObject.Parse(responseMessage).ToObject<MeasurementIntervalUpdatePacket>();
                        _configuratoinReader.SetMeasurementInterval(updatePacket.MeasurementIntervalInSeconds);
                        _cancelService.CancelationToken.ThrowIfCancellationRequested();
                        _logger.LogInformation("Successfully changed client configuration!");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Failed to process response! Exception: \n{e.Message}");
                    }
                }
                client.Dispose();
            }
            catch (Exception e)
            {
                _logger.LogError($"Connection failed to server: {_serverIPAddress.ToString()}:{_serverManagementPort}! Exception: \n{e.Message}");
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(SendClientPacket, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
