using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.Repository
{
    public abstract class Repository : IRepository
    {
        public string ConnectionString { get; set; }
    }

    public static class RepositoryExtensions
    {
        public static T GetNullableFieldValue<T>(this SqlDataReader sqlDataReader, int index)
        {
            var fieldValue = sqlDataReader.GetValue(index);
            if (fieldValue == DBNull.Value)
            {
                return default(T);
            }
            else
            {
                return (T)fieldValue;
            }
        }
    }
}
