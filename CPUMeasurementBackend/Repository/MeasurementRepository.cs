
using CPUMeasurementBackend.HostedService;
using CPUMeasurementCommon;
using CPUMeasurementCommon.DataObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.Repository
{
    public class MeasurementRepository : Repository
    {
        public MeasurementRepository(IConfiguration configuration)
        {
            this.ConnectionString = configuration.GetValue<string>("CPUMeasurementConnectionString");
        }

        internal List<MeasurementData> GetMeasurementData(DateTime? date, string ipAddress)
        {
            List<MeasurementData> result = new List<MeasurementData>();
            var paramAdded = false;
            using var connection = new SqlConnection(this.ConnectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT");
            if (!date.HasValue && string.IsNullOrWhiteSpace(ipAddress))
            {
                sqlBuilder.Append(" TOP (100) ");
            }
            sqlBuilder.Append("[Id],[Received],[Temperature],[TemperatureMeasurementUnit],[AverageLoad],[IPAddress],[MeasurementIntervalInSeconds],[MeasurementDate] FROM [dbo].[cpu_data] ");
            if (date.HasValue)
            {
                sqlBuilder.AddAnd(paramAdded);
                sqlBuilder.Append(" MeasurementDate BETWEEN @dateFrom AND @dateTo");
                command.Parameters.AddWithValue("dateFrom", date.Value.ToUniversalTime());
                command.Parameters.AddWithValue("dateTo", date.Value.AddDays(1).ToUniversalTime());
                paramAdded = true;
            }

            if (!string.IsNullOrWhiteSpace(ipAddress))
            {
                sqlBuilder.AddAnd(paramAdded);
                sqlBuilder.Append(" IPAddress = @IPAddress");
                command.Parameters.AddWithValue("IPAddress", ipAddress);
                paramAdded = true;

            }

            sqlBuilder.Append(" ORDER BY Received DESC");
            command.CommandText = sqlBuilder.ToString();
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var item = new MeasurementData
                {
                    Id = reader.GetInt32(0),
                    Received = reader.GetDateTime(1),
                    Temperature = new Temperature(reader.GetNullableFieldValue<double?>(2), (MeasurementUnit)reader.GetInt32(3)),
                    AverageLoad = reader.GetNullableFieldValue<double?>(4),
                    IPAddress = reader.GetString(5),
                    MeasurementIntervalInSeconds = reader.GetInt32(6),
                    MeasurementDate = reader.GetDateTime(7)
                };
                result.Add(item);
            }
            reader.Close();
            reader.Dispose();
            command.Dispose();
            connection.Close();
            connection.Dispose();
            return result;
        }

        internal int? AddMeasurementData(MeasurementData measurementData, ILogger<MeasurementListener> logger)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(ConnectionString);
                string sql = @"INSERT INTO cpu_data 
                            (Received, Temperature, TemperatureMeasurementUnit, AverageLoad,  IPAddress, MeasurementDate, MeasurementIntervalInSeconds) 
                            VALUES 
                            (@Received, @Temperature, @TemperatureMeasurementUnit, @AverageLoad, @IPAddress, @MeasurementDate, @MeasurementIntervalInSeconds)";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("Received", measurementData.Received);

                if (measurementData.Temperature == null)
                {
                    measurementData.Temperature = new Temperature(null, MeasurementUnit.CELSIUS);
                }
                command.Parameters.AddWithValue("Temperature", measurementData.Temperature.Value.GetNullableDBObject());
                command.Parameters.AddWithValue("TemperatureMeasurementUnit", ((int)measurementData.Temperature.MeasurementUnit).GetNullableDBObject());
                command.Parameters.AddWithValue("AverageLoad", measurementData.AverageLoad.Value.GetNullableDBObject());
                command.Parameters.AddWithValue("IPAddress", measurementData.IPAddress);
                command.Parameters.AddWithValue("MeasurementDate", measurementData.MeasurementDate);
                command.Parameters.AddWithValue("MeasurementIntervalInSeconds", measurementData.MeasurementIntervalInSeconds);

                connection.Open();

                int id = command.ExecuteNonQuery();
                command.Dispose();
                connection.Close();
                connection.Dispose();
                logger.LogInformation($"Measurement is successfully saved from {measurementData.IPAddress}.");
                return id;
            }
            catch (Exception e)
            {
                logger.LogError($"Failed to save data! {measurementData.IPAddress}. Error: \n{e.Message}");
                return null;
            }

        }
    }
}
