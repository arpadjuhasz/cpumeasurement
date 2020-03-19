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
            this._cancelService = cancelService;
            this._configurationReader = configurationReader;
            this._cycleStorageService = cycleStorageService;
            try
            {
                this._serverMeasurementPort = this._configurationReader.Configuration.ServerMeasurementPort;
            }
            catch (Exception)
            {
                this._logger.LogError("Invalid port number!");
            }
            
            this._logger = logger;
            this._computerDiagnostic = computerDiagnostic;
            try
            {
                this._serverIPAddress = IPAddress.Parse(this._configurationReader.Configuration.ServerIPAddress);
            }
            catch (Exception)
            {
                this._logger.LogError("Invalid server IP address format in the client_configuration.json file!");
                this._configurationReader.Configuration.ServerIPAddress = "127.0.0.1";
            }
        }

        public async Task SendMeasurementPacket()
        {
            var packet = this._computerDiagnostic.Update().MeasurementPacket;
            this._cycleStorageService.AddToLogs(packet.MeasurementDate, packet);

            try
            {
                TcpClient client = new TcpClient(this._serverIPAddress.ToString(), this._serverMeasurementPort);
                NetworkStream stream = client.GetStream();
                
                string data = JObject.FromObject(packet).ToString();
                Byte[] dataBuffer = Encoding.ASCII.GetBytes(data);
                stream.Write(dataBuffer, 0, dataBuffer.Length);

                
                byte[] responseBuffer = new byte[1024];
                int responseBytes = client.GetStream().Read(responseBuffer, 0, responseBuffer.Length);

                byte[] filteredBytes = responseBuffer[0..responseBytes];

                string response = Encoding.ASCII.GetString(filteredBytes);
                ResponseStatusCode responseStatusCode = ResponseStatus.GetReponseStatusCode(response);
                switch (responseStatusCode)
                {
                    case ResponseStatusCode.REPONSEFORMATERROR: this._logger.LogError("Unknown response code!"); break;
                    case ResponseStatusCode.ERROR: this._logger.LogError("Error occured!"); break;
                }
                this._logger.LogInformation($"Measurement sent successfully to {_serverIPAddress}:{_serverMeasurementPort} !");
                client.Dispose();
                
            }
            catch (Exception)
            {
                this._logger.LogError($"Connection failed. Host: {_serverIPAddress.ToString()}:{_serverMeasurementPort}");
            }
            

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                await this.SendMeasurementPacket();
                await Task.Delay(this._configurationReader.Configuration.MeasurementIntervalInSeconds*1000, this._cancelService.CancelationToken);
                this._cancelService.Renew();
            }
        }

        public async override Task StopAsync(CancellationToken cancellationToken)
        {
            this._cycleStorageService.WriteLogsToFile();
        }
    }
}
