using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.Repositories
{
    public class RepositoryInitializer
    {
        public static void InitializeDatabase(string connectionString)
        {
            
            try
            {
                string[] sqls = File.ReadAllLines("DatabaseInit.sql");
                string sql = string.Empty;
                StringBuilder sb = new StringBuilder();
                foreach (var line in sqls)
                {
                    sb.AppendLine(line);
                }
                sql = sb.ToString();
                try
                {
                    using SqlConnection connection = new SqlConnection(connectionString);
                    var command = new SqlCommand(sql, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    command.Dispose();
                    connection.Close();
                    connection.Dispose();
                }
                catch (SqlException sqlException)
                { 
                    
                }
            }
            catch (IOException e)
            { 
            
            }
            
        }
    }
}
