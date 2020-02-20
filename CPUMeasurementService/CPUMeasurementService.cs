using CPUMeasurementCommon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPUMeasurementService
{
    public class CPUMeasurementService : IHostedService
    {
        private Timer _timer;

        private readonly IConfiguration _configuration;
        private IPAddress _serverIPAddress;
        private short _serverPort;
        private readonly int _runIntervalInMinutes;

        public CPUMeasurementService(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._runIntervalInMinutes = this._configuration.GetValue<int>("runIntervalInMinutes");
            this._serverPort = this._configuration.GetValue<short>("serverPort");
            try
            {
                this._serverIPAddress = IPAddress.Parse(this._configuration.GetValue<string>("serverIPAddress"));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            if (this._runIntervalInMinutes <= 0)
            {
                this._runIntervalInMinutes = 5;
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._timer = new Timer(SendCPUMeasurementPacket, null, TimeSpan.Zero, TimeSpan.FromMinutes(this._runIntervalInMinutes));
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void SendCPUMeasurementPacket(object state)
        {
            var packet = this.GetCPUMeasurementPacket();
            try
            {
                string message = JObject.FromObject(packet).ToString();
                TcpClient client = new TcpClient(this._serverIPAddress.ToString(), this._serverPort);
                NetworkStream stream = client.GetStream();

                Byte[] data = Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);
                client.Dispose();
            }
            catch (Exception)
            { 
                // TCP client connection exception comes here.
            }
        }

        private CPUMeasurementPacket GetCPUMeasurementPacket()
        {
            CPUMeasurementPacket packet = new CPUMeasurementPacket();
            Computer computer = new Computer();

            try
            {
                computer.Open();
                computer.CPUEnabled = true;

                var cpu = computer.Hardware.FirstOrDefault(x => x.HardwareType == HardwareType.CPU);

                var loadValues = new List<double>();
                var temperatureValues = new List<double?>();

                foreach (var sensor in cpu.Sensors)
                {
                    switch (sensor.SensorType)
                    {
                        case SensorType.Load: loadValues.Add(sensor.Value.GetValueOrDefault()); break;
                        case SensorType.Temperature: temperatureValues.Add(sensor.Value); break;
                    }
                }
                packet.Temperature = temperatureValues.Average();
                packet.TemperatureMeasurementUnit = MeasurementUnit.FAHRENHEIT;
                packet.AverageLoad = loadValues.Average();
                
            }
            finally
            {
                computer.Close();
            }
            return packet;
        }
    }
}
