﻿using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Core.Storage;
using Intellias.CQRS.EventStore.AzureTable.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.EventStore.AzureTable
{
    /// <inheritdoc />
    /// <summary>
    /// Azure Table Read Storage
    /// </summary>
    public class AzureTableReadStore<TReadModel> : IReadModelStore<TReadModel>
        where TReadModel : class, IReadModel
    {
        private readonly ReadModelRepository<TReadModel> repository;

        /// <summary>
        /// AzureTableReadStore
        /// </summary>
        /// <param name="cloudTable">Azure Cloud Table c</param>
        public AzureTableReadStore(CloudTable cloudTable)
        {
            repository = new ReadModelRepository<TReadModel>(cloudTable);
        }

        /// <summary>
        /// Get TReadModel by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TReadModel> GetAsync(string id) => await repository.GetModelAsync(id);

        /// <summary>
        /// Get collection of read models by type
        /// </summary>
        /// <returns></returns>
        public async Task<CollectionReadModel<TReadModel>> GetAllAsync() => await repository.GetAllModelsAsync();

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="newReadModel"></param>
        /// <returns></returns>
        public Task<TReadModel> UpdateAsync(TReadModel newReadModel) => throw new System.NotImplementedException();

        /// <summary>
        /// NOT IMPLENETED
        /// </summary>
        /// <param name="newCollection"></param>
        /// <returns></returns>
        public Task<CollectionReadModel<TReadModel>> UpdateAllAsync(CollectionReadModel<TReadModel> newCollection)
            => throw new System.NotImplementedException();

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task DeleteAsync(string id) => throw new System.NotImplementedException();

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <returns></returns>
        public Task DeleteAllAsync() => throw new System.NotImplementedException();
    }
}
