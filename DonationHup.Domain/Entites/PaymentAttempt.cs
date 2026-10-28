using DonationHup.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Domain.Entites
{
    public class PaymentAttempt:AuditableEntity
    {

        public int DonationId { get; set; }

        public int StripeCheckoutSessionId  { get; set; }

        public int Amount { get; set; }

        public PaymentAttemptStatus Stutes { get; set; } = PaymentAttemptStatus.Created;



    }
}
