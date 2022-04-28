using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Rental.Common.Service.MongoDB
{
    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> itemCollection;

        private readonly FilterDefinitionBuilder<T>
            filterBuilder = Builders<T>.Filter;

        public MongoRepository(
            IMongoDatabase mongoDatabase,
            string collectionName
        )
        {
            itemCollection = mongoDatabase.GetCollection<T>(collectionName);
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await itemCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            return await itemCollection.Find(filter).ToListAsync();
        }

        public async Task<T> GetAsync(Guid id)
        {
            var filter = filterBuilder.Eq(i => i.Id, id);
            return await itemCollection.Find(filter).FirstOrDefaultAsync();
        }

        
        public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
            return await itemCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await itemCollection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var filter = filterBuilder.Eq(i => i.Id, entity.Id);
            await itemCollection.ReplaceOneAsync(filter, entity);
        }

        public async Task RemoveAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var filter = filterBuilder.Eq(i => i.Id, entity.Id);
            await itemCollection.DeleteOneAsync(filter);
        }

    }
}
