using CPUMeasurementBackend.Repository.Poco;
using CPUMeasurementCommon;
using CPUMeasurementCommon.DataObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.Repository
{
    public class CPUDataRepository : Repository
    {
        public CPUDataRepository(IConfiguration configuration)
        {
            this.ConnectionString = configuration.GetValue<string>("CPUMeasurementConnectionString");
            
        }

        public async Task<List<CPUData>> GetCPUData(DateTime? date)
        {
            List<CPUData> result = new List<CPUData>();
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                var sql = @"SELECT TOP (100) [id]
      ,[Received]
      ,[Temperature]
      ,[TemperatureMeasurementUnit]
      ,[AverageLoad]
      ,[IPAddress]
,[MeasurementDate]
,[MeasurementIntervalInSeconds]
  FROM [cpu_measurement].[dbo].[cpu_data] " + (date.HasValue ? " WHERE Received BETWEEN @dateFrom AND @dateTo" : string.Empty) + " ORDER BY received DESC";
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

        public async Task<int> SaveCPUPacket(CPUData cpuData)
        {
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                string sql = "INSERT INTO cpu_data (Received, Temperature, TemperatureMeasurementUnit, AverageLoad,  IPAddress, MeasurementDate, MeasurementIntervalInSeconds) VALUES (@Received, @Temperature, @TemperatureMeasurementUnit, @AverageLoad, @IPAddress, @MeasurementDate, @MeasurementIntervalInSeconds)";

                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("Received",cpuData.Received);
                command.Parameters.AddWithValue("Temperature", cpuData.Temperature.Value);
                command.Parameters.AddWithValue("TemperatureMeasurementUnit", (int)cpuData.Temperature.MeasurementUnit);
                command.Parameters.AddWithValue("AverageLoad", cpuData.AverageLoad);
                command.Parameters.AddWithValue("IPAddress", cpuData.IPAddress);
                command.Parameters.AddWithValue("MeasurementDate", cpuData.MeasurementDate);
                command.Parameters.AddWithValue("MeasurementIntervalInSeconds", cpuData.MeasurementIntervalInSeconds);

                connection.Open();
                command.CommandText = sql;
                return await command.ExecuteNonQueryAsync();
            }

        }
    }
}
