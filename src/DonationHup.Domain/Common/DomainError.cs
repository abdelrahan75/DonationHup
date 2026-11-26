using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Domain.Common
{
    public record DomainError(string Code, string Message);

    public class DomainResult
    {
        protected DomainResult(DomainError? error) => Error = error;

        public DomainError? Error { get; }
        public bool IsSuccess => Error is null;
        public bool IsFailure => !IsSuccess;

        public static DomainResult Success() => new(null);
        public static DomainResult Failure(DomainError error) => new(error);
        public static DomainResult<T> Success<T>(T value) => new(value, null);
        public static DomainResult<T> Failure<T>(DomainError error) => new(default, error);
    }

    public class DomainResult<T> : DomainResult
    {
        internal DomainResult(T? value, DomainError? error) : base(error) => Value = value;

        public T? Value { get; }
    }
}
