using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace CPUMeasurementFrontend.Data
{
    public class CPUDataHttpClient : HttpClient
    {
        private readonly IConfiguration _configuration;
        private readonly string _hostName;

        public CPUDataHttpClient(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._hostName = this._configuration.GetValue<string>("HostName");
        }

        public async Task<T> GetJsonAsyncc<T>(string apiPath, string queryParams = "")
        {
            
            return await this.GetJsonAsync<T>($"{_hostName}{apiPath}{queryParams}");
        }

        public async Task<T> PostJsonAsyncc<T>(string apiPath, object post)
        {

            return await this.PostJsonAsync<T>($"{_hostName}{apiPath}", post);
        }
    }
}
