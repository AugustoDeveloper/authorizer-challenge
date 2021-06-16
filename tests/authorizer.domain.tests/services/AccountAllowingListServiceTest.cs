using System;
using System.Linq;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Repositories;
using Authorizer.Domain.Services;
using Authorizer.Domain.Services.Interfaces;
using Moq;
using Xunit;

namespace Authorizer.Domain.Tests.Services
{
    public class AccountAllowingListServiceTest
    {

        [Fact]
        [Trait(nameof(IAuthorizationService), "new()")]
        public void Given_A_Creation_Of_Instance_When_Pass_Null_Args_Should_Thrown_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AccountAllowingListService(null));
        }

        [Fact]
        [Trait(nameof(IAuthorizationService), nameof(IAuthorizationService.Authorize))]
        public void Given_An_Instance_Calling_Authorize_When_Account_Was_Not_Created_Should_Return_Single_Violation_AccountNotInitialized()
        {
            var accountManagerGatewayMock = new Mock<IAccountRepository>();
            
            accountManagerGatewayMock
                .Setup(m => m.GetCurrentAccount())
                .Returns<Account>(null)
                .Verifiable();

            var service = new AccountAllowingListService(accountManagerGatewayMock.Object);
            var violations = service.AllowList(new AccountAllowList(false));

            Assert.NotEmpty(violations);
            Assert.Single(violations);
            Assert.Equal(Violation.AccountNotInitialized, violations.First());
            Assert.Null(violations.CurrentAccount);
            
            accountManagerGatewayMock.Verify(m => m.GetCurrentAccount(), Times.Once());
        }

        [Fact]
        [Trait(nameof(IAuthorizationService), nameof(IAuthorizationService.Authorize))]
        public void Given_An_Instance_Calling_Authorize_When_Account_Was_Created_Should_Return_Empty_Violation()
        {
            var accountManagerGatewayMock = new Mock<IAccountRepository>();
            
            accountManagerGatewayMock
                .Setup(m => m.GetCurrentAccount())
                .Returns(new Account(true, 100))
                .Verifiable();

            var service = new AccountAllowingListService(accountManagerGatewayMock.Object);
            var violations = service.AllowList(new AccountAllowList(true));

            Assert.Empty(violations);
            Assert.NotNull(violations.CurrentAccount);
            Assert.True(violations.CurrentAccount.AllowListed);
            
            accountManagerGatewayMock.Verify(m => m.GetCurrentAccount(), Times.Once());
        }
    }
}