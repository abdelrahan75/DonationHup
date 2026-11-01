using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Domain.Common
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        protected abstract IEnumerable<object?> GetEqualityComponents();

        public override bool Equals(object? obj)
        {
            if (obj is null || obj.GetType() != GetType())
                return false;

            return GetEqualityComponents().SequenceEqual(((ValueObject)obj).GetEqualityComponents());
        }

        public bool Equals(ValueObject? other) => Equals((object?)other);

        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (var component in GetEqualityComponents())
                hash.Add(component);
            return hash.ToHashCode();
        }

        public static bool operator ==(ValueObject? left, ValueObject? right) => Equals(left, right);

        public static bool operator !=(ValueObject? left, ValueObject? right) => !Equals(left, right);
    }
}