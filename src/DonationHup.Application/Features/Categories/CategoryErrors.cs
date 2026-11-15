using DonationHup.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Categories
{
    public static class CategoryErrors
    {
        public static readonly Error NotFound =
            new("Category.NotFound", "Category not found.", ErrorType.NotFound);

        public static readonly Error HasCampaigns =
            new("Category.HasCampaigns", "This category still has campaigns and cannot be deleted.", ErrorType.Conflict);

        public static readonly Error NameArAlreadyExists =
            new("Category.NameArAlreadyExists", "A category with this Arabic name already exists.", ErrorType.Conflict);

        public static readonly Error NameEnAlreadyExists =
            new("Category.NameEnAlreadyExists", "A category with this English name already exists.", ErrorType.Conflict);
    }

}
