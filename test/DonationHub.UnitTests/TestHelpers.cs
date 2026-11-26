using DonationHup.Domain.Entites;

namespace DonationHub.UnitTests
{
    // A clock that always returns the same time, so tests are deterministic.
    public class FixedTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _now;
        public FixedTimeProvider(DateTime utcNow) => _now = new DateTimeOffset(utcNow, TimeSpan.Zero);
        public override DateTimeOffset GetUtcNow() => _now;
    }

    public static class TestData
    {
        public static readonly DateTime Now = new(2026, 11, 21, 12, 0, 0, DateTimeKind.Utc);

        public static Campaign NewCampaign(decimal goal = 1000, int daysToEnd = 10) =>
            Campaign.Create("عنوان", "Title", "وصف", "Description",
                goal, Now.AddDays(daysToEnd), Guid.NewGuid(), Guid.NewGuid(), Now).Value!;
    }
}


