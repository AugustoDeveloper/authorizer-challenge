using System;
using System.Collections.Generic;
using System.Linq;
using Authorize.Extensions;
using Authorize.IO;
using Authorize.Models.Json;
using Authorizer.Domain.Extensions;
using Authorizer.Domain.Services.Interfaces;

namespace Authorize
{
    /// <summary>
    /// Main processor to execute all authorization transactions
    /// and account operation process
    /// </summary>
    public class AuthorizationProcessor
    {
        private readonly IInputOperation input;
        private readonly IOutputOperation output;
        private readonly IAccountCreationService accountCreationService;
        private readonly IAuthorizationService authorizationService;
        private readonly IAccountAllowingListService accountAllowingListService;
        private readonly IReadOnlyDictionary<OperationKind, Func<Operation, AccountOperationResult>> operations;

        /// <summary>
        /// Create an instance to execute authorization process
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="accountCreationService"></param>
        /// <param name="authorizationService"></param>
        public AuthorizationProcessor(
            IInputOperation input, 
            IOutputOperation output,
            IAccountCreationService accountCreationService,
            IAuthorizationService authorizationService,
            IAccountAllowingListService accountAllowingListService)
        {
            this.input = input ?? throw new ArgumentNullException(nameof(input));
            this.output = output ?? throw new ArgumentNullException(nameof(output));
            this.accountCreationService = accountCreationService ?? throw new ArgumentNullException(nameof(accountCreationService));
            this.authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            this.accountAllowingListService = accountAllowingListService ?? throw new ArgumentNullException(nameof(accountAllowingListService));
            this.operations = new Dictionary<OperationKind, Func<Operation, AccountOperationResult>>
            {
                [OperationKind.AccountOperation] = ExecuteCreationAccount,
                [OperationKind.TransactionOperation] = ExecuteAuthorizationTransaction,
                [OperationKind.AllowListOperation] = ExecuteAllowList
            };
        }

        /// <summary>
        /// Read all data from input, process operations
        /// and write all results
        /// </summary>
        public void Execute()
        {
            while (this.input.MoveNext())
            {
                var operation = this.input.Read<Operation>();

                var kind = operation.GetOperationKind();

                var result = operations[kind](operation);

                this.output.AppendLine(result);
            }

            this.output.WriteAllResults();
        }

        /// <summary>
        /// Execute transaction authorization 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private AccountOperationResult ExecuteAuthorizationTransaction(Operation operation)
        {
            var transaction = operation.Transaction;
            var result = new AccountOperationResult();
            var violations = this.authorizationService.Authorize(transaction.ToEntity());

            result.Violations = violations.GetViolationDescriptions().ToArray();
            result.Account = violations.CurrentAccount.ToModel();
            
            return result;
        }

        private AccountOperationResult ExecuteAllowList(Operation operation)
        {
            var allowList = operation.AllowList;
            var result = new AccountOperationResult();
            var violations = this.accountAllowingListService.AllowList(allowList.ToEntity());

            result.Violations = violations.GetViolationDescriptions().ToArray();
            result.Account = violations.CurrentAccount.ToModel();
            
            return result;
        }

        /// <summary>
        /// Execute account creation operation
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private AccountOperationResult ExecuteCreationAccount(Operation operation)
        {
            var account = operation.Account;
            var result = new AccountOperationResult();
            var violations = this.accountCreationService.CreateAccount(account.ToEntity());

            result.Violations = violations.GetViolationDescriptions().ToArray();
            result.Account = violations.CurrentAccount.ToModel();
            
            return result;
        }
    }
}
