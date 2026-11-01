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
    public class Campaign : Entity
    {
        private readonly List<Donation> _donations = new();
        public string ArName { get; private set; } = string.Empty;
        public string EnName { get; private set; } = string.Empty;
        public string ArDescription { get; private set; } = string.Empty;
        public string EnDescription { get; private set; } = string.Empty;

        public decimal GoalAmount { get; private set; }

        public decimal RaisedAmount { get; private set; }

        public DateTime EndTime { get;private set; }

        public Guid CategoryId { get; private set; }

        public Category Category { get; private set; }

        public string? BannerFileName { get; private set; } = string.Empty;

        public CampaignStutes Status { get; private set; } = CampaignStutes.Active;

        public Guid CreatedByUserId { get; private set; } 

        public DateTime UpdateAt { get; private set; }

        public bool IsDeleted { get; set; } = false;
        public byte[]? RowVersion { get; private set; }
        public IReadOnlyCollection<Donation> Donations => _donations;


        private Campaign() { }

        public static Campaign Create(
       string titleAr, string titleEn,
       string descriptionAr, string descriptionEn,
       decimal goalAmount, DateTime endDateUtc,
       Guid categoryId, Guid createdByUserId,
       DateTime nowUtc)
        {
            EnsureGoalIsPositive(goalAmount);          // invariant 1
            EnsureEndDateInFuture(endDateUtc, nowUtc); // invariant 2

            return new Campaign
            {
                ArName = titleAr,
                EnName = titleEn,
                ArDescription = descriptionAr,
                EnDescription = descriptionEn,
                GoalAmount = goalAmount,
                RaisedAmount = 0,
                EndTime = endDateUtc,
                CategoryId = categoryId,
                CreatedByUserId = createdByUserId,
                Status = CampaignStutes.Active,
                CreatedAt = nowUtc,
                UpdateAt = nowUtc
         
            };
        
        }
        public void Update(
        string titleAr, string titleEn,
        string descriptionAr, string descriptionEn,
        decimal goalAmount, DateTime endDateUtc,
        Guid categoryId, DateTime nowUtc)
        {
            EnsureNotFinal();

            EnsureGoalIsPositive(goalAmount);
            if (goalAmount < RaisedAmount) 
                throw new DomainException("Campaign.GoalBelowRaised", "The goal cannot be lower than the amount already raised.");

            if (endDateUtc != EndTime) 
                EnsureEndDateInFuture(endDateUtc, nowUtc);

            ArName = titleAr;
            EnName = titleEn;
            ArDescription = descriptionAr;
            EnDescription = descriptionEn;
            GoalAmount = goalAmount;
            EndTime = endDateUtc;
            CategoryId = categoryId;
            UpdateAt = nowUtc;
        }
        public void EnsureCanAcceptDonation(DateTime nowUtc)
        {
            if (Status != CampaignStutes.Active || EndTime <= nowUtc)
                throw new DomainException("Campaign.NotActive", "This campaign is not accepting donations.");
        }
        public void AddSucceededDonation(decimal amount, DateTime nowUtc)
        {
            RaisedAmount += amount;
            UpdateAt = nowUtc;

            if (Status == CampaignStutes.Active && RaisedAmount >= GoalAmount)
                Status = CampaignStutes.Completed;
        }

        public void MarkExpired(DateTime nowUtc)
        {
            EnsureNotFinal(); // invariant 8

            if (EndTime > nowUtc)
                throw new DomainException("Campaign.NotActive", "The campaign end date has not passed yet.");

            Status = CampaignStutes.Expired;
            UpdateAt = nowUtc;
        }

        public void SoftDelete(DateTime nowUtc)
        {
            IsDeleted = true;
            UpdateAt = nowUtc;
        }
        private void EnsureNotFinal()
        {
            if (Status != CampaignStutes.Active)
                throw new DomainException("Campaign.NotActive", "A completed or expired campaign cannot be changed.");
        }
        private static void EnsureGoalIsPositive(decimal goal)
        {
            if (goal <= 0)
                throw new DomainException("Campaign.InvalidGoal", "The goal amount must be greater than zero.");
        }
        private static void EnsureEndDateInFuture(DateTime endDateUtc, DateTime nowUtc)
        {
            if (endDateUtc <= nowUtc)
                throw new DomainException("Campaign.EndDateInPast", "The end date must be in the future.");
        }

    };
}
