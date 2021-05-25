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
    public class AccountCreationServiceTest
    {
        [Fact]
        [Trait(nameof(IAccountCreationService), "new()")]
        public void Given_A_Creation_Of_Instance_When_Pass_Null_Gateway_Should_Thrown_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AccountCreationService(null));
        }

        [Fact]
        [Trait(nameof(IAccountCreationService), nameof(IAccountCreationService.CreateAccount))]
        public void Given_A_Service_Instance_When_Pass_An_Already_Created_Account_Should_Returns_Violation_AccountAlreadyInitialized()
        {
            var manager = new Mock<IAccountRepository>();
            manager
                .Setup(m => m.GetCurrentAccount())
                .Returns(new Account(false, 50));

            var service = new AccountCreationService(manager.Object);
            var violations = service.CreateAccount(new Account(true,100));

            Assert.NotEmpty(violations);
            Assert.Single(violations);
            Assert.Equal(Violation.AccountAlreadyInitialized, violations.First());

            Assert.NotNull(violations.CurrentAccount);
            Assert.False(violations.CurrentAccount.ActiveCard);
            Assert.Equal<uint>(50, violations.CurrentAccount.AvailableLimit);

            manager.Verify(m => m.GetCurrentAccount(), Times.Once());
            manager.Verify(m => m.Add(It.IsAny<Account>()), Times.Never());
        }

        [Fact]
        [Trait(nameof(IAccountCreationService), nameof(IAccountCreationService.CreateAccount))]
        public void Given_A_Service_Instance_When_Pass_Not_Created_Accout_Should_Return_An_Empty_Violations_Collection()
        {
            var manager = new Mock<IAccountRepository>();
            manager
                .Setup(m => m.GetCurrentAccount())
                .Returns<Account>(null)
                .Verifiable();
            
            manager
                .Setup(m => m.Add(It.IsAny<Account>()))
                .Verifiable();

            var service = new AccountCreationService(manager.Object);
            var violations = service.CreateAccount(new Account(true, 100));

            Assert.Empty(violations);
            Assert.NotNull(violations.CurrentAccount);
            Assert.True(violations.CurrentAccount.ActiveCard);
            Assert.Equal<uint>(100, violations.CurrentAccount.AvailableLimit);
            
            manager.Verify(m => m.GetCurrentAccount(), Times.Once());
            manager.Verify(m => m.Add(It.IsAny<Account>()), Times.Once());
        }
    }
}