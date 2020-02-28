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
        private readonly short _managementPort;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly Management _management;
        

        public ManagementListenerService(IConfiguration configuration, ILogger<ManagementListenerService> logger, Management management)
        {

            this._managementPort = configuration.GetValue<short>("ManagementPort");
            this._ipAddress = IPAddress.Parse(configuration.GetValue<string>("IPAddress"));
            this._configuration = configuration;
            this._logger = logger;
            this._management = management;
            
        }

        

        public async Task ReceiveClientPacket()
        {
            //this._tcpListener.Start();
            //while (true)
            //{
                //var clientTask = _tcpListener.AcceptTcpClientAsync();
                //                string message = String.Empty;
                //                if (clientTask.Result != null)
                //                {
                //                    while (message != null)
                //                    {
                //                        byte[] buffer = new byte[1024];
                //                        await clientTask.Result.GetStream().ReadAsync(buffer, 0, buffer.Length);
                //                        message = Encoding.ASCII.GetString(buffer);
                //                        var clientIPAddress = ((IPEndPoint)clientTask.Result.Client.RemoteEndPoint).Address;
                //                        try
                //                        {
                //                            var jsonObject = JObject.Parse(message);
                //                            if (jsonObject != null)
                //                            {
                //                                ClientPacket clientPacket = jsonObject.ToObject<ClientPacket>();
                //                                ClientData clientData = new ClientData
                //                                {
                //                                    CPUName = clientPacket.CPUName,
                //                                    IntervalInSeconds = clientPacket.IntervalInSeconds,
                //                                    //IPAddress = clientIPAddress,
                //                                    ComputerName = clientPacket.ComputerName
                //};
                //                                this._management.ConnectedClients.TryAdd(clientIPAddress, clientData);
                //                            }
                //                        }
                //                        catch (Exception)
                //                        {
                //                            this._logger.LogError(string.Format($"Failed to save the data from client! IPAddress: {clientIPAddress}, Message: {message}", message, clientIPAddress.ToString()));
                //                            clientTask.Result.Client.Send(Encoding.ASCII.GetBytes("400"));
                //                        }
                //                        clientTask.Result.GetStream().Close();//GetStream().Dispose();
                //                    }
                //                }

                //            }
                UdpClient listener = new UdpClient(this._managementPort);
                IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, this._managementPort);

                try
                {
                    
                        Console.WriteLine("Waiting for broadcast");
                        var bytes = listener.Receive(ref groupEP);
                 

                        Console.WriteLine($"Received broadcast from {groupEP} :");
                        Console.WriteLine($" {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");
                    
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e);
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