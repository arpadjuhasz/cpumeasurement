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

        internal void Renew()
        {
            this.CancelationToken = new CancellationToken();
        }
    }
}
