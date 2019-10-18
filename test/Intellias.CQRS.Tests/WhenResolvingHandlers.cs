﻿using System;
using System.Linq;
using Intellias.CQRS.Core.Tools;
using Intellias.CQRS.Tests.Core;
using Intellias.CQRS.Tests.Core.EventHandlers;
using Intellias.CQRS.Tests.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Intellias.CQRS.Tests
{
    public class WhenResolvingHandlers : TestBase
    {
        [Fact]
        public void ShouldReceiveDemoEventHandler()
        {
            var @event = new TestCreatedEvent();

            var resolver = ServiceProvider.GetService<HandlerDependencyResolver>();

            var result = resolver.ResolveEvent(@event).ToList();

            Assert.True(result.Any(), "Handlers not found in assembly");
            Assert.True(result.First().GetType() == typeof(DemoEventHandlers));
        }

        [Fact]
        public void ShouldReceiveDerivedEventHanderOnlyWhenRaisedDerivedEvent()
        {
            var @event = new WrappedTestCreatedEvent();

            var resolver = ServiceProvider.GetService<HandlerDependencyResolver>();

            var result = resolver.ResolveEvent(@event).ToList();

            var wrappedHandler = result.Single();
            Assert.True(wrappedHandler.GetType() == typeof(WrappedEventHandler));
        }

        [Fact]
        public void ShouldRaiseErrorOnIllegalAssembly()
        {
            var @event = new TestDeletedEvent();

            var resolver = ServiceProvider.GetService<HandlerDependencyResolver>();

            Assert.Throws<ArgumentNullException>(() =>
                resolver.ResolveEvent(@event).ToList());
        }
    }
}
