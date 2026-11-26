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
    public class Campaign : Entity, ISoftDeletable
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

        public DateTime? DeletedAtUtc { get; set ;}
        public byte[]? RowVersion { get; private set; }
        public IReadOnlyCollection<Donation> Donations => _donations;

       

        private Campaign() { }

        public static DomainResult<Campaign> Create(
      string titleAr, string titleEn,
      string descriptionAr, string descriptionEn,
      decimal goalAmount, DateTime endDateUtc,
      Guid categoryId, Guid createdByUserId,
      DateTime nowUtc)
        {
            if (CheckGoal(goalAmount) is { } goalError)               // invariant 1
                return DomainResult.Failure<Campaign>(goalError);
            if (CheckEndDate(endDateUtc, nowUtc) is { } dateError)    // invariant 2
                return DomainResult.Failure<Campaign>(dateError);

            return DomainResult.Success(new Campaign
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

            });

        }
        public DomainResult Update(
        string titleAr, string titleEn,
        string descriptionAr, string descriptionEn,
        decimal goalAmount, DateTime endDateUtc,
        Guid categoryId, DateTime nowUtc)
        {
            if (CheckNotFinal() is { } finalError)                         // invariant 8
                return DomainResult.Failure(finalError);
            if (CheckGoal(goalAmount) is { } goalError)                    // invariant 1
                return DomainResult.Failure(goalError);
            if (goalAmount < RaisedAmount)                                 // invariant 7
                return DomainResult.Failure(new DomainError("Campaign.GoalBelowRaised", "The goal cannot be lower than the amount already raised."));
            if (endDateUtc != EndTime && CheckEndDate(endDateUtc, nowUtc) is { } dateError) // invariant 2
                return DomainResult.Failure(dateError);

            ArName = titleAr;
            EnName = titleEn;
            ArDescription = descriptionAr;
            EnDescription = descriptionEn;
            GoalAmount = goalAmount;
            EndTime = endDateUtc;
            CategoryId = categoryId;
            UpdateAt = nowUtc;
            return DomainResult.Success();
        }
        public DomainResult EnsureCanAcceptDonation(DateTime nowUtc)
        {
            return Status != CampaignStutes.Active || EndTime <= nowUtc
                ? DomainResult.Failure(new DomainError("Campaign.NotActive", "This campaign is not accepting donations."))
                : DomainResult.Success();
        }
        public void AddSucceededDonation(decimal amount, DateTime nowUtc)
        {
            RaisedAmount += amount;
            UpdateAt = nowUtc;

            if (Status == CampaignStutes.Active && RaisedAmount >= GoalAmount)
                Status = CampaignStutes.Completed;
        }

        public DomainResult MarkExpired(DateTime nowUtc)
        {
            if (CheckNotFinal() is { } finalError)    // invariant 8
                return DomainResult.Failure(finalError);

            if (EndTime > nowUtc)
                return DomainResult.Failure(new DomainError("Campaign.NotActive", "The campaign end date has not passed yet."));

            Status = CampaignStutes.Expired;
            UpdateAt = nowUtc;
            return DomainResult.Success();
        }

        public void SoftDelete(DateTime nowUtc)
        {
            IsDeleted = true;
            DeletedAtUtc = nowUtc;
            UpdateAt = nowUtc;
        }
        private DomainError? CheckNotFinal() =>
            Status != CampaignStutes.Active
                ? new DomainError("Campaign.NotActive", "A completed or expired campaign cannot be changed.")
                : null;

        private static DomainError? CheckGoal(decimal goal) =>
            goal <= 0
                ? new DomainError("Campaign.InvalidGoal", "The goal amount must be greater than zero.")
                : null;

        private static DomainError? CheckEndDate(DateTime endDateUtc, DateTime nowUtc) =>
            endDateUtc <= nowUtc
                ? new DomainError("Campaign.EndDateInPast", "The end date must be in the future.")
                : null;
    }
}
