using DonationHup.Domain.Common;
using DonationHup.Domain.Enum;
using DonationHup.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace DonationHup.Domain.Entites
{
    public class Donation : Entity
    {
        private readonly List<PaymentAttempt> _paymentAttempts = new();
        public Guid CampaignId { get; private set; }

        public Campaign Campaign { get; private set; }

        public Guid DonorUserId { get; private set; }

        public decimal Amount { get; private set; }

        public DonationStutes Stutes { get; private set; } = DonationStutes.Pending;


        public DateTime? CompletedAt { get; private set; }

        public string? FailureReason { get; private set; }
        public string? ReceiptNumber { get; private set; }
        public string? ReceiptFileName { get; private set; }

        public IReadOnlyCollection<PaymentAttempt> PaymentAttempts => _paymentAttempts;

        private Donation() { }


        public static Donation Create(
        Guid campaignId, Guid donorUserId,
        decimal amount, decimal minAmount, decimal maxAmount,
        DateTime nowUtc)
        {

            if (amount < minAmount || amount > maxAmount)
                throw new DomainException("Donation.AmountOutOfRange", $"The amount must be between {minAmount} and {maxAmount}.");

            return new Donation
            {
                CampaignId = campaignId,
                DonorUserId = donorUserId,
                Amount = amount,
                Stutes = DonationStutes.Pending,
                CreatedAt = nowUtc
            };
        }
        public void MarkSucceeded(string receiptNumber, DateTime nowUtc)
        {
            EnsurePending(); // invariant 8
            Stutes = DonationStutes.Succeeded;
            CompletedAt = nowUtc;
            ReceiptNumber = receiptNumber;
        }
        public void MarkFailed(string reason)
        {
            EnsurePending(); // invariant 8
            Stutes = DonationStutes.Failed;
            FailureReason = reason;
        }
        private void EnsurePending()
        {
            if (Stutes != DonationStutes.Pending)
                throw new DomainException("Donation.InvalidState", "Only a pending donation can change its state.");
        }
    }
}
