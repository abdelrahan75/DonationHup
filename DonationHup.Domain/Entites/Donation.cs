using DonationHup.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Domain.Entites
{
    public class Donation : AuditableEntity
    {
        

        public int CampaignId { get; set; }

        public int DonorUserId { get; set; }

        public int Amount { get; set; }

        public DonationStutes Stutes { get; set; } = DonationStutes.Pending;


        public DateTime CompletedAt { get; set; }
        public int ReceiptNumber { get; set; } 

    }
}
