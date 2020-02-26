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
    public class ManagementListenerService : IHostedService
    {
        private readonly IPAddress _ipAddress;
        private readonly TcpListener _tcpListener;
        private readonly short _port;
        private IConfiguration _configuration;
        private ILogger _logger;
        private readonly Management _management;

        public ManagementListenerService(IConfiguration configuration, ILogger<ManagementListenerService> logger, Management management)
        {

            this._port = configuration.GetValue<short>("ManagementPort");
            this._ipAddress = IPAddress.Parse(configuration.GetValue<string>("IPAddress"));
            this._tcpListener = new TcpListener(_ipAddress, this._port);
            this._configuration = configuration;
            this._logger = logger;
            this._management = management;
            this._management.ConnectedClients.TryAdd(this._ipAddress, new ClientData { ComputerName = "ez", CPUName = "amd", IntervalInSeconds = 354, MAC = "foo" });
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (true && !cancellationToken.IsCancellationRequested)
            {
                await Task.Run(() => { this.Start(); });
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
                            var jsonObject = JObject.Parse(message);
                            if (jsonObject != null)
                            {
                                ClientPacket clientPacket = jsonObject.ToObject<ClientPacket>();
                                ClientData clientData = new ClientData
                                {
                                    CPUName = clientPacket.CPUName,
                                    IntervalInSeconds = clientPacket.IntervalInSeconds,
                                    //IPAddress = clientIPAddress,
                                    ComputerName = clientPacket.ComputerName
};
                                this._management.ConnectedClients.TryAdd(clientIPAddress, clientData);
                            }
                        }
                        catch (Exception)
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