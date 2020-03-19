using CPUMeasurementBackend.Repository;
using CPUMeasurementCommon;
using CPUMeasurementCommon.DataObjects;
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
    public class MeasurementListener : BackgroundService//IHostedService
    {
        private readonly IPAddress _ipAddress;
        private readonly TcpListener _tcpListener;
        private readonly short _measurementPort;
        private IConfiguration _configuration;
        private ILogger _logger;
        
        public MeasurementListener(IConfiguration configuration, ILogger<MeasurementListener> logger)
        {
            
            this._measurementPort = configuration.GetValue<short>("MeasurementPort");
            this._ipAddress = IPAddress.Parse(configuration.GetValue<string>("IPAddress"));
            this._tcpListener = new TcpListener(_ipAddress, this._measurementPort);
            this._configuration = configuration;
            this._logger = logger;
            this._logger.LogInformation($"Measurement listener is waiting for packets on {_ipAddress}:{_measurementPort}");
        }

        public async Task ReceiveMeasurementPacket()
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
                            var jsonObject = JObject.Parse(message);
                            if (jsonObject != null)
                            {
                                MeasurementPacket cpuPacket = jsonObject.ToObject<MeasurementPacket>();

                                var repository = new MeasurementRepository(this._configuration);
                                await repository.SaveMeasurementData(MeasurementData.Create(cpuPacket, clientIPAddress));
                                this._logger.LogInformation($"Measurement saved to database from {clientIPAddress}");

                                var bytes = Encoding.ASCII.GetBytes(((int)ResponseStatusCode.SUCCESS).ToString());
                                await clientTask.Result.GetStream().WriteAsync(bytes, 0, bytes.Length);
                            }
                            else
                            {
                                var bytes = Encoding.ASCII.GetBytes(ResponseStatusCode.ERROR.ToString());
                                await clientTask.Result.GetStream().WriteAsync(bytes, 0, bytes.Length);
                            }

                        }
                        catch (Exception)
                        {
                            this._logger.LogError($"Failed to save the data from client! IPAddress: {clientIPAddress.ToString()}, Message: {message}");
                            var bytes = Encoding.ASCII.GetBytes(ResponseStatusCode.ERROR.ToString());
                            clientTask.Result.GetStream().Write(bytes, 0, bytes.Length);
                        }
                        clientTask.Result.GetStream().Dispose();
                    }

                }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            await Task.Run(() => { this.ReceiveMeasurementPacket(); });
        }

        

        
    }
}
