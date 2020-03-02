using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CPUMeasurementService
{
    public class ClientConfigurationReader
    {
        private string _input { get; set; }

        private const string CLIENTCONFIGURATIONFILE = "client_configuration.json";

        public ClientConfiguration Configuration { get; set; }

        private ILogger<ClientConfigurationReader> _logger;

        public ClientConfigurationReader(ILogger<ClientConfigurationReader> logger)
        {
            this._logger = logger;
            this.CheckConfigurationExists();
            this.ReadConfiguration();
        }

        private void CheckConfigurationExists()
        {
            if (!File.Exists(CLIENTCONFIGURATIONFILE))
            {
                File.Create(CLIENTCONFIGURATIONFILE).Close();
                this.WriteDefaultSettings();
                this._logger.LogInformation($"{CLIENTCONFIGURATIONFILE} cretead with default settings!");
            }
        }

        private ClientConfiguration ReadConfiguration()
        {
            this._input = File.ReadAllText(CLIENTCONFIGURATIONFILE);
            try
            {
                var jobject = JObject.Parse(this._input);
                this.Configuration = jobject.ToObject<ClientConfiguration>();
            }
            catch (Exception e)
            {
                this.WriteDefaultSettings();
                this._logger.LogError("Configuration file was corrupted! All setting were overwritten to default values!");
                
            }
            return this.Configuration;
        }

        private void WriteDefaultSettings()
        {
            this.Configuration = new ClientConfiguration();
            var configurationText = JObject.FromObject(this.Configuration).ToString();
            File.WriteAllText(CLIENTCONFIGURATIONFILE, configurationText);
        }

        public void SetMeasurementInterval(int seconds)
        {
            this.Configuration.MeasurementIntervalInSeconds = seconds;
            OverwriteConfiguration();
        }

        private void OverwriteConfiguration()
        {
            var configurationText = JObject.FromObject(this.Configuration).ToString();
            File.WriteAllText(CLIENTCONFIGURATIONFILE, configurationText);
        }

    }
}
