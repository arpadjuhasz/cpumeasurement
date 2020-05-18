using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.Repositories
{
    public interface IRepository
    {
        string ConnectionString { get; set; }
    }
}
