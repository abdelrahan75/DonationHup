using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Common.Exceptions
{
    public class AppException : Exception
    {
        public string Code { get; }
        public int StatusCode { get; }

        public AppException(string code, string message, int statusCode) : base(message)
        {
            Code = code;
            StatusCode = statusCode;
        }
    }
}
