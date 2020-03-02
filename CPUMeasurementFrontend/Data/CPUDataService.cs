using CPUMeasurementCommon.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPUMeasurementFrontend.Data
{
    public class CPUDataService
    {
        private readonly CPUDataHttpClient _httpClient;
        private const string ENDPOINT = "/cpudata";

        public CPUDataService(CPUDataHttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public async Task<List<CPUData>> GetCPUData()
        {
            return await this._httpClient.GetJsonAsyncc<List<CPUData>>(ENDPOINT);
        }

        public async Task<List<CPUData>> GetCPUDataByDate(DateTime date)
        {
            return await this._httpClient.GetJsonAsyncc<List<CPUData>>(ENDPOINT, $"?date={date.ToString("yyyy-MM-dd")}");
        }
    }
}
