﻿using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Commands
{
    /// <inheritdoc cref="ICommandResult" />
    public class CommandResult : ExecutionResult, ICommandResult
    {
        /// <summary>
        /// Execution Result
        /// </summary>
        protected CommandResult() { }

        /// <summary>
        /// Execution Result
        /// </summary>
        /// <param name="failureReason">Reason of failure</param>
        protected CommandResult(string failureReason) : base(failureReason)
        {
        }

        /// <summary>
        /// Succesful result
        /// </summary>
        public static CommandResult Success { get; } = new CommandResult();

        /// <summary>
        /// Fail result
        /// </summary>
        /// <param name="reason">Reason of failure</param>
        /// <returns></returns>
        public static CommandResult Fail(string reason)
        {
            return new CommandResult(reason);
        }
    }
}
