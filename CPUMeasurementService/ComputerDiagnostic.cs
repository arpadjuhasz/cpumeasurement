using CPUMeasurementCommon;
using CPUMeasurementCommon.Management;
using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPUMeasurementService
{
    public class ComputerDiagnostic
    {
        public ManagementPacket ClientPacket { get; set; }

        public MeasurementPacket MeasurementPacket { get; set; }
        
        public ComputerDiagnostic()
        {
            this.MeasurementPacket = new MeasurementPacket();
            this.ClientPacket = new ManagementPacket();
            this.Update();
        }

        public ComputerDiagnostic Update()
        {
            Computer computer = new Computer();
            try
            {
                computer.CPUEnabled = true;
                computer.Open();


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
                MeasurementPacket.Temperature = new Temperature(temperatureValues.Average(), MeasurementUnit.FAHRENHEIT).InCelsius();
                MeasurementPacket.AverageLoad = loadValues.Average();
                MeasurementPacket.MeasurementDate = DateTime.UtcNow;
                MeasurementPacket.MeasurementIntervalInSeconds = this.ClientPacket.MeasurementIntervalInSeconds;
                
                this.ClientPacket.CPUName = cpu.Name;
                this.ClientPacket.ComputerName = Environment.MachineName;
            }
            finally
            {
                computer.Close();
            }
            return this;
        }
    }
}
