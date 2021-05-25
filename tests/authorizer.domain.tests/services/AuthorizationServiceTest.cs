using System;
using System.Collections.ObjectModel;
using System.Linq;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Repositories;
using Authorizer.Domain.Services;
using Authorizer.Domain.Services.Interfaces;
using Moq;
using Xunit;

namespace Authorizer.Domain.Tests.Services
{
    public class AuthorizationServiceTest
    {
        [Fact]
        [Trait(nameof(IAuthorizationService), "new()")]
        public void Given_A_Creation_Of_Instance_When_Pass_Null_Gateway_Should_Thrown_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AuthorizationService(null, null));
            Assert.Throws<ArgumentNullException>(() => new AuthorizationService(new Mock<ITransactionRepository>().Object, null));
            Assert.Throws<ArgumentNullException>(() => new AuthorizationService(null, new Mock<IAccountRepository>().Object));
        }

        [Fact]
        [Trait(nameof(IAuthorizationService), nameof(IAuthorizationService.Authorize))]
        public void Given_An_Instance_Calling_Authorize_When_Account_Was_Not_Created_Should_Return_Single_Violation_AccountNotInitialized()
        {
            var accountManagerGatewayMock = new Mock<IAccountRepository>();
            var transactionManagerGatewayMock = new Mock<ITransactionRepository>();
            
            accountManagerGatewayMock
                .Setup(m => m.GetCurrentAccount())
                .Returns<Account>(null)
                .Verifiable();

            var service = new AuthorizationService(transactionManagerGatewayMock.Object, accountManagerGatewayMock.Object);
            var violations = service.Authorize(new Transaction());

            Assert.NotEmpty(violations);
            Assert.Single(violations);
            Assert.Equal(Violation.AccountNotInitialized, violations.First());
            Assert.Null(violations.CurrentAccount);
            
            accountManagerGatewayMock.Verify(m => m.GetCurrentAccount(), Times.Once());
            transactionManagerGatewayMock.Verify(t => t.Add(It.IsAny<Transaction>()), Times.Never());
        }

        [Fact]
        [Trait(nameof(IAuthorizationService), nameof(IAuthorizationService.Authorize))]
        public void Given_An_Instance_Calling_Authorize_When_Card_Initialized_With_Inactive_Card_Should_Return_Single_Violation_CardNotActive()
        {
            var accountManagerGatewayMock = new Mock<IAccountRepository>();
            var transactionManagerGatewayMock = new Mock<ITransactionRepository>();
            
            accountManagerGatewayMock
                .Setup(m => m.GetCurrentAccount())
                .Returns(new Account(false, 100))
                .Verifiable();


            var service = new AuthorizationService(transactionManagerGatewayMock.Object, accountManagerGatewayMock.Object);
            var violations = service.Authorize(new Transaction());

            Assert.NotEmpty(violations);
            Assert.Single(violations);
            Assert.Equal(Violation.CardNotActive, violations.First());
            Assert.NotNull(violations.CurrentAccount);
            Assert.False(violations.CurrentAccount.ActiveCard);
            Assert.Equal<uint>(100, violations.CurrentAccount.AvailableLimit);
            
            accountManagerGatewayMock.Verify(m => m.GetCurrentAccount(), Times.Once());
            transactionManagerGatewayMock.Verify(t => t.Add(It.IsAny<Transaction>()), Times.Never());
        }

        [Fact]
        [Trait(nameof(IAuthorizationService), nameof(IAuthorizationService.Authorize))]
        public void Given_An_Instance_Calling_Authorize_When_Initialized_Account_And_Transaction_Amount_Is_Greater_Than_Limit_Should_Return_Single_Violation_InsuficientLimit()
        {
            var accountManagerGatewayMock = new Mock<IAccountRepository>();
            var transactionManagerGatewayMock = new Mock<ITransactionRepository>();
            
            accountManagerGatewayMock
                .Setup(m => m.GetCurrentAccount())
                .Returns(new Account(true, 100))
                .Verifiable();

            var service = new AuthorizationService(transactionManagerGatewayMock.Object, accountManagerGatewayMock.Object);
            var violations = service.Authorize(new Transaction("apple", 101, DateTimeOffset.UtcNow));

            Assert.NotEmpty(violations);
            Assert.Single(violations);
            Assert.Equal(Violation.InsuficientLimit, violations.First());
            Assert.NotNull(violations.CurrentAccount);
            Assert.True(violations.CurrentAccount.ActiveCard);
            Assert.Equal<uint>(100, violations.CurrentAccount.AvailableLimit);
            
            accountManagerGatewayMock.Verify(m => m.GetCurrentAccount(), Times.Once());
            transactionManagerGatewayMock.Verify(t => t.Add(It.IsAny<Transaction>()), Times.Never());
        }

        [Fact]
        [Trait(nameof(IAuthorizationService.Authorize), nameof(IAuthorizationService.Authorize))]
        public void Given_An_Instance_Calling_Authorize_When_Initialized_Account_And_There_Is_Same_Transaction_In_Two_Minutes_Interval_Should_Returns_Single_Violation_DoubleTransaction()
        {
            var accountManagerGatewayMock = new Mock<IAccountRepository>();
            var transactionManagerGatewayMock = new Mock<ITransactionRepository>();
            
            accountManagerGatewayMock
                .Setup(m => m.GetCurrentAccount())
                .Returns(new Account(true, 100))
                .Verifiable();
            
            transactionManagerGatewayMock
                .Setup(t => t.GetLastTwoMinutesTransactionsFrom(It.IsAny<DateTimeOffset>()))
                .Returns(new Collection<Transaction>
                {
                    new Transaction("apple", 10, DateTimeOffset.UtcNow),
                    new Transaction("microsoft", 11, DateTimeOffset.UtcNow)
                });

            var service = new AuthorizationService(transactionManagerGatewayMock.Object, accountManagerGatewayMock.Object);
            var violations = service.Authorize(new Transaction("apple", 10, DateTimeOffset.UtcNow));

            Assert.NotEmpty(violations);
            Assert.Single(violations);
            Assert.Equal(Violation.DoubleTransaction, violations.First());
            Assert.NotNull(violations.CurrentAccount);
            Assert.True(violations.CurrentAccount.ActiveCard);
            Assert.Equal<uint>(100, violations.CurrentAccount.AvailableLimit);
            
            accountManagerGatewayMock.Verify(m => m.GetCurrentAccount(), Times.Once());
            transactionManagerGatewayMock.Verify(t => t.GetLastTwoMinutesTransactionsFrom(It.IsAny<DateTimeOffset>()), Times.Once());
            transactionManagerGatewayMock.Verify(t => t.Add(It.IsAny<Transaction>()), Times.Never());
        }

        [Fact]
        [Trait(nameof(IAuthorizationService), nameof(IAuthorizationService.Authorize))]
        public void Given_An_Instance_Calling_Authorize_When_Initialized_Account_And_There_Is_Frequency_Transaction_In_Small_Interval_Should_Returns_Single_Violation_HighFrquencySmallInterval()
        {
            var accountManagerGatewayMock = new Mock<IAccountRepository>();
            var transactionManagerGatewayMock = new Mock<ITransactionRepository>();
            
            accountManagerGatewayMock
                .Setup(m => m.GetCurrentAccount())
                .Returns(new Account(true, 100))
                .Verifiable();
            
            transactionManagerGatewayMock
                .Setup(t => t.GetLastTwoMinutesTransactionsFrom(It.IsAny<DateTimeOffset>()))
                .Returns(new Collection<Transaction>
                {
                    new Transaction("apple", 10, DateTimeOffset.UtcNow),
                    new Transaction("microsoft", 10, DateTimeOffset.UtcNow),
                    new Transaction("amazon", 10, DateTimeOffset.UtcNow),
                });

            var service = new AuthorizationService(transactionManagerGatewayMock.Object, accountManagerGatewayMock.Object);
            var violations = service.Authorize(new Transaction("ebay", 10, DateTimeOffset.UtcNow));

            Assert.NotEmpty(violations);
            Assert.Single(violations);
            Assert.Equal(Violation.HighFrequencySmallInterval, violations.First());
            Assert.NotNull(violations.CurrentAccount);
            Assert.True(violations.CurrentAccount.ActiveCard);
            Assert.Equal<uint>(100, violations.CurrentAccount.AvailableLimit);
            
            accountManagerGatewayMock.Verify(m => m.GetCurrentAccount(), Times.Once());
            transactionManagerGatewayMock.Verify(t => t.GetLastTwoMinutesTransactionsFrom(It.IsAny<DateTimeOffset>()), Times.Once());
            transactionManagerGatewayMock.Verify(t => t.Add(It.IsAny<Transaction>()), Times.Never());
        }

        [Fact]
        [Trait(nameof(IAuthorizationService), nameof(IAuthorizationService.Authorize))]
        public void Given_An_Instance_Calling_Authorize_When_Initialized_Account_And_There_Is_Frequency_Transaction_In_Small_Interval_With_Double_Transaction_Should_Returns_Two_Violations_HighFrquencySmallInterval_And_DoubleTransaction()
        {
            var accountManagerGatewayMock = new Mock<IAccountRepository>();
            var transactionManagerGatewayMock = new Mock<ITransactionRepository>();
            
            accountManagerGatewayMock
                .Setup(m => m.GetCurrentAccount())
                .Returns(new Account(true, 100))
                .Verifiable();
            
            transactionManagerGatewayMock
                .Setup(t => t.GetLastTwoMinutesTransactionsFrom(It.IsAny<DateTimeOffset>()))
                .Returns(new Collection<Transaction>
                {
                    new Transaction("apple", 10, DateTimeOffset.UtcNow),
                    new Transaction("microsoft", 10, DateTimeOffset.UtcNow),
                    new Transaction("amazon", 10, DateTimeOffset.UtcNow),
                });

            var service = new AuthorizationService(transactionManagerGatewayMock.Object, accountManagerGatewayMock.Object);
            var violations = service.Authorize(new Transaction("amazon", 10, DateTimeOffset.UtcNow));

            Assert.NotEmpty(violations);
            Assert.Contains(Violation.HighFrequencySmallInterval, violations);
            Assert.Contains(Violation.DoubleTransaction, violations);
            Assert.True(violations.Count == 2);
            Assert.NotNull(violations.CurrentAccount);
            Assert.True(violations.CurrentAccount.ActiveCard);
            Assert.Equal<uint>(100, violations.CurrentAccount.AvailableLimit);
            
            accountManagerGatewayMock.Verify(m => m.GetCurrentAccount(), Times.Once());
            transactionManagerGatewayMock.Verify(t => t.GetLastTwoMinutesTransactionsFrom(It.IsAny<DateTimeOffset>()), Times.Once());
            transactionManagerGatewayMock.Verify(t => t.Add(It.IsAny<Transaction>()), Times.Never());
        }

        [Fact]
        [Trait(nameof(IAuthorizationService), nameof(IAuthorizationService.Authorize))]
        public void Given_An_Instance_Calling_Authorize_When_Transaction_Is_Ok_To_Authorize_Should_Return_Empty_Violation()
        {
            var accountManagerGatewayMock = new Mock<IAccountRepository>();
            var transactionManagerGatewayMock = new Mock<ITransactionRepository>();
            
            accountManagerGatewayMock
                .Setup(m => m.GetCurrentAccount())
                .Returns(new Account(true, 100))
                .Verifiable();
            
            transactionManagerGatewayMock
                .Setup(t => t.GetLastTwoMinutesTransactionsFrom(It.IsAny<DateTimeOffset>()))
                .Returns(new Collection<Transaction>
                {
                    new Transaction("apple", 10, DateTimeOffset.UtcNow),
                    new Transaction("microsoft", 10, DateTimeOffset.UtcNow),
                });

            var service = new AuthorizationService(transactionManagerGatewayMock.Object, accountManagerGatewayMock.Object);
            var violations = service.Authorize(new Transaction("amazon", 10, DateTimeOffset.UtcNow.AddMinutes(3)));

            Assert.Empty(violations);
            Assert.NotNull(violations.CurrentAccount);
            Assert.True(violations.CurrentAccount.ActiveCard);
            Assert.Equal<uint>(90, violations.CurrentAccount.AvailableLimit);
            
            accountManagerGatewayMock.Verify(m => m.GetCurrentAccount(), Times.Once());
            transactionManagerGatewayMock.Verify(t => t.GetLastTwoMinutesTransactionsFrom(It.IsAny<DateTimeOffset>()), Times.Once());
            transactionManagerGatewayMock.Verify(t => t.Add(It.IsAny<Transaction>()), Times.Once());
        }
    }
}