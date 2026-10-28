using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Domain.Entites
{
    public abstract class AuditableEntity : GenaricEntity
    {
        public DateTime CreatedAt { get; protected set; } 
        public DateTime UpdatedAt { get; protected set; }
    }
}
