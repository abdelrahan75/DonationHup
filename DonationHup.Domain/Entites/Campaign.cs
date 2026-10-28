using DonationHup.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Domain.Entites
{
    public class Campaign : AuditableEntity
    {
     
        public string ArName { get; set; } = string.Empty;
        public string EnName { get; set; } = string.Empty;
        public string ArDescription { get; set; } = string.Empty;
        public string EnDescription { get; set; } = string.Empty;

        public int GoalAmount { get; set; }

        public int RaisedAmount { get; set; }

        public DateTime EndTime { get; set; }

        public Guid CatogryId { get; set; }

        public string BannerFileName { get; set; } = string.Empty;

        public CampaignStutes Stutes { get; set; } = CampaignStutes.Active;

        public bool createdByUserId { get; set; } 

        public bool IsDeleted { get; set; } = false;
        public int RowVersion { get; set; } 
    }
}
