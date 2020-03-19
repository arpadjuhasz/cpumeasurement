using CPUMeasurementCommon.DataObjects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        public async Task<List<MeasurementData>> GetMeasurementData(DateTime? date, string ipAddress)
        {
            string request = $"{ENDPOINT}"
                + (date.HasValue || !string.IsNullOrWhiteSpace(ipAddress) ? "?" : string.Empty)
                + (date.HasValue ? $"date={date.Value.ToString("yyyy-MM-dd")}" : string.Empty)
                + (date.HasValue && !string.IsNullOrWhiteSpace(ipAddress) ? $"&" : string.Empty)
                + (!string.IsNullOrWhiteSpace(ipAddress) ? $"ipAddress={ipAddress}" : string.Empty);
            var result = await this._httpClient.GetAsync(request);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<List<MeasurementData>>(await result.Content.ReadAsStringAsync());
            }
            return null;
        }
    }
}
