using System;
using System.IO;
using System.Text;
using Authorize.Infrastructure.Repositories.Memory;
using Authorize.IO;
using Authorize.IO.Json;
using Authorize.Models.Json;
using Authorizer.Domain.Services;
using Authorizer.Domain.Services.Interfaces;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Authorize.Tests
{
    public class AuthorizationProcessorTest
    {
        private readonly ITestOutputHelper output;

        public AuthorizationProcessorTest(ITestOutputHelper output)
        {
            this.output = output;
        }
        [Fact]
        [Trait(nameof(AuthorizationProcessor), "new()")]
        public void Given_Calling_Consutructor_AuthorizationProcessor_When_Pass_Invalid_Args_Should_Thrown_ArgumentNullException()
        {
            var inputMock = new Mock<IInputOperation>();
            var outputMock = new Mock<IOutputOperation>();
            var accountServiceMock = new Mock<IAccountCreationService>();
            var transactionServiceMock = new Mock<IAuthorizationService>();
            var accountAllowListServiceMock = new Mock<IAccountAllowingListService>();

            Assert.Throws<ArgumentNullException>(() => new AuthorizationProcessor(null,null,null,null,null));
            Assert.Throws<ArgumentNullException>(() => new AuthorizationProcessor(inputMock.Object,null,null,null, null));
            Assert.Throws<ArgumentNullException>(() => new AuthorizationProcessor(inputMock.Object,outputMock.Object,null,null, null));
            Assert.Throws<ArgumentNullException>(() => new AuthorizationProcessor(inputMock.Object,outputMock.Object,accountServiceMock.Object,null, null));
            Assert.Throws<ArgumentNullException>(() => new AuthorizationProcessor(inputMock.Object,outputMock.Object,accountServiceMock.Object,transactionServiceMock.Object, null));

        }

        [Fact]
        [Trait(nameof(AuthorizationProcessor), nameof(AuthorizationProcessor.Execute))]
        public void Given_An_Instance_Calling_Execute_When_One_Line_Text_Of_Creation_Account_In_Input_Should_Returns_Empty_Violation_With_An_Account_Created()
        {
            var accountRepository = new AccountMemoryRepository();
            var transactionRepository = new TransactionMemoryRepository();
            
            var accountService = new AccountCreationService(accountRepository);
            var authorizationService = new AuthorizationService(transactionRepository, accountRepository);
            var accountAllowingListService = new AccountAllowingListService(accountRepository);

            var inputBuilder = new StringBuilder();
            inputBuilder.Append("{\"account\": {\"active-card\": false, \"available-limit\": 750}}");

            var reader = new StringReader(inputBuilder.ToString());

            var outputBuilder = new StringBuilder();
            var writer = new StringWriter(outputBuilder);

            var input = new InputJsonOperation(reader);
            var output = new OutputJsonOperation(writer);

            var processor = new AuthorizationProcessor(input, output, accountService, authorizationService, accountAllowingListService);
            processor.Execute();

            var outputText = "{\"account\":{\"active-card\":false,\"available-limit\":750},\"violations\":[]}";

            Assert.Equal(outputText, outputBuilder.ToString());
        }

        [Fact]
        [Trait(nameof(AuthorizationProcessor), nameof(AuthorizationProcessor.Execute))]
        public void Given_An_Instance_Calling_Execute_When_Pass_Double_Creation_Accout_Operation_Should_Returns_One_Result_Without_Violation_And_Another_One_With_AccountAlready_Initialized_Violation()
        {
            var accountRepository = new AccountMemoryRepository();
            var transactionRepository = new TransactionMemoryRepository();
            
            var accountService = new AccountCreationService(accountRepository);
            var authorizationService = new AuthorizationService(transactionRepository, accountRepository);
            var accountAllowingListService = new AccountAllowingListService(accountRepository);

            var inputBuilder = new StringBuilder();
            inputBuilder.AppendLine("{\"account\": {\"active-card\": false, \"available-limit\": 175}}");
            inputBuilder.Append("{\"account\": {\"active-card\": false, \"available-limit\": 350}}");

            var reader = new StringReader(inputBuilder.ToString());

            var outputBuilder = new StringBuilder();
            var writer = new StringWriter(outputBuilder);

            var input = new InputJsonOperation(reader);
            var output = new OutputJsonOperation(writer);

            var processor = new AuthorizationProcessor(input, output, accountService, authorizationService, accountAllowingListService);
            processor.Execute();

            var outputText = "{\"account\":{\"active-card\":false,\"available-limit\":175},\"violations\":[]}" + Environment.NewLine +
                             "{\"account\":{\"active-card\":false,\"available-limit\":175},\"violations\":[\"account-already-initialized\"]}";

            Assert.Equal(outputText, outputBuilder.ToString());
        }

        [Fact]
        [Trait(nameof(AuthorizationProcessor), nameof(AuthorizationProcessor.Execute))]
        public void Given_An_Instance_Calling_Execute_When_Pass_One_Account_And_One_Transaction_Operation_Should_Returns_Two_Operation_Results_With_Empty_Violation()
        {
            var accountRepository = new AccountMemoryRepository();
            var transactionRepository = new TransactionMemoryRepository();
            
            var accountService = new AccountCreationService(accountRepository);
            var authorizationService = new AuthorizationService(transactionRepository, accountRepository);
            var accountAllowingListService = new AccountAllowingListService(accountRepository);

            var inputBuilder = new StringBuilder();
            inputBuilder.AppendLine("{\"account\": {\"active-card\": true, \"available-limit\": 100}}");
            inputBuilder.Append("{\"transaction\": {\"merchant\": \"Burger King\", \"amount\": 20, \"time\": \"2019-02-13T11:00:00.000Z\"}}");

            var reader = new StringReader(inputBuilder.ToString());

            var outputBuilder = new StringBuilder();
            var writer = new StringWriter(outputBuilder);

            var input = new InputJsonOperation(reader);
            var output = new OutputJsonOperation(writer);

            var processor = new AuthorizationProcessor(input, output, accountService, authorizationService, accountAllowingListService);
            processor.Execute();

            var outputText = "{\"account\":{\"active-card\":true,\"available-limit\":100},\"violations\":[]}" + Environment.NewLine +
                             "{\"account\":{\"active-card\":true,\"available-limit\":80},\"violations\":[]}";

            Assert.Equal(outputText, outputBuilder.ToString());
        }

        [Fact]
        [Trait(nameof(AuthorizationProcessor), nameof(AuthorizationProcessor.Execute))]
        public void Given_An_Instance_Calling_Execute_When_Pass_One_Transaction_Operation_And_Account_Was_Not_Created_After_That_Created_Account_And_One_Transaction_Operation_Should_Returns_Three_Operation_Results_And_One_Violation_AccountNotInitilized()
        {
            var accountRepository = new AccountMemoryRepository();
            var transactionRepository = new TransactionMemoryRepository();
            
            var accountService = new AccountCreationService(accountRepository);
            var authorizationService = new AuthorizationService(transactionRepository, accountRepository);
            var accountAllowingListService = new AccountAllowingListService(accountRepository);

            var inputBuilder = new StringBuilder();
            inputBuilder.AppendLine("{\"transaction\": {\"merchant\": \"Uber Eats\", \"amount\": 25, \"time\": \"2020-12-01T11:07:00.000Z\"}}");
            inputBuilder.AppendLine("{\"account\": {\"active-card\": true, \"available-limit\": 225}}");
            inputBuilder.Append("{\"transaction\": {\"merchant\": \"Uber Eats\", \"amount\": 25, \"time\": \"2020-12-01T11:07:00.000Z\"}}");

            var reader = new StringReader(inputBuilder.ToString());

            var outputBuilder = new StringBuilder();
            var writer = new StringWriter(outputBuilder);

            var input = new InputJsonOperation(reader);
            var output = new OutputJsonOperation(writer);

            var processor = new AuthorizationProcessor(input, output, accountService, authorizationService, accountAllowingListService);
            processor.Execute();

            var outputText = "{\"account\":{},\"violations\":[\"account-not-initialized\"]}" + Environment.NewLine +
                             "{\"account\":{\"active-card\":true,\"available-limit\":225},\"violations\":[]}" + Environment.NewLine +
                             "{\"account\":{\"active-card\":true,\"available-limit\":200},\"violations\":[]}";

            Assert.Equal(outputText, outputBuilder.ToString());
        }

        [Fact]
        [Trait(nameof(AuthorizationProcessor), nameof(AuthorizationProcessor.Execute))]
        public void Given_An_Instance_Calling_Execute_When_Pass_One_Account_Operation_With_Card_Inactive_And_One_Transaction_Operation_Should_Returns_Two_OperationResults_With_One_Violation_CardIsNotActive()
        {
            var accountRepository = new AccountMemoryRepository();
            var transactionRepository = new TransactionMemoryRepository();
            
            var accountService = new AccountCreationService(accountRepository);
            var authorizationService = new AuthorizationService(transactionRepository, accountRepository);
            var accountAllowingListService = new AccountAllowingListService(accountRepository);

            var inputBuilder = new StringBuilder();
            inputBuilder.AppendLine("{\"account\": {\"active-card\": false, \"available-limit\": 225}}");
            inputBuilder.Append("{\"transaction\": {\"merchant\": \"Uber Eats\", \"amount\": 25, \"time\": \"2020-12-01T11:07:00.000Z\"}}");

            var reader = new StringReader(inputBuilder.ToString());

            var outputBuilder = new StringBuilder();
            var writer = new StringWriter(outputBuilder);

            var input = new InputJsonOperation(reader);
            var output = new OutputJsonOperation(writer);

            var processor = new AuthorizationProcessor(input, output, accountService, authorizationService, accountAllowingListService);
            processor.Execute();

            var outputText = "{\"account\":{\"active-card\":false,\"available-limit\":225},\"violations\":[]}" + Environment.NewLine +
                             "{\"account\":{\"active-card\":false,\"available-limit\":225},\"violations\":[\"card-not-active\"]}";

            Assert.Equal(outputText, outputBuilder.ToString());
        }

        [Fact]
        [Trait(nameof(AuthorizationProcessor), nameof(AuthorizationProcessor.Execute))]
        public void Given_An_Instance_Calling_Execute_When_Pass_One_Account_Operation_And_One_Transaction_Operation_With_Amount_Greater_Than_AvailableLimit_On_Account_Should_Returns_Two_OperationResults_With_One_Violation_InsufficientLimit()
        {
            var accountRepository = new AccountMemoryRepository();
            var transactionRepository = new TransactionMemoryRepository();
            
            var accountService = new AccountCreationService(accountRepository);
            var authorizationService = new AuthorizationService(transactionRepository, accountRepository);
            var accountAllowingListService = new AccountAllowingListService(accountRepository);

            var inputBuilder = new StringBuilder();
            inputBuilder.AppendLine("{\"account\": {\"active-card\": true, \"available-limit\": 225}}");
            inputBuilder.Append("{\"transaction\": {\"merchant\": \"Uber Eats\", \"amount\": 250, \"time\": \"2020-12-01T11:07:00.000Z\"}}");

            var reader = new StringReader(inputBuilder.ToString());

            var outputBuilder = new StringBuilder();
            var writer = new StringWriter(outputBuilder);

            var input = new InputJsonOperation(reader);
            var output = new OutputJsonOperation(writer);

            var processor = new AuthorizationProcessor(input, output, accountService, authorizationService, accountAllowingListService);
            processor.Execute();

            var outputText = "{\"account\":{\"active-card\":true,\"available-limit\":225},\"violations\":[]}" + Environment.NewLine +
                             "{\"account\":{\"active-card\":true,\"available-limit\":225},\"violations\":[\"insufficient-limit\"]}";

            Assert.Equal(outputText, outputBuilder.ToString());
        }

        [Fact]
        [Trait(nameof(AuthorizationProcessor), nameof(AuthorizationProcessor.Execute))]
        public void Given_An_Instance_Calling_Execute_When_Pass_One_Account_Operation_And_Four_Transactions_Operations_With_Small_Interval_Time_Should_Returns_Five_OperationResults_With_One_Violation_HighFrequencySmallInterval()
        {
            var accountRepository = new AccountMemoryRepository();
            var transactionRepository = new TransactionMemoryRepository();
            
            var accountService = new AccountCreationService(accountRepository);
            var authorizationService = new AuthorizationService(transactionRepository, accountRepository);
            var accountAllowingListService = new AccountAllowingListService(accountRepository);

            var inputBuilder = new StringBuilder();
            inputBuilder.AppendLine("{\"account\": {\"active-card\": true, \"available-limit\": 225}}");
            inputBuilder.AppendLine("{\"transaction\": {\"merchant\": \"Uber Eats\", \"amount\": 1, \"time\": \"2020-12-01T11:07:00.000Z\"}}");
            inputBuilder.AppendLine("{\"transaction\": {\"merchant\": \"Uber Eats\", \"amount\": 2, \"time\": \"2020-12-01T11:07:30.000Z\"}}");
            inputBuilder.AppendLine("{\"transaction\": {\"merchant\": \"Uber Eats\", \"amount\": 3, \"time\": \"2020-12-01T11:08:00.000Z\"}}");
            inputBuilder.Append("{\"transaction\": {\"merchant\": \"Uber Eats\", \"amount\": 4, \"time\": \"2020-12-01T11:09:00.000Z\"}}");

            var reader = new StringReader(inputBuilder.ToString());

            var outputBuilder = new StringBuilder();
            var writer = new StringWriter(outputBuilder);

            var input = new InputJsonOperation(reader);
            var output = new OutputJsonOperation(writer);

            var processor = new AuthorizationProcessor(input, output, accountService, authorizationService, accountAllowingListService);
            processor.Execute();

            var outputText = "{\"account\":{\"active-card\":true,\"available-limit\":225},\"violations\":[]}" + Environment.NewLine +
                             "{\"account\":{\"active-card\":true,\"available-limit\":224},\"violations\":[]}" + Environment.NewLine +
                             "{\"account\":{\"active-card\":true,\"available-limit\":222},\"violations\":[]}" + Environment.NewLine +
                             "{\"account\":{\"active-card\":true,\"available-limit\":219},\"violations\":[]}" + Environment.NewLine +
                             "{\"account\":{\"active-card\":true,\"available-limit\":219},\"violations\":[\"high-frequency-small-interval\"]}";

            Assert.Equal(outputText, outputBuilder.ToString());
        }

        [Fact]
        [Trait(nameof(AuthorizationProcessor), nameof(AuthorizationProcessor.Execute))]
        public void Given_An_Instance_Calling_Execute_When_Pass_One_Account_Operation_And_Two_Equal_Transactions_Operations_With_Small_Interval_Time_Should_Returns_Five_OperationResults_With_One_Violation_DoubleTransaction()
        {
            var accountRepository = new AccountMemoryRepository();
            var transactionRepository = new TransactionMemoryRepository();
            
            var accountService = new AccountCreationService(accountRepository);
            var authorizationService = new AuthorizationService(transactionRepository, accountRepository);
            var accountAllowingListService = new AccountAllowingListService(accountRepository);

            var inputBuilder = new StringBuilder();
            inputBuilder.AppendLine("{\"account\": {\"active-card\": true, \"available-limit\": 225}}");
            inputBuilder.AppendLine("{\"transaction\": {\"merchant\": \"Uber Eats\", \"amount\": 1, \"time\": \"2020-12-01T11:07:00.000Z\"}}");
            inputBuilder.Append("{\"transaction\": {\"merchant\": \"Uber Eats\", \"amount\": 1, \"time\": \"2020-12-01T11:09:00.000Z\"}}");

            var reader = new StringReader(inputBuilder.ToString());

            var outputBuilder = new StringBuilder();
            var writer = new StringWriter(outputBuilder);

            var input = new InputJsonOperation(reader);
            var output = new OutputJsonOperation(writer);

            var processor = new AuthorizationProcessor(input, output, accountService, authorizationService, accountAllowingListService);
            processor.Execute();

            var outputText = "{\"account\":{\"active-card\":true,\"available-limit\":225},\"violations\":[]}" + Environment.NewLine +
                             "{\"account\":{\"active-card\":true,\"available-limit\":224},\"violations\":[]}" + Environment.NewLine +
                             "{\"account\":{\"active-card\":true,\"available-limit\":224},\"violations\":[\"double-transaction\"]}";

            Assert.Equal(outputText, outputBuilder.ToString());
        }

        [Fact]
        [Trait(nameof(AuthorizationProcessor), nameof(AuthorizationProcessor.Execute))]
        public void Given_An_Instance_Calling_Execute_When_Pass_One_Account_Operation_And_Four_Transactions_Operations_With_Small_Interval_Time_And_Last_One_Is_Double_Should_Returns_Five_OperationResults_With_Two_Violations_HighFrequencySmallInterval_DoubleTransaction()
        {
            var accountRepository = new AccountMemoryRepository();
            var transactionRepository = new TransactionMemoryRepository();
            
            var accountService = new AccountCreationService(accountRepository);
            var authorizationService = new AuthorizationService(transactionRepository, accountRepository);
            var accountAllowingListService = new AccountAllowingListService(accountRepository);

            var inputBuilder = new StringBuilder();
            inputBuilder.AppendLine("{\"account\": {\"active-card\": true, \"available-limit\": 225}}");
            inputBuilder.AppendLine("{\"transaction\": {\"merchant\": \"Uber Eats\", \"amount\": 1, \"time\": \"2020-12-01T11:07:00.000Z\"}}");
            inputBuilder.AppendLine("{\"transaction\": {\"merchant\": \"Uber Eats\", \"amount\": 2, \"time\": \"2020-12-01T11:07:30.000Z\"}}");
            inputBuilder.AppendLine("{\"transaction\": {\"merchant\": \"Uber Eats\", \"amount\": 3, \"time\": \"2020-12-01T11:08:00.000Z\"}}");
            inputBuilder.Append("{\"transaction\": {\"merchant\": \"Uber Eats\", \"amount\": 1, \"time\": \"2020-12-01T11:09:00.000Z\"}}");

            var reader = new StringReader(inputBuilder.ToString());

            var outputBuilder = new StringBuilder();
            var writer = new StringWriter(outputBuilder);

            var input = new InputJsonOperation(reader);
            var output = new OutputJsonOperation(writer);

            var processor = new AuthorizationProcessor(input, output, accountService, authorizationService, accountAllowingListService);
            processor.Execute();

            var outputText = "{\"account\":{\"active-card\":true,\"available-limit\":225},\"violations\":[]}" + Environment.NewLine +
                             "{\"account\":{\"active-card\":true,\"available-limit\":224},\"violations\":[]}" + Environment.NewLine +
                             "{\"account\":{\"active-card\":true,\"available-limit\":222},\"violations\":[]}" + Environment.NewLine +
                             "{\"account\":{\"active-card\":true,\"available-limit\":219},\"violations\":[]}" + Environment.NewLine +
                             "{\"account\":{\"active-card\":true,\"available-limit\":219},\"violations\":[\"high-frequency-small-interval\",\"double-transaction\"]}";

            Assert.Equal(outputText, outputBuilder.ToString());
        }

        [Fact]
        [Trait(nameof(AuthorizationProcessor), nameof(AuthorizationProcessor.Execute))]
        public void Given_An_Instance_Calling_Execute_When_Pass_One_Account_Operation_And_Four_Transactions_Operations_With_Amount_Greater_Than_AvailableLimit_Returns_Five_OperationResults_With_One_Violation_InsuficientLimit()
        {
            var accountRepository = new AccountMemoryRepository();
            var transactionRepository = new TransactionMemoryRepository();
            
            var accountService = new AccountCreationService(accountRepository);
            var authorizationService = new AuthorizationService(transactionRepository, accountRepository);
            var accountAllowingListService = new AccountAllowingListService(accountRepository);

            var inputBuilder = new StringBuilder();
            inputBuilder.AppendLine("{\"account\": {\"active-card\": true, \"available-limit\": 1000}}");
            inputBuilder.AppendLine("{\"transaction\": {\"merchant\": \"Vivara\", \"amount\": 1250, \"time\": \"2019-02-13T11:00:00.000Z\"}}");
            inputBuilder.AppendLine("{\"transaction\": {\"merchant\": \"Samsung\", \"amount\": 2500, \"time\": \"2019-02-13T11:00:01.000Z\"}}");
            inputBuilder.AppendLine("{\"transaction\": {\"merchant\": \"Nike\", \"amount\": 800, \"time\": \"2019-02-13T11:01:01.000Z\"}}");
            inputBuilder.Append("{\"transaction\": {\"merchant\": \"Uber\", \"amount\": 80, \"time\": \"2019-02-13T11:01:31.000Z\"}}");

            var reader = new StringReader(inputBuilder.ToString());

            var outputBuilder = new StringBuilder();
            var writer = new StringWriter(outputBuilder);

            var input = new InputJsonOperation(reader);
            var output = new OutputJsonOperation(writer);

            var processor = new AuthorizationProcessor(input, output, accountService, authorizationService, accountAllowingListService);
            processor.Execute();

            var outputText = "{\"account\":{\"active-card\":true,\"available-limit\":1000},\"violations\":[]}" + Environment.NewLine +
                             "{\"account\":{\"active-card\":true,\"available-limit\":1000},\"violations\":[\"insufficient-limit\"]}" + Environment.NewLine +
                             "{\"account\":{\"active-card\":true,\"available-limit\":1000},\"violations\":[\"insufficient-limit\"]}" + Environment.NewLine +
                             "{\"account\":{\"active-card\":true,\"available-limit\":200},\"violations\":[]}" + Environment.NewLine +
                             "{\"account\":{\"active-card\":true,\"available-limit\":120},\"violations\":[]}";

            Assert.Equal(outputText, outputBuilder.ToString());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        [InlineData(1_000_000)]
        [InlineData(5_000_000)]
        [Trait(nameof(AuthorizationProcessor), nameof(AuthorizationProcessor.Execute))]
        public void Given_An_Instance_Calling_Execute_When_Pass_One_Account_And_Configured_Differents_Transactions_With_One_Minute_Range_Should_Returns_Configured_OperationResults_Plus_One_Account_Operation_With_Empty_Violation(int totalTransaction)
        {
            this.output.WriteLine(totalTransaction.ToString());
            var accountRepository = new AccountMemoryRepository();
            var transactionRepository = new TransactionMemoryRepository();
            
            var accountService = new AccountCreationService(accountRepository);
            var authorizationService = new AuthorizationService(transactionRepository, accountRepository);
            var accountAllowingListService = new AccountAllowingListService(accountRepository);

            var inputBuilder = new StringBuilder();
            var outputBuilder = new StringBuilder();
            inputBuilder.Append("{\"account\": {\"active-card\": true, \"available-limit\": "+totalTransaction+"}}");
            outputBuilder.Append("{\"account\":{\"active-card\":true,\"available-limit\":"+totalTransaction+"},\"violations\":[]}");

            var time = DateTimeOffset.UtcNow;

            for(int i = 0;i < totalTransaction;i++)
            {
                var merchant = Guid.NewGuid().ToString();
                var amount = totalTransaction - (i+1);
                time = time.AddMinutes(1);
                var timeFormatted = time.ToString("O");
                inputBuilder.Append(Environment.NewLine +"{\"transaction\": {\"merchant\": \""+merchant+"\", \"amount\": 1, \"time\": \""+timeFormatted+"\"}}");
                outputBuilder.Append(Environment.NewLine + "{\"account\":{\"active-card\":true,\"available-limit\":"+amount+"},\"violations\":[]}");
            }

            var reader = new StringReader(inputBuilder.ToString());

            var outputExpected = outputBuilder.ToString();
            outputBuilder = new StringBuilder();
            var writer = new StringWriter(outputBuilder);

            var input = new InputJsonOperation(reader);
            var output = new OutputJsonOperation(writer);

            var processor = new AuthorizationProcessor(input, output, accountService, authorizationService, accountAllowingListService);
            processor.Execute();

            Assert.Equal(outputExpected, outputBuilder.ToString());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        [InlineData(1_000_000)]
        [InlineData(5_000_000)]
        [Trait(nameof(AuthorizationProcessor), nameof(AuthorizationProcessor.Execute))]
        public void Given_An_Instance_Calling_Execute_When_Pass_One_Account_With_Insuficient_Limit_And_Configured_Differents_Transactions_With_One_Minute_Range_Should_Returns_Configured_OperationResults_Plus_One_Account_Operation_With_One_Violation_InsufficientLimit(int totalTransaction)
        {
            this.output.WriteLine(totalTransaction.ToString());
            var accountRepository = new AccountMemoryRepository();
            var transactionRepository = new TransactionMemoryRepository();
            
            var accountService = new AccountCreationService(accountRepository);
            var authorizationService = new AuthorizationService(transactionRepository, accountRepository);
            var accountAllowingListService = new AccountAllowingListService(accountRepository);

            var inputBuilder = new StringBuilder();
            var outputBuilder = new StringBuilder();
            inputBuilder.Append("{\"account\": {\"active-card\": true, \"available-limit\": 0}}");
            outputBuilder.Append("{\"account\":{\"active-card\":true,\"available-limit\":0},\"violations\":[]}");

            var time = DateTimeOffset.UtcNow;

            for(int i = 0;i < totalTransaction;i++)
            {
                var merchant = Guid.NewGuid().ToString();
                var amount = totalTransaction - (i+1);
                time = time.AddMinutes(1);
                var timeFormatted = time.ToString("O");
                inputBuilder.Append(Environment.NewLine +"{\"transaction\": {\"merchant\": \""+merchant+"\", \"amount\": 1, \"time\": \""+timeFormatted+"\"}}");
                outputBuilder.Append(Environment.NewLine + "{\"account\":{\"active-card\":true,\"available-limit\":0},\"violations\":[\"insufficient-limit\"]}");
            }

            var reader = new StringReader(inputBuilder.ToString());

            var outputExpected = outputBuilder.ToString();
            outputBuilder = new StringBuilder();
            var writer = new StringWriter(outputBuilder);

            var input = new InputJsonOperation(reader);
            var output = new OutputJsonOperation(writer);

            var processor = new AuthorizationProcessor(input, output, accountService, authorizationService, accountAllowingListService);
            processor.Execute();

            Assert.Equal(outputExpected, outputBuilder.ToString());
        }

        // [Theory]
        // [InlineData(2_000_000)]
        // [Trait(nameof(AuthorizationProcessor), nameof(AuthorizationProcessor.Execute))]
        // public void GenerateFileForExecutionTest(int totalTransaction)
        // {
        //     var inputBuilder = new StringBuilder();
        //     var outputBuilder = new StringBuilder();
        //     inputBuilder.Append("{\"account\": {\"active-card\": true, \"available-limit\": "+totalTransaction+"}}");

        //     var time = DateTimeOffset.UtcNow;

        //     for(int i = 0;i < totalTransaction;i++)
        //     {
        //         var merchant = Guid.NewGuid().ToString();
        //         var amount = totalTransaction - (i+1);
        //         time = time.AddMinutes(1);
        //         var timeFormatted = time.ToString("O");
        //         inputBuilder.Append(Environment.NewLine +"{\"transaction\": {\"merchant\": \""+merchant+"\", \"amount\": 1, \"time\": \""+timeFormatted+"\"}}");
        //     }

        //     File.WriteAllText("output-_000_00", inputBuilder.ToString());
        // }
    }
}