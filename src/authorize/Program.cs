using System;
using Authorize.Infrastructure.Repositories.Memory;
using Authorize.IO.Json;
using Authorizer.Domain.Services;

namespace Authorize
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setup memory repositories
            var accountRepository = new AccountMemoryRepository();
            var transactionRepository = new TransactionMemoryRepository();
            
            // Setup service with memory repositories
            var accountService = new AccountCreationService(accountRepository);
            var authorizationService = new AuthorizationService(transactionRepository, accountRepository);

            // Setup input with Console container TextWriter and TextReader
            var input = new InputJsonOperation(Console.In);
            var output = new OutputJsonOperation(Console.Out);

            // Setup processor with all above setups variables
            var processor = new AuthorizationProcessor(input, output, accountService, authorizationService);

            // Execute authorization process
            processor.Execute();
        }
    }
}