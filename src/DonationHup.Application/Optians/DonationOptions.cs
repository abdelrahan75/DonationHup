using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Optians
{
    public class DonationOptions : IValidatableObject
    {
        public const string SectionName = "Donation";

        [Range(typeof(decimal), "0.01", "1000000000")]
        public decimal MinAmount { get; set; } = 1.00m;

        [Range(typeof(decimal), "0.01", "1000000000")]
        public decimal MaxAmount { get; set; } = 10_000.00m;

        [Range(1, 1440)]
        public int PendingTimeoutMinutes { get; set; } = 30;

        [Required, StringLength(3, MinimumLength = 3)]
        public string DefaultCurrency { get; set; } = "USD";

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MaxAmount < MinAmount)
            {
                yield return new ValidationResult(
                    "MaxAmount must be greater than or equal to MinAmount.",
                    new[] { nameof(MaxAmount), nameof(MinAmount) });
            }
        }
    }
}
