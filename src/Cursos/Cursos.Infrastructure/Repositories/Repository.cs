using Cursos.Domain.Abstractions;
using MongoDB.Driver;

namespace Cursos.Infrastructure.Repositories;

public class Repository<T> where T : Entity
{
    protected readonly IMongoCollection<T> _collection;

    public Repository(IMongoCollection<T> collection)
    {
        _collection = collection;
    }

    public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(Builders<T>.Filter.Eq("Id", id)).FirstOrDefaultAsync(cancellationToken);
    }
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }
    public async Task<bool> UpdateAsync(Guid id, T entity, CancellationToken cancellationToken = default)
    {
        var result = await _collection.ReplaceOneAsync(
            Builders<T>.Filter.Eq("Id", id),
            entity,
            new ReplaceOptions { IsUpsert = true },
            cancellationToken: cancellationToken
        );
        
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }
}