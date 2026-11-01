using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Domain.Common
{
    public sealed class Money : ValueObject
    {
        public decimal Amount { get; }
        public string Currency { get; }

        private Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static Money Create(decimal amount, string currency)
        {
            if (amount <= 0)
                throw new DomainException("Money.InvalidAmount", "Amount must be greater than zero.");

            if (string.IsNullOrWhiteSpace(currency) || currency.Trim().Length != 3)
                throw new DomainException("Money.InvalidCurrency", "Currency must be a 3-letter code.");

            return new Money(amount, currency.ToUpperInvariant());
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }
}

