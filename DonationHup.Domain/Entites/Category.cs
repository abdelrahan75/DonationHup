using DonationHup.Domain.Common;
using DonationHup.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Domain.Entites
{
    public class Category : Entity
    {
        private readonly List<Campaign> _campaigns = new();

        public string ArName { get; private set; } = string.Empty;

        public string EnName { get; private set; } = string.Empty;

        public string ArDescription { get;private set; } = string.Empty;

        public string EnDescription { get; private set; } = string.Empty;

        public DateTime UpdateAt { get; private set; }

        public IReadOnlyCollection<Campaign> Campaigns => _campaigns;
        private Category() { }


        public static Category Create(string nameAr, string nameEn, string? descriptionAr, string? descriptionEn, DateTime nowUtc)
        {
            return new Category
            {
                ArName = nameAr.Trim(),
                EnName = nameEn.Trim(),
                ArDescription = descriptionAr,
                EnDescription  = descriptionEn,
                CreatedAt = nowUtc,
                UpdateAt = nowUtc
            };
        }
        public void Update(string nameAr, string nameEn, string? descriptionAr, string? descriptionEn, DateTime nowUtc)
        {
            ArName = nameAr.Trim();
            EnName = nameEn.Trim();
            ArDescription = descriptionAr;
            EnDescription = descriptionEn;
            UpdateAt = nowUtc;
        }

        public void EnsureCanBeDeleted(bool hasCampaigns)
        {
            if (hasCampaigns)
                throw new DomainException("Category.HasCampaigns", "This category still has campaigns and cannot be deleted.");
        }


    }
}
