using CPUMeasurementCommon;
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
    public class MeasurementService : BackgroundService
    {
private IPAddress _serverIPAddress;
        private int _serverMeasurementPort;
        private readonly ILogger<MeasurementService> _logger;
        private readonly ComputerDiagnostic _computerDiagnostic;
        private readonly ClientConfigurationReader _configurationReader;
        private readonly CancelService _cancelService;
        private readonly CycleStorageService _cycleStorageService;

        public MeasurementService(ILogger<MeasurementService> logger, ComputerDiagnostic computerDiagnostic, ClientConfigurationReader configurationReader, CancelService cancelService, CycleStorageService cycleStorageService)
        {
            _cancelService = cancelService;
            _configurationReader = configurationReader;
            _cycleStorageService = cycleStorageService;
            _logger = logger;
            _computerDiagnostic = computerDiagnostic;
            
            try
            {
                _serverIPAddress = IPAddress.Parse(this._configurationReader.Configuration.ServerIPAddress);
                _serverMeasurementPort = this._configurationReader.Configuration.ServerMeasurementPort;
            }
            catch (Exception)
            {
                _logger.LogError("Invalid server IP address format, port number in the client_configuration.json file! IPAddress set to default (127.0.0.1 with port 1400)");
                var defaultClientConfiguration = new ClientConfiguration();
                _serverIPAddress = IPAddress.Parse(defaultClientConfiguration.ServerIPAddress);
                _serverMeasurementPort = defaultClientConfiguration.ServerMeasurementPort;
            }
        }

        public void SendMeasurementPacket()
        {
            var packet = this._computerDiagnostic.Update().MeasurementPacket;
            _cycleStorageService.AddToCycleStorageLogs(packet);

            try
            {
                TcpClient client = new TcpClient(this._serverIPAddress.ToString(), this._serverMeasurementPort);
                NetworkStream stream = client.GetStream();
                
                string data = JObject.FromObject(packet).ToString();
                byte[] dataBuffer = Encoding.ASCII.GetBytes(data);
                stream.Write(dataBuffer, 0, dataBuffer.Length);

                
                byte[] responseBuffer = new byte[1024];
                int responseBytes = client.GetStream().Read(responseBuffer, 0, responseBuffer.Length);

                byte[] filteredBytes = responseBuffer[0..responseBytes];

                string response = Encoding.ASCII.GetString(filteredBytes);
                ResponseStatusCode responseStatusCode = ResponseStatus.GetReponseStatusCode(response);
                switch (responseStatusCode)
                {
                    case ResponseStatusCode.RESPONSEFORMATERROR: this._logger.LogError("Unknown response code!"); break;
                    case ResponseStatusCode.ERROR: this._logger.LogError("Error occured!"); break;
                }
                _logger.LogInformation($"Measurement sent successfully to {_serverIPAddress}:{_serverMeasurementPort} !");
                client.Dispose();
                
            }
            catch (Exception e)
            {
                _logger.LogError($"Connection failed. Host: {_serverIPAddress.ToString()}:{_serverMeasurementPort} Exception: {e.Message}");
            }
            

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                this.SendMeasurementPacket();
                await Task.Delay(this._configurationReader.Configuration.MeasurementIntervalInSeconds*1000, this._cancelService.CancelationToken);
                _cancelService.Renew();
            }
        }

        public async override Task StopAsync(CancellationToken cancellationToken)
        {
            this._cycleStorageService.WriteLogsToFile();
        }
    }
}
