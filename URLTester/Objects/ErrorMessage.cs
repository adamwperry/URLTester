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
        public string message { get; set; }
        public bool criticalError { get; set; }

        public ErrorMessage(string _message, bool _criticalError = false)
        {
            message = _message;
            criticalError = _criticalError;
        }
    }
}
