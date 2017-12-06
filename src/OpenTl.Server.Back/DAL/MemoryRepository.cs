namespace OpenTl.Server.Back.DAL
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;

    using OpenTl.Server.Back.Contracts.Entities;
    using OpenTl.Server.Back.DAL.Interfaces;

    public class MemoryRepository<TEntity>: IRepository<TEntity>
        where TEntity : IEntity
    {
        private readonly ConcurrentDictionary<Guid, TEntity> _entities;

        public MemoryRepository()
        {
            _entities = new ConcurrentDictionary<Guid, TEntity>();
        }
        public IQueryable<TEntity> GetAll()
        {
            return _entities.Values.AsQueryable();
        }

        public TEntity Get(Guid id)
        {
            return _entities[id];
        }

        public void Create(TEntity entity)
        {
            _entities.AddOrUpdate(entity.Id, entity, (guid, e) => entity);
        }

        public void Update(TEntity entity)
        {
            _entities.AddOrUpdate(entity.Id, entity, (guid, e) => entity);
        }

        public void Delete(TEntity entity)
        {
            _entities.TryRemove(entity.Id, out var _);
        }
    }
}