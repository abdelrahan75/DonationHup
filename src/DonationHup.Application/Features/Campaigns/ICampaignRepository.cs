using DonationHup.Application.Common.Interfaces;
using DonationHup.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Campaigns
{
    public interface ICampaignRepository : IGenericRepository<Campaign>
    {
        void SetRowVersion(Campaign campaign, byte[] rowVersion);
    }
}
