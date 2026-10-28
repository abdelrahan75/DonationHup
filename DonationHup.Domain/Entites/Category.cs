using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Domain.Entites
{
    public class Category : AuditableEntity
    {
       

        public string ArName { get; set; } = string.Empty;

        public string EnName { get; set; } = string.Empty;

        public string ArDescription { get; set; } = string.Empty;

        public string EnDescription { get; set; } = string.Empty;

        
    }
}
