using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Domain.Entites
{
    public abstract class GenaricEntity
    {
        public Guid Id { get; protected set; }
    }
}
