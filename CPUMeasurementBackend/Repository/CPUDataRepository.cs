using CPUMeasurementBackend.Repository.Poco;
using CPUMeasurementCommon;
using CPUMeasurementCommon.DataObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.Repository
{
    public class CPUDataRepository : Repository
    {
        private readonly ILogger<CPUDataRepository> _logger;

        public CPUDataRepository(IConfiguration configuration, ILogger<CPUDataRepository> logger)
        {
            this.ConnectionString = configuration.GetValue<string>("CPUMeasurementConnectionString");
            this._logger = logger;
        }

        public async Task<List<CPUData>> GetCPUData(DateTime? date)
        {
            List<CPUData> result = new List<CPUData>();
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                var sql = @"SELECT TOP (100) [id]
      ,[received]
      ,[temperature]
      ,[temperature_unit_id]
      ,[average_load]
      ,[ip_address]
  FROM [cpu_measurement].[dbo].[cpu_data] " + (date.HasValue ? " WHERE received BETWEEN @dateFrom AND @dateTo" : string.Empty) + " ORDER BY received DESC";
                SqlCommand command = new SqlCommand(sql, connection);
                if (date.HasValue)
                { 
                    command.Parameters.AddWithValue("dateFrom", date.Value.ToUniversalTime());
                    command.Parameters.AddWithValue("dateTo", date.Value.AddDays(1).ToUniversalTime());
                }
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    CPUData item = new CPUData
                    {
                        Id = reader.GetInt32(0),
                        Received = reader.GetDateTime(1),
                        Temperature = new Temperature(reader.GetNullableFieldValue<double?>(2), (MeasurementUnit)reader.GetInt32(3)),
                        AverageLoad = reader.GetNullableFieldValue<double?>(4),
                        IPAddress = reader.GetString(5)
                    };
                    result.Add(item);
                }
                return result;
            }
        }

    }
}
