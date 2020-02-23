using CPUMeasurementBackend.Repository.Poco;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.Repository
{
    public class CPUDataRepository : IRepository
    {
        public string ConnectionString { get; set; }
        private readonly IConfiguration _configuration;
        private readonly ILogger<CPUDataRepository> _logger;

        public CPUDataRepository(IConfiguration configuration, ILogger<CPUDataRepository> logger)
        {
            this.ConnectionString = configuration.GetValue<string>("CPUMeasurementConnectionString");
            this._logger = logger;
        }

        public async Task<List<CPUData>> GetCPUData()
        {
            List<CPUData> result = new List<CPUData>();
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                var sql = @"SELECT TOP (1000) [id]
      ,[received]
      ,[temperature]
      ,[temperature_unit_id]
      ,[average_load]
      ,[ip_address]
  FROM [cpu_measurement].[dbo].[cpu_data]";
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    CPUData item = new CPUData
                    {
                        id = reader.GetInt32(0),
                        received = reader.GetDateTime(1),
                        temperature = reader.GetDouble(2),
                        temperature_unit_id = reader.GetInt32(3),
                        average_load = reader.GetDouble(4),
                        ip_address = reader.GetString(5)
                    };
                    result.Add(item);
                }
                return result;
            }
        }
    }
}
