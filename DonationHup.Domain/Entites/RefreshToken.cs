using DonationHup.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Domain.Entites
{
    public class RefreshToken : Entity
    {
        public Guid UserId { get; private set; }
        public string TokenHash { get; private set; } = "";   
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public Guid? ReplacedByTokenId { get; private set; }
        public static RefreshToken Create(Guid userId, string tokenHash, DateTime now, TimeSpan lifetime) => new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAt = now,
            ExpiresAt = now + lifetime
        };

        public void Revoke(DateTime nowUtc, Guid? replacedByTokenId = null)
        {
            RevokedAt = nowUtc;
            ReplacedByTokenId = replacedByTokenId;
        }
    }
}
