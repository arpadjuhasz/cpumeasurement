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
    public class ManagementListener : IHostedService
    {
        private readonly IPAddress _ipAddress;
        private readonly short _managementPort;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly Management _management;
        

        public ManagementListener(IConfiguration configuration, ILogger<ManagementListener> logger, Management management)
        {

            this._managementPort = configuration.GetValue<short>("ManagementPort");
            this._ipAddress = IPAddress.Parse(configuration.GetValue<string>("IPAddress"));
            this._configuration = configuration;
            this._logger = logger;
            this._management = management;
            
        }

        

        public async Task ReceiveClientPacket()
        {
            UdpClient listener = new UdpClient(this._managementPort);
                IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, this._managementPort);

                try
                {
                    var bytes = listener.Receive(ref groupEP);
                var message = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    var jsonObject = JObject.Parse(message);
                if (jsonObject != null)
                {
                    ClientPacket clientPacket = jsonObject.ToObject<ClientPacket>();
                    ClientData clientData = new ClientData
                    {
                        IPv4Address = groupEP.Address.ToString(),
                        CPUName = clientPacket.CPUName,
                        RunIntervalInSeconds = clientPacket.MeasurementIntervalInSeconds,
                        ComputerName = clientPacket.ComputerName,
                        LastUpdate = DateTime.UtcNow,
                    };
                    if (!this._management.ConnectedClients.TryAdd(groupEP.Address, clientData))
                    {
                        ClientData comparisonClientData = null;
                        this._management.ConnectedClients.TryGetValue(groupEP.Address, out comparisonClientData);
                        this._management.ConnectedClients.TryUpdate(groupEP.Address, clientData, comparisonClientData);
                    }

                        Console.WriteLine($"{message}");

                    }
                    else
                    {
                    this._logger.LogWarning("Malformed JSON!");
                    }
                }
                catch (SocketException e)
                {
                this._logger.LogError($"Unable to update {groupEP}");
            }
                finally
                {
                    listener.Close();
                }
            }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (true)
            await Task.Run(() => { this.ReceiveClientPacket(); });
            
            //await Task.CompletedTask
        }

        public void Stop()
        {
            //this._tcpListener.Stop();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        
    }
}