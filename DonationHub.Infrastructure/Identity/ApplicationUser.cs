using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHub.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } 
        public string PreferredLanguage { get; set; } 
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}
