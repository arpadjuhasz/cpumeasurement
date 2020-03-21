using CPUMeasurementCommon;
using CPUMeasurementCommon.DataObjects;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CPUMeasurementService
{
    public class CycleStorageService
    {
        private ConcurrentDictionary<DateTime, MeasurementPacket> Logs = new ConcurrentDictionary<DateTime, MeasurementPacket>();

        private const string LOGFILENAME = "Measurement.log";
        private readonly ILogger<CycleStorageService> _logger;

        public CycleStorageService(ILogger<CycleStorageService> logger)
        {
            this._logger = logger;
            this.ReadLogsFromFile();
        }

        public void AddToLogs(DateTime key, MeasurementPacket measurement)
        {
            if (Logs.Count >= 100)
            {
                MeasurementPacket removed = null;
                this.Logs.TryRemove(this.Logs.Keys.Min(),out removed);
            }
            Logs.TryAdd(key, measurement);
        }

        public void WriteLogsToFile()
        {
            var logs = Logs.Values.ToList().OrderByDescending(x => x.MeasurementDate);
            this.CheckFileExists();
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
                        this.AddToLogs(log.MeasurementDate, log);
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
