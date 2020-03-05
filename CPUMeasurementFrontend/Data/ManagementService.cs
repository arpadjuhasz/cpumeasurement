using CPUMeasurementCommon.DataObjects;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CPUMeasurementFrontend.Data
{
    public class ManagementService
    {
        private readonly HttpClient _httpClient;
        private const string APIPATH = "/management";

        public ManagementService(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public async Task<List<ClientData>> GetConnectedClients()
        {
            return await this._httpClient.GetJsonAsync<List<ClientData>>(APIPATH);
        }

        public async Task UpdateMeasurementInterval(string clientIPAddress, int measurementInterval)
        {
            var dto = new MeasurementIntervalUpdate { MeasurementIntervalInSeconds = measurementInterval };
            await this._httpClient.PutJsonAsync($"{APIPATH}/client/{clientIPAddress}", dto);
        }
    }
}
