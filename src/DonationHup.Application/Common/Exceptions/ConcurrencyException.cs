using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Common.Exceptions
{
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(Exception inner) : base("A concurrency conflict was detected.", inner) { }
    }
}
