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
        public void Create_GoalNotPositive_Throws(decimal goal)
        {
            // Arrange + Act
            var act = () => Campaign.Create("ع", "T", "و", "D", goal, Now.AddDays(5), Guid.NewGuid(), Guid.NewGuid(), Now);

            // Assert
            act.Should().Throw<DomainException>().Which.Code.Should().Be("Campaign.InvalidGoal");
        }

        [Fact]
        public void Create_EndDateInPast_Throws()
        {
            var act = () => Campaign.Create("ع", "T", "و", "D", 100, Now.AddDays(-1), Guid.NewGuid(), Guid.NewGuid(), Now);

            act.Should().Throw<DomainException>().Which.Code.Should().Be("Campaign.EndDateInPast");
        }

        [Fact]
        public void Create_ValidData_StartsActiveWithZeroRaised()
        {
            var campaign = NewCampaign();

            campaign.Status.Should().Be(CampaignStutes.Active);
            campaign.RaisedAmount.Should().Be(0);
        }

        [Fact]
        public void Update_GoalBelowRaised_Throws()
        {
            // Arrange
            var campaign = NewCampaign(goal: 1000);
            campaign.AddSucceededDonation(400, Now);

            // Act
            var act = () => campaign.Update("ع", "T", "و", "D", 300, campaign.EndTime, campaign.CategoryId, Now);

            // Assert
            act.Should().Throw<DomainException>().Which.Code.Should().Be("Campaign.GoalBelowRaised");
        }

        [Fact]
        public void Update_CompletedCampaign_Throws()
        {
            var campaign = NewCampaign(goal: 100);
            campaign.AddSucceededDonation(100, Now); // reaches the goal -> Completed

            var act = () => campaign.Update("ع", "T", "و", "D", 500, campaign.EndTime, campaign.CategoryId, Now);

            act.Should().Throw<DomainException>().Which.Code.Should().Be("Campaign.NotActive");
        }

        [Fact]
        public void Update_ExpiredCampaign_Throws()
        {
            var campaign = NewCampaign(daysToEnd: 1);
            campaign.MarkExpired(Now.AddDays(2));

            var act = () => campaign.Update("ع", "T", "و", "D", 500, Now.AddDays(30), campaign.CategoryId, Now.AddDays(2));

            act.Should().Throw<DomainException>().Which.Code.Should().Be("Campaign.NotActive");
        }

        [Fact]
        public void Update_NewEndDateInPast_Throws()
        {
            var campaign = NewCampaign();

            var act = () => campaign.Update("ع", "T", "و", "D", 1000, Now.AddDays(-1), campaign.CategoryId, Now);

            act.Should().Throw<DomainException>().Which.Code.Should().Be("Campaign.EndDateInPast");
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

            var act = () => category.EnsureCanBeDeleted(hasCampaigns: true);

            act.Should().Throw<DomainException>().Which.Code.Should().Be("Category.HasCampaigns");
        }
    }
}

