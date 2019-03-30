using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Objects
{
    /// <summary>
    /// CRL error object for storage
    /// </summary>
    public class ErrorMessage
    {
        public string Message { get; set; }
        public bool CriticalError { get; set; }

        public ErrorMessage(string message, bool criticalError = false)
        {
            Message = message;
            CriticalError = criticalError;
        }
    }
}
