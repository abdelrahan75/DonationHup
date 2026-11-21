using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Common.Results
{
    public static class CommonErrors
    {
        public static readonly Error RateLimited =
            new("Common.RateLimited", "Too many requests. Try again later.", ErrorType.TooManyRequests);
    }

    public static class FileErrors
    {
        public static readonly Error InvalidType =
            new("File.InvalidType", "This file type is not allowed.", ErrorType.Validation);

        public static readonly Error TooLarge =
            new("File.TooLarge", "The file exceeds the maximum size.", ErrorType.PayloadTooLarge);
    }

    public static class PaymentErrors
    {
        public static readonly Error InvalidSignature =
            new("Payment.InvalidSignature", "The webhook signature is invalid.", ErrorType.Validation);
    }

    public static class IdempotencyErrors
    {
        public static readonly Error KeyReusedWithDifferentRequest =
            new("Idempotency.KeyReusedWithDifferentRequest", "The same Idempotency-Key was used with a different request.", ErrorType.UnprocessableEntity);
    }
}
