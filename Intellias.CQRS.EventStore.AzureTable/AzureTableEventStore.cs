﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.EventStore.AzureTable.Documents;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.EventStore.AzureTable
{
    /// <summary>
    /// Azure Table Storage event store.
    /// </summary>
    public class AzureTableEventStore : IEventStore
    {
        private readonly CloudTable eventTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTableEventStore"/> class.
        /// </summary>
        /// <param name="account">Azure Table Storage Account.</param>
        public AzureTableEventStore(CloudStorageAccount account)
        {
            var client = account.CreateCloudTableClient();

            eventTable = client.GetTableReference(nameof(EventStore));

            if (!eventTable.ExistsAsync().GetAwaiter().GetResult())
            {
                eventTable.CreateIfNotExistsAsync().Wait();
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IEvent>> SaveAsync(IAggregateRoot entity)
        {
            if (!entity.Events.Any())
            {
                throw new InvalidOperationException("No events for serialization.");
            }

            var batchOperation = new TableBatchOperation();
            entity.Events.ToList().ForEach(e => batchOperation.Insert(new EventStoreEvent(e)));
            await eventTable.ExecuteBatchAsync(batchOperation);

            return entity.Events;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IEvent>> GetAsync(string aggregateId, int fromVersion)
        {
            var query = new TableQuery<EventStoreEvent>()
                .Where(TableQuery.GenerateFilterCondition(
                    "PartitionKey",
                    QueryComparisons.Equal,
                    aggregateId));

            var results = new List<EventStoreEvent>();
            var continuationToken = new TableContinuationToken();

            do
            {
                var queryResults = await eventTable.ExecuteQuerySegmentedAsync(query, continuationToken);

                if (!queryResults.Results.Any())
                {
                    throw new KeyNotFoundException($"Aggregate Root with id = '{aggregateId}' hasn't been found");
                }

                continuationToken = queryResults.ContinuationToken;
                results.AddRange(queryResults.Results);
            }
            while (continuationToken != null);

            return results.Select(tableEntity => tableEntity.ToEvent());
        }
    }
}
