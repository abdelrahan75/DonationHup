using DonationHup.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features
{
    public static class DonationErrors
    {
        public static readonly Error NotFound =
            new("Donation.NotFound", "Donation not found.", ErrorType.NotFound);

        public static readonly Error AmountOutOfRange =
            new("Donation.AmountOutOfRange", "The amount is outside the allowed range.", ErrorType.UnprocessableEntity);

        public static readonly Error InvalidState =
            new("Donation.InvalidState", "The donation is already in a final state.", ErrorType.Conflict);
    }
}
