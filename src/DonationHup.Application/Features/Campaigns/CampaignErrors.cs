using DonationHup.Application.Common.Results;
using DonationHup.Domain.Common;
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

        public static readonly Error NotActive =
            new("Campaign.NotActive", "The campaign is completed or expired.", ErrorType.Conflict);

        public static readonly Error GoalBelowRaised =
            new("Campaign.GoalBelowRaised", "The goal cannot be lower than the amount already raised.", ErrorType.UnprocessableEntity);

        public static readonly Error EndDateInPast =
            new("Campaign.EndDateInPast", "The end date must be in the future.", ErrorType.UnprocessableEntity);

        public static readonly Error ConcurrencyConflict =
            new("Campaign.ConcurrencyConflict", "The campaign was changed by someone else. Reload it and try again.", ErrorType.Conflict);

        public static Error FromDomain(DomainError e) => e.Code switch
        {
            "Campaign.NotActive" => NotActive,
            "Campaign.GoalBelowRaised" => GoalBelowRaised,
            "Campaign.EndDateInPast" => EndDateInPast,
            _ => new Error(e.Code, e.Message, ErrorType.UnprocessableEntity)
        };
    }
}
