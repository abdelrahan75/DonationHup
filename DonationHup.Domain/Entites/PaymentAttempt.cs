using DonationHup.Domain.Enum;
using DonationHup.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Domain.Entites
{
    public class PaymentAttempt:AuditableEntity
    {

        public Guid DonationId { get; private set; }
        public string? StripePaymentIntentId { get; private set; }
        public string? StripeCheckoutSessionId  { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentAttemptStatus Stutes { get; private set; } = PaymentAttemptStatus.Created;
        public static PaymentAttempt Create(Guid donationId, string stripeCheckoutSessionId, decimal amount) => new()
        {
            Id = Guid.NewGuid(),
            DonationId = donationId,
            StripeCheckoutSessionId = stripeCheckoutSessionId,
            Amount = amount,
            Stutes = PaymentAttemptStatus.Created
        };
        public void SetPaymentIntent(string paymentIntentId) => StripePaymentIntentId = paymentIntentId;
        public void MarkSucceeded() => Finish(PaymentAttemptStatus.Succeeded);
        public void MarkFailed() => Finish(PaymentAttemptStatus.Failed);
        public void MarkExpired() => Finish(PaymentAttemptStatus.Expired);

        private void Finish(PaymentAttemptStatus status)
        {
            if (Stutes != PaymentAttemptStatus.Created)
                throw new DomainException("Payment attempt is already in a final state.");

            Stutes = status;
        }



    }
}
