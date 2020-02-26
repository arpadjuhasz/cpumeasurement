using CPUMeasurementBackend.HostedService;
using CPUMeasurementCommon;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.Repository
{
    public class ListenerRepository : IRepository
    {
        public string ConnectionString { get; set; }
        
        public  ListenerRepository(IConfiguration configuration)
        {
            this.ConnectionString = configuration.GetValue<string>("CPUMeasurementConnectionString");
        }

        public async Task<int> SaveCPUPacket(CPUDataPacket packet, IPAddress senderIpAddress)
        {
            using (var connection = new SqlConnection(this.ConnectionString))
            { 
                string sql = "INSERT INTO cpu_data (received, temperature, temperature_unit_id, average_load,  ip_address) VALUES (@received, @temperature, @temperature_unit_id, @average_load, @ip_address)";
                
                SqlCommand command = new SqlCommand(sql, connection);
                
                command.Parameters.AddWithValue("received", DateTime.UtcNow);
                command.Parameters.AddWithValue("temperature", packet.Temperature.Value);
                command.Parameters.AddWithValue("temperature_unit_id", (int)packet.Temperature.MeasurementUnit);
                command.Parameters.AddWithValue("average_load", packet.AverageLoad);
                command.Parameters.AddWithValue("ip_address", senderIpAddress.ToString());
                
                connection.Open();
                command.CommandText = sql;
                return await command.ExecuteNonQueryAsync();
            }
        }



        
    }
}
