using System;
using System.Collections.Generic;
using System.Text;

namespace CPUMeasurementCommon
{
    public enum ResponseStatusCode
    { 
        ERROR = 400,
        SUCCESS = 200 ,
        RESPONSEFORMATERROR = 501
    }

    public class ResponseStatus
    {
        public static ResponseStatusCode GetReponseStatusCode(string statusText)
        {
            int code;
            if (int.TryParse(statusText, out code))
            {
                return (ResponseStatusCode)code;
            }
            else
            { 
                return ResponseStatusCode.RESPONSEFORMATERROR;
            }
        }
    }
}
