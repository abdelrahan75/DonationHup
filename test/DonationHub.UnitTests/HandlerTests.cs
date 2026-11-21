using DonationHup.Application.Common.Behavior;
using DonationHup.Application.Common.Exceptions;
using DonationHup.Application.Common.Interfaces;
using DonationHup.Application.Common.Results;
using DonationHup.Application.Features.Auth;
using DonationHup.Application.Features.Auth.Login;
using DonationHup.Application.Features.Campaigns;
using DonationHup.Application.Features.Campaigns.Commend.CreateCampaigns;
using DonationHup.Application.Features.Campaigns.Commend.DeleteCampaign;
using DonationHup.Application.Features.Campaigns.Commend.UpdateCampaign;
using DonationHup.Application.Features.Categories;
using DonationHup.Domain.Entites;
using FluentAssertions;
using Moq;
using Xunit;
using static DonationHub.UnitTests.TestData;

namespace DonationHub.UnitTests
{
    public class HandlerTests
    {
        private readonly Mock<ICampaignRepository> _campaigns = new();
        private readonly Mock<IGenericRepository<Campaign>> _campaignsGeneric = new();
        private readonly Mock<IGenericRepository<Category>> _categories = new();
        private readonly Mock<IUnitofWork> _uow = new();
        private readonly FixedTimeProvider _time = new(Now);

        private static Category NewCategory() => Category.Create("ص", "Health", "و", "D", Now);

        private static CreateCampaignCommand CreateCmd(Guid categoryId) =>
            new("ع", "T", "و", "D", 1000, Now.AddDays(10), categoryId) { UserId = Guid.NewGuid() };

        private static UpdateCampaignCommand UpdateCmd(Guid id, Guid categoryId, decimal goal = 1000) =>
            new("ع", "T", "و", "D", goal, Now.AddDays(10), categoryId, new byte[] { 1 }) { Id = id };

        // ---------- Create ----------

