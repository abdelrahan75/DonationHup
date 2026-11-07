using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Common.Exceptions
{
    public class AuthException : AppException
    {
        public AuthException(string code, string message, int statusCode) : base(code, message, statusCode)
        {

       
        }


    }

}
