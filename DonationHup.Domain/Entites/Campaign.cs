using DonationHup.Domain.Common;
using DonationHup.Domain.Enum;
using DonationHup.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Domain.Entites
{
    public class Campaign : AuditableEntity
    {
     
        public string ArName { get; private set; } = string.Empty;
        public string EnName { get; private set; } = string.Empty;
        public string ArDescription { get; private set; } = string.Empty;
        public string EnDescription { get; private set; } = string.Empty;

        public decimal GoalAmount { get; private set; }

        public decimal RaisedAmount { get; private set; }

        public DateTime EndTime { get;private set; }

        public Guid CategoryId { get; private set; }

        public string? BannerFileName { get; private set; } = string.Empty;

        public CampaignStutes Status { get; private set; } = CampaignStutes.Active;

        public Guid CreatedByUserId { get; private set; } 

        public bool IsDeleted { get; set; } = false;
        public byte[]? RowVersion { get; private set; }
        public static Campaign Create(string titleAr, string titleEn, string descriptionAr, string descriptionEn,
       decimal goalAmount, DateTime endDate, Guid categoryId, Guid createdByUserId, DateTime now)
        {
            var campaign = new Campaign
            {
                Id = Guid.NewGuid(),
                CreatedByUserId = createdByUserId,
                Status = CampaignStutes.Active
            };
            campaign.Update(titleAr, titleEn, descriptionAr, descriptionEn, categoryId, goalAmount, endDate, now);
            return campaign;
        }
        public void Update(string titleAr, string titleEn, string descriptionAr, string descriptionEn,
        Guid categoryId, decimal goalAmount, DateTime endDate, DateTime now)
        {
            if (Status != CampaignStutes.Active)
                throw new DomainException("Campaign is not active.");
            if (goalAmount <= 0)
                throw new DomainException("Goal amount must be greater than zero.");
            if (goalAmount < RaisedAmount)
                throw new DomainException("Goal amount cannot be lower than the raised amount.");
            if (endDate != EndTime && endDate <= now)
                throw new DomainException("End date must be in the future.");

            ArName = titleAr;
            EnName = titleEn;
            ArDescription = descriptionAr;
            EnDescription = descriptionEn;
            CategoryId = categoryId;
            GoalAmount = goalAmount;
            EndTime = endDate;
        }
        public bool CanAcceptDonation(DateTime now) => Status == CampaignStutes.Active && EndTime > now;
        public void Expire(DateTime now)
        {
            if (Status == CampaignStutes.Active && EndTime <= now)
                Status = CampaignStutes.Expired;
        }

        public void SoftDelete() => IsDeleted = true;

        public void SetBanner(string? fileName) => BannerFileName = fileName;

        internal void AddDonation(decimal amount)
        {
            RaisedAmount += amount;
            if (Status == CampaignStutes.Active && RaisedAmount >= GoalAmount)
                Status = CampaignStutes.Completed;
        }

    };
}