        [Fact]
        public async Task Create_UnknownCategory_ReturnsCategoryNotFound_AndNeverSaves()
        {
            // Arrange
            var cmd = CreateCmd(Guid.NewGuid());
            _categories.Setup(r => r.GetByIdAsync(cmd.CategoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);
            var handler = new CreateCampaignCommandHandler(_campaignsGeneric.Object, _categories.Object, _uow.Object, _time);

            // Act
            var result = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(CategoryErrors.NotFound);
            _uow.Verify(u => u.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Create_ValidCommand_AddsCampaignAndSavesOnce()
        {
            // Arrange
            var cmd = CreateCmd(Guid.NewGuid());
            _categories.Setup(r => r.GetByIdAsync(cmd.CategoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(NewCategory());
            var handler = new CreateCampaignCommandHandler(_campaignsGeneric.Object, _categories.Object, _uow.Object, _time);

            // Act
            var result = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _campaignsGeneric.Verify(r => r.AddAsync(It.IsAny<Campaign>(), It.IsAny<CancellationToken>()), Times.Once);
            _uow.Verify(u => u.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_EndDateInPast_ReturnsEndDateInPast()
        {
            var cmd = CreateCmd(Guid.NewGuid()) with { EndDate = Now.AddDays(-1) };
            _categories.Setup(r => r.GetByIdAsync(cmd.CategoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(NewCategory());
            var handler = new CreateCampaignCommandHandler(_campaignsGeneric.Object, _categories.Object, _uow.Object, _time);

            var result = await handler.Handle(cmd, CancellationToken.None);

            result.Error.Should().Be(CampaignErrors.EndDateInPast);
        }

        // ---------- Update ----------

        [Fact]
        public async Task Update_UnknownCampaign_ReturnsCampaignNotFound()
        {
            // Arrange
            var cmd = UpdateCmd(Guid.NewGuid(), Guid.NewGuid());
            _campaigns.Setup(r => r.GetByIdAsync(cmd.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Campaign?)null);
            var handler = new UpdateCampaignCommandHandler(_campaigns.Object, _categories.Object, _uow.Object, _time);

            // Act
            var result = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.Error.Should().Be(CampaignErrors.NotFound);
        }

        [Fact]
        public async Task Update_ConcurrencyException_ReturnsConcurrencyConflict()
        {
            // Arrange
            var campaign = NewCampaign();
            var cmd = UpdateCmd(campaign.Id, campaign.CategoryId);
            _campaigns.Setup(r => r.GetByIdAsync(cmd.Id, It.IsAny<CancellationToken>())).ReturnsAsync(campaign);
            _categories.Setup(r => r.GetByIdAsync(cmd.CategoryId, It.IsAny<CancellationToken>())).ReturnsAsync(NewCategory());
            _uow.Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ConcurrencyException(new Exception("db")));
            var handler = new UpdateCampaignCommandHandler(_campaigns.Object, _categories.Object, _uow.Object, _time);

            // Act
            var result = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.Error.Should().Be(CampaignErrors.ConcurrencyConflict);
            result.Error!.Type.Should().Be(ErrorType.Conflict);
        }

        [Fact]
        public async Task Update_GoalBelowRaised_ReturnsGoalBelowRaised_AndNeverSaves()
        {
            // Arrange
            var campaign = NewCampaign(goal: 1000);
            campaign.AddSucceededDonation(400, Now);
            var cmd = UpdateCmd(campaign.Id, campaign.CategoryId, goal: 300);
            _campaigns.Setup(r => r.GetByIdAsync(cmd.Id, It.IsAny<CancellationToken>())).ReturnsAsync(campaign);
            _categories.Setup(r => r.GetByIdAsync(cmd.CategoryId, It.IsAny<CancellationToken>())).ReturnsAsync(NewCategory());
            var handler = new UpdateCampaignCommandHandler(_campaigns.Object, _categories.Object, _uow.Object, _time);

            // Act
            var result = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.Error.Should().Be(CampaignErrors.GoalBelowRaised);
            _uow.Verify(u => u.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        // ---------- Delete ----------

        [Fact]
        public async Task Delete_ExistingCampaign_SoftDeletesAndSaves()
        {
            // Arrange
            var campaign = NewCampaign();
            _campaigns.Setup(r => r.GetByIdAsync(campaign.Id, It.IsAny<CancellationToken>())).ReturnsAsync(campaign);
            var handler = new DeleteCampaignCommandHandler(_campaigns.Object, _uow.Object, _time);

            // Act
            var result = await handler.Handle(new DeleteCampaignCommand(campaign.Id), CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            campaign.IsDeleted.Should().BeTrue();
            _uow.Verify(u => u.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        // ---------- Login ----------

        [Fact]
        public async Task Login_ServiceRejectsCredentials_ReturnsInvalidCredentials()
        {
            // Arrange
            var auth = new Mock<IAuthService>();
            auth.Setup(a => a.LoginAsync(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<AuthResponse>(AuthErrors.InvalidCredentials));
            var handler = new LoginCommandHandler(auth.Object);

            // Act
            var result = await handler.Handle(new LoginCommand("x@y.com", "Wrong1Pass"), CancellationToken.None);

            // Assert
            result.Error.Should().Be(AuthErrors.InvalidCredentials);
        }

        // ---------- Pipeline ----------

        [Fact]
        public async Task ValidationBehavior_InvalidRequest_ReturnsValidationFailed_AndNeverCallsHandler()
        {
            // Arrange
            var behavior = new ValidationBehavior<CreateCampaignCommand, Result<CampaignAdminDto>>(
                new[] { new CreateCampaignCommandValidator() });
            var bad = new CreateCampaignCommand("", "", "", "", 0, Now.AddDays(5), Guid.Empty);
            var handlerCalled = false;

            // Act
            var result = await behavior.Handle(bad, () =>
            {
                handlerCalled = true;
                return Task.FromResult(Result.Success(new CampaignAdminDto(
                    Guid.Empty, "", "", "", "", 0, 0, Now, Guid.Empty, DonationHup.Domain.Enum.CampaignStutes.Active, null)));
            }, CancellationToken.None);

            // Assert
            handlerCalled.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeOfType<ValidationError>();
            result.Error!.Code.Should().Be("Validation.Failed");
        }
    }
}

