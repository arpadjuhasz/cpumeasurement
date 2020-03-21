using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CPUMeasurementCommon
{
    public static class NetworkCommunication
    {
        public static async Task WriteStringAsync(this NetworkStream networkStream, string message)
        {
            var bytes = Encoding.ASCII.GetBytes(message);
            await networkStream.WriteAsync(bytes, 0, bytes.Length);
        }

        public static void WriteString(this NetworkStream networkStream, string message)
        {
            var bytes = Encoding.ASCII.GetBytes(message);
            networkStream.Write(bytes, 0, bytes.Length);
        }
    }
}
