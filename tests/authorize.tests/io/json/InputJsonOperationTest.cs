using System;
using Xunit;
using Authorize.IO.Json;
using System.IO;
using System.Text;
using Authorize.Models.Json;

namespace Authorize.Tests.IO
{
    public class InputJsonOperationTest
    {
        [Fact]
        [Trait(nameof(InputJsonOperation), "new()")]
        public void Given_A_Creation_Of_InputJsonOperation_When_Pass_Null_Arg_Should_Thrown_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new InputJsonOperation(null));
        }

        [Fact]
        [Trait(nameof(InputJsonOperation), nameof(InputJsonOperation.Read))]
        public void Given_An_Instance_Calling_TryRead_For_Kind_Of_Classes_When_TextReader_Filled_Of_Two_Lines_Of_Operation_Should_Returns_Two_Operation_Objects()
        {
            var builder = new StringBuilder();
            
            builder.AppendLine("{\"account\": {\"active-card\": true, \"available-limit\": 100}}");
            builder.AppendLine("{\"transaction\": {\"merchant\": \"Burger King\", \"amount\": 20, \"time\": \"2019-02-13T11:00:00.000Z\"}}");

            var stream = new StringReader(builder.ToString());

            var input = new InputJsonOperation(stream);

            Assert.True(input.MoveNext(), "First Move Next");
            var accountOperation = input.Read<Operation>();
            Assert.True(input.MoveNext(), "Second Move Next");
            var transactionOperation = input.Read<Operation>();
            Assert.False(input.MoveNext(), "Third Move Next");

            Assert.NotNull(accountOperation);
            Assert.NotNull(accountOperation.Account);
            Assert.True(accountOperation.Account.ActiveCard);
            Assert.Equal<uint>(100, accountOperation.Account.AvailableLimit.Value);

            Assert.NotNull(transactionOperation);
            Assert.NotNull(transactionOperation.Transaction);
            Assert.Equal("Burger King", transactionOperation.Transaction.Merchant);
            Assert.Equal<uint>(20, transactionOperation.Transaction.Amount);
            Assert.Equal(DateTimeOffset.Parse("2019-02-13T11:00:00.000Z"), transactionOperation.Transaction.Time);
        }
    }
}