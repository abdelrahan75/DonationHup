using DonationHup.Application.Features.Auth.Register;
using DonationHup.Application.Features.Campaigns.Commend.CreateCampaigns;
using FluentAssertions;
using Xunit;
using static DonationHub.UnitTests.TestData;

namespace DonationHub.UnitTests
{
    public class ValidatorTests
    {
        private readonly CreateCampaignCommandValidator _campaignValidator = new();
        private readonly RegisterCommandValidator _registerValidator = new();

        [Fact]
        public void CreateCampaign_MissingTitlesZeroGoalEmptyCategory_Fails()
        {
            // Arrange
            var command = new CreateCampaignCommand("", "", "وصف", "Desc", 0, Now.AddDays(5), Guid.Empty);

            // Act
            var result = _campaignValidator.Validate(command);

            // Assert
            result.Errors.Select(e => e.PropertyName)
                .Should().Contain(new[] { "TitleAr", "TitleEn", "GoalAmount", "CategoryId" });
        }

        [Fact]
        public void CreateCampaign_ValidCommand_Passes()
        {
            var command = new CreateCampaignCommand("ع", "T", "و", "D", 100, Now.AddDays(5), Guid.NewGuid());

            _campaignValidator.Validate(command).IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("short1A")]       // less than 8 characters
        [InlineData("alllowercase1")] // no uppercase
        [InlineData("ALLUPPERCASE1")] // no lowercase
        [InlineData("NoDigitsHere")]  // no digit
        public void Register_WeakPassword_Fails(string password)
        {
            var command = new RegisterCommand("a@b.com", "Ali", password, "en");

            var result = _registerValidator.Validate(command);

            result.Errors.Should().Contain(e => e.PropertyName == "Password");
        }

        [Fact]
        public void Register_StrongPassword_Passes()
        {
            var command = new RegisterCommand("a@b.com", "Ali", "Strong1Pass", "en");

            _registerValidator.Validate(command).IsValid.Should().BeTrue();
        }
    }
}
