using DonationHup.Domain.Enum;
using DonationHup.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Domain.Entites
{
    public class Donation : AuditableEntity
    {
        

        public Guid CampaignId { get; private set; }

        public Guid DonorUserId { get; private set; }

        public decimal Amount { get; private set; }

        public DonationStutes Stutes { get; private set; } = DonationStutes.Pending;


        public DateTime? CompletedAt { get; private set; }
        public string? ReceiptNumber { get; private set; }

        public static Donation Create(Campaign campaign, Guid donorUserId, decimal amount,
        decimal minAmount, decimal maxAmount, DateTime now)
        {
            if (!campaign.CanAcceptDonation(now))
                throw new DomainException("Campaign is not accepting donations.");
            if (amount < minAmount || amount > maxAmount)
                throw new DomainException("Donation amount is out of range.");

            return new Donation
            {
                Id = Guid.NewGuid(),
                CampaignId = campaign.Id,
                DonorUserId = donorUserId,
                Amount = amount,
                Stutes = DonationStutes.Pending
            };
        }
        public void MarkSucceeded(Campaign campaign, string receiptNumber, DateTime now)
        {
            if (Stutes != DonationStutes.Pending)
                throw new DomainException("Only a pending donation can succeed.");

            Stutes = DonationStutes.Succeeded;
            CompletedAt = now;
            ReceiptNumber = receiptNumber;
            campaign.AddDonation(Amount);
        }
        public void MarkFailed()
        {
            if (Stutes != DonationStutes.Pending)
                throw new DomainException("Only a pending donation can fail.");

            Stutes = DonationStutes.Failed;
        }
    }
}
