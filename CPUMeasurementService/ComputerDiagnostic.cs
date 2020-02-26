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
        public Temperature Temperature { get; set; }
        public double? AverageLoad { get; set; }

        public string CPUName { get; set; }
        public string ComputerName { get; set; }
        private Computer Computer { get; set; }

        public ComputerDiagnostic()
        { }

        public CPUDataPacket GetCPUDataPacket()
        {
                this.Computer = new Computer();
            try
            {     
                this.Computer.Open();
                this.Computer.CPUEnabled = true;

                var cpu = Computer.Hardware.FirstOrDefault(x => x.HardwareType == HardwareType.CPU);

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
                this.Temperature = new Temperature(temperatureValues.Average(), MeasurementUnit.FAHRENHEIT);
                this.AverageLoad = loadValues.Average();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                this.Computer.Close();
            }
            CPUDataPacket packet = new CPUDataPacket();
            packet.Temperature = this.Temperature.InCelsius();
            packet.AverageLoad = this.AverageLoad;

            return packet;
        }

        public ClientPacket GetClientPacket()
        {
            this.Computer = new Computer();
            try
            {
                this.Computer.Open();
                this.Computer.CPUEnabled = true;

                var cpu = Computer.Hardware.FirstOrDefault(x => x.HardwareType == HardwareType.CPU);

                this.CPUName = cpu.Name;
                this.ComputerName = Environment.MachineName;
            }
            catch (Exception)
            { }
            finally
            {
                this.Computer.Close();
            }
            return new ClientPacket
            {
                ComputerName = this.ComputerName,
                CPUName = this.CPUName,
                IntervalInSeconds = 300
                
            
            };
            
        }
    }
}
