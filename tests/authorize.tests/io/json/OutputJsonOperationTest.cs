using System;
using Xunit;
using Authorize.IO.Json;
using System.IO;
using System.Text;
using Authorize.Models.Json;

namespace Authorize.Tests.IO
{
    public class OutputJsonOperationTest
    {
        [Fact]
        [Trait(nameof(OutputJsonOperation), "new()")]
        public void Given_A_Creation_Of_OutputJsonOperation_When_Pass_Null_Arg_Should_Thrown_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new OutputJsonOperation(null));
        }

        [Fact]
        [Trait(nameof(OutputJsonOperation), nameof(OutputJsonOperation.WriteAllResults))]
        public void Given_An_Instance_Calling_WriteAllResults_When_Append_Two_Result_Should_Returns_Serialized_Json()
        {
            var results = new[]
            {
                new AccountOperationResult
                {
                    Account = new Account
                    {
                        ActiveCard = true,
                        AvailableLimit = 40
                    },   
                },

                new AccountOperationResult
                {
                    Account = new Account
                    {
                        ActiveCard = true,
                        AvailableLimit = 40
                    },
                    Violations = new string[] { "high-frequency-small-interval" }
                },
            };

            var builder = new StringBuilder();
            var writer = new StringWriter(builder);
            var operation = new OutputJsonOperation(writer);
            
            operation.AppendLine(results[0]);
            operation.AppendLine(results[1]);

            operation.WriteAllResults();

            var output = "{\"account\":{\"active-card\":true,\"available-limit\":40},\"violations\":[]}"+Environment.NewLine+
                         "{\"account\":{\"active-card\":true,\"available-limit\":40},\"violations\":[\"high-frequency-small-interval\"]}";

            Assert.Equal(output, builder.ToString());
        }

        [Fact]
        [Trait(nameof(OutputJsonOperation), nameof(OutputJsonOperation.WriteAllResults))]
        public void Given_An_Instance_Calling_WriteAllResult_When_Append_Empty_Account_Instance_Should_Returns_Empty_Account_Section()
        {
            var results = new[]
            {
                new AccountOperationResult
                {
                    Account = new Account()
                },
            };

            var builder = new StringBuilder();
            var writer = new StringWriter(builder);
            var operation = new OutputJsonOperation(writer);
            
            operation.AppendLine(results[0]);

            operation.WriteAllResults();

            var output = "{\"account\":{},\"violations\":[]}";

            Assert.Equal(output, builder.ToString());
        }
    }
}