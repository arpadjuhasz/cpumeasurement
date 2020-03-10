using CPUMeasurementCommon.DataObjects;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CPUMeasurementFrontend.Data
{
    public class MeasurementService
    {
        private readonly HttpClient _httpClient;
        private const string ENDPOINT = "/measurement";

        public MeasurementService(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public async Task<List<CPUData>> GetCPUData()
        {
            return await this._httpClient.GetJsonAsync<List<CPUData>>(ENDPOINT);
        }

        public async Task<List<CPUData>> GetCPUDataByDate(DateTime? date, string ipAddress)
        {
            string request = $"{ENDPOINT}"
                + (date.HasValue || !string.IsNullOrWhiteSpace(ipAddress) ? "?" : string.Empty)
                + (date.HasValue ? $"date={date.Value.ToString("yyyy-MM-dd")}" : string.Empty)
                + (date.HasValue && !string.IsNullOrWhiteSpace(ipAddress) ? $"&" : string.Empty)
                + (!string.IsNullOrWhiteSpace(ipAddress) ? $"ipAddress={ipAddress}" : string.Empty);
            return await this._httpClient.GetJsonAsync<List<CPUData>>(request);
        }
    }
}
