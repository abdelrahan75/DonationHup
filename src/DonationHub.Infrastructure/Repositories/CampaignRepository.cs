using DonationHub.Infrastructure.data;
using DonationHup.Application.Features.Campaigns;
using DonationHup.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHub.Infrastructure.Repositories
{
    public class CampaignRepository : GenaricRepository<Campaign>, ICampaignRepository
    {
        private readonly AppDbContext _context;

        public CampaignRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void SetRowVersion(Campaign campaign, byte[] rowVersion) =>
            _context.Entry(campaign).Property(c => c.RowVersion).OriginalValue = rowVersion;
    }
}
