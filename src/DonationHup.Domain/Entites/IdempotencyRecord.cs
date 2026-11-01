using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Domain.Entites
{
    public class IdempotencyRecord
    {
        public string Key { get; private set; } = default!;
        public Guid UserId { get; private set; }
        public string RequestHash { get; private set; } = default!;
        public int ResponseStatusCode { get; private set; }
        public string ResponseBody { get; private set; } = default!;
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }

        private IdempotencyRecord() { } 

        public static IdempotencyRecord Create(
            string key, Guid userId, string requestHash,
            int responseStatusCode, string responseBody,
            DateTime nowUtc, TimeSpan lifetime)
        {
            return new IdempotencyRecord
            {
                Key = key,
                UserId = userId,
                RequestHash = requestHash,
                ResponseStatusCode = responseStatusCode,
                ResponseBody = responseBody,
                CreatedAt = nowUtc,
                ExpiresAt = nowUtc.Add(lifetime)
            };
        }


    }
}
