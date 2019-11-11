using System;
using System.Collections.Generic;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Results.Errors;
using Intellias.CQRS.Pipelines.CommandHandlers.Behaviors;

namespace Intellias.CQRS.Pipelines.CommandHandlers
{
    /// <summary>
    /// Base class for command handlers.
    /// </summary>
    public abstract class BaseCommandHandler
    {
        /// <summary>
        /// Creates Validation Failed result with some internal error.(to be moved to CQRS).
        /// </summary>
        /// <param name="codeInfo">Error code info about internal error.</param>
        /// <returns>Execution result.</returns>
        protected static IExecutionResult ValidationFailedWithCode(ErrorCodeInfo codeInfo)
        {
            return FailedResult.CreateWithInternal(CoreErrorCodes.ValidationFailed, codeInfo);
        }

        /// <summary>
        /// Creates Validation Failed result with some internal error and custom message for it.(to be moved to CQRS).
        /// </summary>
        /// <param name="codeInfo">Error code info about internal error.</param>
        /// <param name="customMessage">Custom message.</param>
        /// <returns>Execution result.</returns>
        protected static IExecutionResult ValidationFailedWithCode(ErrorCodeInfo codeInfo, string customMessage)
        {
            return FailedResult.CreateWithInternal(CoreErrorCodes.ValidationFailed, codeInfo, customMessage);
        }

        /// <summary>
        /// Creates Validation Failed result with internal errors.
        /// </summary>
        /// <param name="internalErrors">Internal errors.</param>
        /// <returns>Execution result.</returns>
        protected static IExecutionResult ValidationFailedWithDetails(IReadOnlyCollection<ExecutionError> internalErrors)
        {
            return FailedResult.ValidationFailedWith(internalErrors);
        }

        /// <summary>
        /// Creates <see cref="IntegrationEventExecutionResult"/>.
        /// </summary>
        /// <param name="context">Execution context.</param>
        /// <param name="setup">Configures integration event.</param>
        /// <typeparam name="TIntegrationEvent">Type of integration event.</typeparam>
        /// <returns>Execution result.</returns>
        protected IExecutionResult IntegrationEvent<TIntegrationEvent>(AggregateExecutionContext context, Action<TIntegrationEvent> setup)
            where TIntegrationEvent : IntegrationEvent, new()
        {
            var @event = context.CreateIntegrationEvent<TIntegrationEvent>();

            setup(@event);

            return new IntegrationEventExecutionResult(@event);
        }
    }
}