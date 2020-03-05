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
        public ClientPacket ClientPacket { get; set; }

        public MeasurementPacket CPUDataPacket { get; set; }
        
        public ComputerDiagnostic()
        {
            this.CPUDataPacket = new MeasurementPacket();
            this.ClientPacket = new ClientPacket();
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
                CPUDataPacket.Temperature = new Temperature(temperatureValues.Average(), MeasurementUnit.FAHRENHEIT).InCelsius();
                CPUDataPacket.AverageLoad = loadValues.Average();
                CPUDataPacket.MeasurementDate = DateTime.UtcNow;
                

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
