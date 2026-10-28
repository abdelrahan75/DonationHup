using DonationHup.Domain.Common;
using DonationHup.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Domain.Entites
{
    public class Category : AuditableEntity
    {
        public string ArName { get; private set; } = string.Empty;

        public string EnName { get; private set; } = string.Empty;

        public string ArDescription { get;private set; } = string.Empty;

        public string EnDescription { get; private set; } = string.Empty;

        public static Category Create(string nameAr, string nameEn, string descriptionAr, string descriptionEn)
        {
            var category = new Category { Id = Guid.NewGuid() };
            category.Update(nameAr, nameEn, descriptionAr, descriptionEn);
            return category;
        }
        public void Update(string nameAr, string nameEn, string descriptionAr, string descriptionEn)
        {
            ArName = nameAr;
            EnName = nameEn;
            ArDescription = descriptionAr;
            EnDescription = descriptionEn;
        }


    }
}
