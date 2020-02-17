using Microsoft.Extensions.Hosting;
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
    public class CPUMeasurementListener : IHostedService
    {
        private readonly IPAddress IPAddress = IPAddress.Parse("192.168.0.16");
        private readonly TcpListener TcpListener;
        private readonly short Port;

        public CPUMeasurementListener()
        {
            this.Port = 1337;
            this.TcpListener = new TcpListener(IPAddress, this.Port);
            
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            /*while (true && !cancellationToken.cancellationToken)
            {
                Console.WriteLine("Foo Service");
            }*/
            while (true && !cancellationToken.IsCancellationRequested) { 
            await Task.Run(() => { this.Start(); });
            }

        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() => { this.Stop(); });
        }

        public void Start()
        {
            this.TcpListener.Start();
            Console.WriteLine($"Listening on {IPAddress}:{Port}", IPAddress.ToString(), this.Port);
            while (true)
            {
                var clientTask = TcpListener.AcceptTcpClientAsync();
                string message = String.Empty;
                if (clientTask.Result != null)
                {
                    while (message != null)
                    {
                        byte[] buffer = new byte[1024];
                        clientTask.Result.GetStream().Read(buffer, 0, buffer.Length);
                        message = Encoding.ASCII.GetString(buffer);

                        try
                        {
                            var jsonObject = JObject.Parse(message);//.ToObject<CPUPacket>();
                            if (jsonObject != null)
                            {
                                CPUMeasurementPacket cpuPacket = jsonObject.ToObject<CPUMeasurementPacket>();
                            }

                            Console.WriteLine(message);
                        }
                        catch (Exception)
                        {
                            clientTask.Result.Client.Send(Encoding.ASCII.GetBytes("400"));
                        }
                    }
                    clientTask.Result.GetStream().Dispose();
                }

            }
        }

        public void Stop()
        {
            this.TcpListener.Stop();
        }
    }
}
