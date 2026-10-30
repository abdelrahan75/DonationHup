using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Domain.Entites
{
    public class ProcessedStripeEvent
    {
        public string EventId { get; private set; } = default!;
        public string Type { get; private set; } = default!;
        public DateTime ProcessedAt { get; private set; }

        private ProcessedStripeEvent() { } // for EF Core

        public static ProcessedStripeEvent Create(string eventId, string type, DateTime nowUtc)
        {
            return new ProcessedStripeEvent { EventId = eventId, Type = type, ProcessedAt = nowUtc };
        }



    }
}
