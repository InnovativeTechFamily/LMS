using LMS.API.Models.Domain;
using MongoDB.Driver;

namespace LMS.API.Services.Repositories.Implementations
{
    public class BaseRepository<T> where T : class
    {
        protected readonly IMongoCollection<T> Collection;
        protected readonly ILogger<BaseRepository<T>> Logger;

        public BaseRepository(IMongoCollection<T> collection, ILogger<BaseRepository<T>> logger)
        {
            Collection = collection;
            Logger = logger;
        }

        public virtual async Task<T?> GetByIdAsync(string id)
        {
            Logger.LogInformation("Getting {EntityType} by ID: {Id}", typeof(T).Name, id);
            return await Collection.Find(Builders<T>.Filter.Eq("_id", id)).FirstOrDefaultAsync();
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            Logger.LogInformation("Getting all {EntityType}", typeof(T).Name);
            return await Collection.Find(_ => true).ToListAsync();
        }

        public virtual async Task<List<T>> FindAsync(System.Linq.Expressions.Expression<System.Func<T, bool>> filter)
        {
            Logger.LogInformation("Finding {EntityType}", typeof(T).Name);
            return await Collection.Find(filter).ToListAsync();
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            Logger.LogInformation("Creating new {EntityType}", typeof(T).Name);
            await Collection.InsertOneAsync(entity);
            return entity;
        }

        public virtual async Task<T?> UpdateAsync(string id, T entity)
        {
            Logger.LogInformation("Updating {EntityType} with ID: {Id}", typeof(T).Name, id);
            return await Collection.FindOneAndReplaceAsync(
                Builders<T>.Filter.Eq("_id", id),
                entity,
                new FindOneAndReplaceOptions<T> { ReturnDocument = ReturnDocument.After }
            );
        }

        public virtual async Task<bool> DeleteAsync(string id)
        {
            Logger.LogInformation("Deleting {EntityType} with ID: {Id}", typeof(T).Name, id);
            var result = await Collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id));
            return result.DeletedCount > 0;
        }

        public virtual async Task<long> CountAsync()
        {
            Logger.LogInformation("Counting {EntityType}", typeof(T).Name);
            return await Collection.CountDocumentsAsync(_ => true);
        }
    }
}
