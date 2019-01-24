﻿using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Core.Tools
{
    /// <summary>
    /// Handles event for all 
    /// </summary>
    public class EventHandlerManager
        
    {
        private EventHandlerDependencyResolver Resolver { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolver"></param>
        public EventHandlerManager(EventHandlerDependencyResolver resolver)
        {
            Resolver = resolver;
        }


        /// <summary>
        /// HandleAsync
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        /// <returns></returns>
        public Task HandleAsync<T>(T @event) where T : IEvent =>
            Task.WhenAll(Resolver.Resolve(@event).Select(handler => handler.HandleAsync(@event)));
    }
}
