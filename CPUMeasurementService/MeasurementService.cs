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

        public MeasurementService(ILogger<MeasurementService> logger, ComputerDiagnostic computerDiagnostic, ClientConfigurationReader configurationReader, CancelService cancelService)
        {
            this._cancelService = cancelService;
            this._configurationReader = configurationReader;

            this._serverMeasurementPort = this._configurationReader.Configuration.ServerMeasurementPort;
            this._logger = logger;
            this._computerDiagnostic = computerDiagnostic;
            try
            {
                this._serverIPAddress = IPAddress.Parse(this._configurationReader.Configuration.ServerIPAddress);
            }
            catch (Exception e)
            {
                this._logger.LogError("Invalid server IP address in the client_configuration.json file!");
                this._configurationReader.Configuration.ServerIPAddress = "127.0.0.1";
            }
        }

        private void SendCPUDataPacket(object state)
        {
            var packet = this._computerDiagnostic.Update().CPUDataPacket;
            try
            {
                string message = JObject.FromObject(packet).ToString();
                TcpClient client = new TcpClient(this._serverIPAddress.ToString(), this._serverMeasurementPort);
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
                this._logger.LogError($"Connection failed. Host: {_serverIPAddress}:{_serverMeasurementPort}");
            }
            
        }

        public async Task SendCPUDataPacketAsync()
        {
            var packet = this._computerDiagnostic.Update().CPUDataPacket;
            packet.MeasurementIntervalInSeconds = this._configurationReader.Configuration.MeasurementIntervalInSeconds;
            packet.MeasurementDate = DateTime.UtcNow;
            try
            {
                string message = JObject.FromObject(packet).ToString();
                TcpClient client = new TcpClient(this._serverIPAddress.ToString(), this._serverMeasurementPort);
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
                this._logger.LogError($"Connection failed. Host: {_serverIPAddress.ToString()}:{_serverMeasurementPort}");
            }
            

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                await this.SendCPUDataPacketAsync();
                await Task.Delay(this._configurationReader.Configuration.MeasurementIntervalInSeconds*1000, this._cancelService.CancelationToken);
                this._cancelService.Renew();
                
            }
        }
    }
}
