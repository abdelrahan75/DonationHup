using DonationHup.Application.Features.Campaigns;
using FluentAssertions;
using Xunit;
using static DonationHub.UnitTests.TestData;

namespace DonationHub.UnitTests
{
    public class CampaignMappingTests
    {
        [Fact]
        public void ProgressPercent_RaisedMoreThanGoal_IsCappedAt100()
        {
            var percent = CampaignMapping.ProgressPercent(raised: 1500, goal: 1000);

            percent.Should().Be(100);
        }

        [Fact]
        public void ProgressPercent_HalfRaised_Is50()
        {
            CampaignMapping.ProgressPercent(500, 1000).Should().Be(50);
        }

        [Fact]
        public void DaysLeft_TenDaysAhead_Is10()
        {
            // Arrange
            var time = new FixedTimeProvider(Now);

            // Act
            var days = CampaignMapping.DaysLeft(Now.AddDays(10), time.GetUtcNow().UtcDateTime);

            // Assert
            days.Should().Be(10);
        }

        [Fact]
        public void DaysLeft_EndDatePassed_IsNeverNegative()
        {
            var days = CampaignMapping.DaysLeft(Now.AddDays(-5), Now);

            days.Should().Be(0);
        }

        [Theory]
        [InlineData("ar", true)]
        [InlineData("ar-EG", true)]
        [InlineData("ar,en;q=0.9", true)]
        [InlineData("en", false)]
        [InlineData("fr", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsArabic_PicksLanguageFromHeader(string? header, bool expected)
        {
            CampaignMapping.IsArabic(header).Should().Be(expected);
        }
    }
}

