using CPUMeasurementBackend.Repositories;
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
        private readonly IConfiguration _configuration;
        private readonly ILogger<MeasurementListener> _logger;
        
        public MeasurementListener(IConfiguration configuration, ILogger<MeasurementListener> logger)
        {
            this._configuration = configuration;
            this._logger = logger;
            try
            {
                this._measurementPort = configuration.GetValue<short>("MeasurementPort");
                this._ipAddress = IPAddress.Parse(configuration.GetValue<string>("IPAddress"));
                this._tcpListener = new TcpListener(_ipAddress, this._measurementPort);
                this._logger.LogInformation($"Listener started for measurements on {_ipAddress}:{_measurementPort}");
            }
            catch (Exception e)
            {
                this._logger.LogError($"Failed to start listener for measurements! Please check the configuration file (appsettings). Exception: \n{e.Message}");
            }
                
            
            
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
                    NetworkStream stream = clientTask.Result.GetStream();
                    while (message != null)
                    {
                        byte[] buffer = new byte[1024];
                        int responseBytes = stream.Read(buffer, 0, buffer.Length);
                        message = Encoding.ASCII.GetString(buffer);
                        var clientIPAddress = ((IPEndPoint)clientTask.Result.Client.RemoteEndPoint).Address;
                        try
                        {
                            var jsonObject = JObject.Parse(message);
                            if (jsonObject != null)
                            {
                                MeasurementPacket cpuPacket = jsonObject.ToObject<MeasurementPacket>();

                                var repository = new CPUMeasurementRepository(this._configuration);
                                if (( repository.AddMeasurementData(CPUMeasurement.Create(cpuPacket, clientIPAddress), this._logger)).HasValue)
                                {
                                    await stream.WriteStringAsync(((int)ResponseStatusCode.SUCCESS).ToString());
                                }
                                else
                                {
                                    await stream.WriteStringAsync(((int)ResponseStatusCode.ERROR).ToString());
                                }
                            }
                            else
                            {
                                await stream.WriteStringAsync(((int)ResponseStatusCode.RESPONSEFORMATERROR).ToString());
                            }
                        }
                        catch (Exception e)
                        {
                            this._logger.LogError($"Failed to to digest message from: {clientIPAddress.ToString()}, Message: {message}\nException: {e.Message}");
                            await stream.WriteStringAsync(((int)ResponseStatusCode.ERROR).ToString());
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
