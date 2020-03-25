using System.Threading;

namespace CPUMeasurementService
{
    public class CancelService
    {
        public CancellationToken CancelationToken { get; set; }

        public CancelService()
        {
            CancelationToken = new CancellationToken();
        }

        public void Renew()
        {
            if (this.CancelationToken.IsCancellationRequested)
            { 
                CancelationToken = new CancellationToken();
            }
        }
    }
}
