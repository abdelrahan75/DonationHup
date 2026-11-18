using DonationHup.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Campaigns
{
    public static class CampaignErrors
    {
        public static readonly Error NotFound =
            new("Campaign.NotFound", "Campaign was not found.", ErrorType.NotFound);
    }
}
