using CPUMeasurementBackend.Repository;
using CPUMeasurementCommon;
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

namespace CPUMeasurementBackend.Service
{
    public class ListenerService : IHostedService
    {
        private readonly IPAddress _ipAddress;
        private readonly TcpListener _tcpListener;
        private readonly short _port;
        private IConfiguration _configuration;
        private ILogger _logger;
        
        public ListenerService(IConfiguration configuration, ILogger<ListenerService> logger)
        {
            
            this._port = configuration.GetValue<short>("Port");
            this._ipAddress = IPAddress.Parse(configuration.GetValue<string>("IPAddress"));
            this._tcpListener = new TcpListener(_ipAddress, this._port);
            this._configuration = configuration;
            this._logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (true && !cancellationToken.IsCancellationRequested) { 
            await Task.Run(() =>  { this.Start(); });
            }

        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() => { this.Stop(); });
        }

        public async Task Start()
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
                        await clientTask.Result.GetStream().ReadAsync(buffer, 0, buffer.Length);
                        message = Encoding.ASCII.GetString(buffer);
                        var clientIPAddress = ((IPEndPoint)clientTask.Result.Client.RemoteEndPoint).Address;
                        try
                        {
                            var jsonObject = JObject.Parse(message);//.ToObject<CPUPacket>();
                            if (jsonObject != null)
                            {
                                CPUMeasurementPacket cpuPacket = jsonObject.ToObject<CPUMeasurementPacket>();
                            
                                var repository = new ListenerRepository(this._configuration);
                                await repository.SaveCPUPacket(cpuPacket, clientIPAddress);
                            }
                        }
                        catch (Exception e)
                        {
                            this._logger.LogError(string.Format($"Failed to save the data from client! IPAddress: {clientIPAddress}, Message: {message}", message, clientIPAddress.ToString()));
                            clientTask.Result.Client.Send(Encoding.ASCII.GetBytes("400"));
                        }
                        clientTask.Result.GetStream().Dispose();
                    }
                }

            }
        }

        public void Stop()
        {
            this._tcpListener.Stop();
        }
    }
}
