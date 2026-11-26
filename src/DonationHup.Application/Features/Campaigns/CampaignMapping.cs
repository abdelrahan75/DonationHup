using DonationHup.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Campaigns
{
    public static class CampaignMapping
    {
        public static bool IsArabic(string? acceptLanguage)
        {
            if (string.IsNullOrWhiteSpace(acceptLanguage)) return false;
            var first = acceptLanguage.Split(',', ';')[0].Trim();
            return first.Equals("ar", StringComparison.OrdinalIgnoreCase)
                || first.StartsWith("ar-", StringComparison.OrdinalIgnoreCase);
        }

        public static decimal ProgressPercent(decimal raised, decimal goal) =>
            goal <= 0 ? 0 : Math.Min(100m, Math.Round(raised / goal * 100m, 2));

        public static int DaysLeft(DateTime endUtc, DateTime nowUtc) =>
            endUtc <= nowUtc ? 0 : (int)Math.Ceiling((endUtc - nowUtc).TotalDays);
        public static CampaignAdminDto ToAdmin(Campaign c) => new(
            c.Id, c.ArName, c.EnName, c.ArDescription, c.EnDescription,
            c.GoalAmount, c.RaisedAmount, c.EndTime, c.CategoryId, c.Status, c.RowVersion);
    }
}
