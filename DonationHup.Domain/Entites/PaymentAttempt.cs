using DonationHup.Domain.Common;
using DonationHup.Domain.Enum;
using DonationHup.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Domain.Entites
{
 
    public class PaymentAttempt: Entity
    {

        public Guid DonationId { get; private set; }
        public string StripePaymentIntentId { get; private set; }
        public string? StripeCheckoutSessionId  { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentAttemptStatus Status { get; private set; } = PaymentAttemptStatus.Created;

        public DateTime UpdatedAt { get; private set; }

        public string? FailureCode { get; private set; } 
        private PaymentAttempt() { } 
        public static PaymentAttempt Create(Guid donationId, string checkoutSessionId, decimal amount, DateTime nowUtc)
        {
            return new PaymentAttempt
            {
                DonationId = donationId,
                StripeCheckoutSessionId = checkoutSessionId,
                Amount = amount,
                Status = PaymentAttemptStatus.Created,
                CreatedAt = nowUtc,
                UpdatedAt = nowUtc
            };
        }
        public void MarkSucceeded(string? paymentIntentId, DateTime nowUtc)
        {
            EnsureCreated();
            Status = PaymentAttemptStatus.Succeeded;
            StripePaymentIntentId = paymentIntentId;
            UpdatedAt = nowUtc;
        }
        public void MarkFailed(string? failureCode, DateTime nowUtc)
        {
            EnsureCreated();
            Status = PaymentAttemptStatus.Failed;
            FailureCode = failureCode;
            UpdatedAt = nowUtc;
        }
        public void MarkExpired(DateTime nowUtc)
        {
            EnsureCreated();
            Status = PaymentAttemptStatus.Expired;
            UpdatedAt = nowUtc;
        }
        private void EnsureCreated()
        {
            if (Status != PaymentAttemptStatus.Created)
                throw new DomainException("Donation.InvalidState", "This payment attempt is already final.");
        }



    }
}
