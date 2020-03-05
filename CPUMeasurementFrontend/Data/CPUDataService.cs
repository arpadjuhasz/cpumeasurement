using CPUMeasurementCommon.DataObjects;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CPUMeasurementFrontend.Data
{
    public class CPUDataService
    {
        //private readonly CPUDataHttpClient _httpClient;
        private readonly HttpClient _httpClient;
        private const string ENDPOINT = "/cpudata";

        public CPUDataService(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public async Task<List<CPUData>> GetCPUData()
        {
            return await this._httpClient.GetJsonAsync<List<CPUData>>(ENDPOINT);
        }

        public async Task<List<CPUData>> GetCPUDataByDate(DateTime date)
        {
            return await this._httpClient.GetJsonAsync<List<CPUData>>($"{ENDPOINT}?date={date.ToString("yyyy-MM-dd")}");
        }
    }
}
