using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CPUMeasurementService
{
    public class CancelService
    {
        public CancellationToken CancelationToken { get; set; }

        public CancelService()
        {
            this.CancelationToken = new CancellationToken();
        }

        public void Renew()
        {
            if (this.CancelationToken.IsCancellationRequested)
            { 
                this.CancelationToken = new CancellationToken();
            }
        }
    }
}
