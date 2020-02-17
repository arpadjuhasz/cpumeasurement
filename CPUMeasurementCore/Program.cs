using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPUMeasurementCore
{
    class Program
    {
        
            static async Task Main(string[] args)
            {
                Console.WriteLine("Read system information..." + Environment.NewLine);
                Console.WriteLine("Results:");
                SystemInfo systemInfo = await ReadSystemInfoAsync();

                foreach (SystemInfo.CoreInfo cInfo in systemInfo.CoreInfos)
                {
                    Console.WriteLine($"Name: {cInfo.Name} - {cInfo.Load} % - {cInfo.Temp} �C");
                }

                Console.WriteLine("Done.");
                Console.ReadKey();
            }

            public static async Task<SystemInfo> ReadSystemInfoAsync()
            {
                return await Task.Run(() =>
                {
                    SystemInfo systemInfo = new SystemInfo();

                    SystemVisitor updateVisitor = new SystemVisitor();
                    Computer computer = new Computer();

                    try
                    {
                        computer.Open();
                        computer.CPUEnabled = true;

                        computer.Accept(updateVisitor);

                        foreach (IHardware hw in computer.Hardware
                            .Where(hw => hw.HardwareType == HardwareType.CPU))
                        {
                            foreach (ISensor sensor in hw.Sensors)
                            {
                                switch (sensor.SensorType)
                                {
                                    case SensorType.Load:
                                        systemInfo.AddOrUpdateCoreLoad(
                                        sensor.Name, sensor.Value.GetValueOrDefault(0));

                                        break;
                                    case SensorType.Temperature:
                                        systemInfo.AddOrUpdateCoreTemp(
                                        sensor.Name, sensor.Value.GetValueOrDefault(0));

                                        break;
                                }
                            }
                        }
                    }
                    finally
                    {
                        computer.Close();
                    }

                    return systemInfo;
                });
            }
        }

        public class SystemInfo
        {
            public class CoreInfo
            {
                public string Name { get; set; }
                public double Load { get; set; }
                public double Temp { get; set; }
            }

            public List<CoreInfo> CoreInfos = new List<CoreInfo>();

            private CoreInfo GetCoreInfo(string name)
            {
                CoreInfo coreInfo = CoreInfos.SingleOrDefault(c => c.Name == name);
                if (coreInfo is null)
                {
                    coreInfo = new CoreInfo { Name = name };
                    CoreInfos.Add(coreInfo);
                }

                return coreInfo;
            }

            public void AddOrUpdateCoreTemp(string name, double temp)
            {
                CoreInfo coreInfo = GetCoreInfo(name);
                coreInfo.Temp = temp;
            }

            public void AddOrUpdateCoreLoad(string name, double load)
            {
                CoreInfo coreInfo = GetCoreInfo(name);
                coreInfo.Load = load;
            }
        }

        public class SystemVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer) { computer.Traverse(this); }

            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
            }

            public void VisitSensor(ISensor sensor) { }
            public void VisitParameter(IParameter parameter) { }
        }
    }
