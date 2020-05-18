using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.Repositories
{
   public abstract class Repository : IRepository
    {
        public string ConnectionString { get; set; }

    }

    public static class RepositoryExtensions
    {
        public static void AddAnd(this StringBuilder stringBuilder,bool paramAdded)
        {
            if (paramAdded)
            {
                stringBuilder.Append(" AND ");
            }
            else
            {
                stringBuilder.Append(" WHERE");
            }
        }

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

        public static object GetNullableDBObject(this object value)
        {
            if (value == null)
            {
                return (object)DBNull.Value;
            }
            else
            {
                return value;
            }
        }
    }
}
