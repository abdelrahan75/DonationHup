using DonationHup.Domain.Common;
using DonationHup.Domain.Entites;
using DonationHup.Domain.Enum;
using FluentAssertions;
using Xunit;
using static DonationHub.UnitTests.TestData;

namespace DonationHub.UnitTests
{
    public class CampaignDomainTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Create_GoalNotPositive_Fails(decimal goal)
        {
            // Arrange + Act
            var result = Campaign.Create("ع", "T", "و", "D", goal, Now.AddDays(5), Guid.NewGuid(), Guid.NewGuid(), Now);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error!.Code.Should().Be("Campaign.InvalidGoal");
        }

        [Fact]
        public void Create_EndDateInPast_Fails()
        {
            var result = Campaign.Create("ع", "T", "و", "D", 100, Now.AddDays(-1), Guid.NewGuid(), Guid.NewGuid(), Now);

            result.Error!.Code.Should().Be("Campaign.EndDateInPast");
        }

        [Fact]
        public void Create_ValidData_StartsActiveWithZeroRaised()
        {
            var campaign = NewCampaign();

            campaign.Status.Should().Be(CampaignStutes.Active);
            campaign.RaisedAmount.Should().Be(0);
        }

        [Fact]
        public void Update_GoalBelowRaised_Fails()
        {
            // Arrange
            var campaign = NewCampaign(goal: 1000);
            campaign.AddSucceededDonation(400, Now);

            // Act
            var result = campaign.Update("ع", "T", "و", "D", 300, campaign.EndTime, campaign.CategoryId, Now);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error!.Code.Should().Be("Campaign.GoalBelowRaised");
        }

        [Fact]
        public void Update_CompletedCampaign_Fails()
        {
            var campaign = NewCampaign(goal: 100);
            campaign.AddSucceededDonation(100, Now); // reaches the goal -> Completed

            var result = campaign.Update("ع", "T", "و", "D", 500, campaign.EndTime, campaign.CategoryId, Now);

            result.IsFailure.Should().BeTrue();
            result.Error!.Code.Should().Be("Campaign.NotActive");
        }

        [Fact]
        public void Update_ExpiredCampaign_Fails()
        {
            var campaign = NewCampaign(daysToEnd: 1);
            campaign.MarkExpired(Now.AddDays(2)).IsSuccess.Should().BeTrue();

            var result = campaign.Update("ع", "T", "و", "D", 500, Now.AddDays(30), campaign.CategoryId, Now.AddDays(2));

            result.IsFailure.Should().BeTrue();
            result.Error!.Code.Should().Be("Campaign.NotActive");
        }

        [Fact]
        public void Update_NewEndDateInPast_Fails()
        {
            var campaign = NewCampaign();

            var result = campaign.Update("ع", "T", "و", "D", 1000, Now.AddDays(-1), campaign.CategoryId, Now);

            result.IsFailure.Should().BeTrue();
            result.Error!.Code.Should().Be("Campaign.EndDateInPast");
        }

        [Fact]
        public void AddSucceededDonation_GoalReached_BecomesCompleted()
        {
            var campaign = NewCampaign(goal: 100);

            campaign.AddSucceededDonation(100, Now);

            campaign.Status.Should().Be(CampaignStutes.Completed);
        }

        [Fact]
        public void SoftDelete_MarksDeleted()
        {
            var campaign = NewCampaign();

            campaign.SoftDelete(Now);

            campaign.IsDeleted.Should().BeTrue();
            campaign.DeletedAtUtc.Should().Be(Now);
        }

        [Fact]
        public void Category_WithCampaigns_CannotBeDeleted()
        {
            var category = Category.Create("ص", "Health", "و", "D", Now);

            var result = category.EnsureCanBeDeleted(hasCampaigns: true);

            result.IsFailure.Should().BeTrue();
            result.Error!.Code.Should().Be("Category.HasCampaigns");
        }
    }
}

