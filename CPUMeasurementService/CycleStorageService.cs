using CPUMeasurementCommon;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CPUMeasurementService
{
    public class CycleStorageService
    {
        private ConcurrentDictionary<DateTime, MeasurementPacket> Logs = new ConcurrentDictionary<DateTime, MeasurementPacket>();

        private const string LOGFILENAME = "Measurement.log";
        private readonly ILogger<CycleStorageService> _logger;

        public CycleStorageService(ILogger<CycleStorageService> logger)
        {
            _logger = logger;
            ReadLogsFromFile();
        }

        public void AddToCycleStorageLogs(MeasurementPacket measurement)
        {
            if (Logs.Count >= 100)
            {
                MeasurementPacket removed = null;
                Logs.TryRemove(this.Logs.Keys.Min(),out removed);
            }
            Logs.TryAdd(measurement.MeasurementDate, measurement.GetMemberwiseClone());
        }

        public void WriteLogsToFile()
        {
            var logs = Logs.Values.ToList().OrderByDescending(x => x.MeasurementDate);
            CheckFileExists();
            var logsInStringFormat = JsonConvert.SerializeObject(logs, Formatting.Indented);
            File.WriteAllText(LOGFILENAME, logsInStringFormat);
        }

        private void ReadLogsFromFile()
        {
            this.CheckFileExists();
            var logsInStringFormat = string.Empty;
            try
            {
                logsInStringFormat = File.ReadAllText(LOGFILENAME);
                try
                {
                    var logs = JsonConvert.DeserializeObject<List<MeasurementPacket>>(logsInStringFormat);
                    foreach (var log in logs)
                    {
                        this.AddToCycleStorageLogs(log);
                    }
                }
                catch (Exception e)
                {
                    this._logger.LogError($"Unable to parse the content of the {LOGFILENAME} Exception: \n{e.Message}");
                }
            }
            catch (Exception e)
            {
                this._logger.LogError($"Unable to read from the {LOGFILENAME}. Please check it! Exception: \n{e.Message}");
            }
        }

        private bool CheckFileExists()
        {
            if (!File.Exists(LOGFILENAME))
            {
                File.Create(LOGFILENAME).Close();
                File.WriteAllText(LOGFILENAME, "[]");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
