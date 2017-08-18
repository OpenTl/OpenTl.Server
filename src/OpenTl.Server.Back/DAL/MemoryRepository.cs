namespace OpenTl.Server.Back.DAL
{
    using System.Collections.Generic;
    using System.Linq;

    using OpenTl.Server.Back.DAL.Interfaces;
    using OpenTl.Server.Back.Entities;

    public class MemoryRepository<TEntity>: IRepository<TEntity>
        where TEntity : IEntity
    {
        private readonly List<TEntity> _entities = new List<TEntity>();
        
        public IQueryable<TEntity> GetAll()
        {
            return _entities.AsQueryable();
        }

        public void Create(TEntity entity)
        {
            _entities.Add(entity);
        }

        public void Update(TEntity entity)
        {
            var item = _entities.Single(e => e.Id.Equals(entity.Id));
                                
            _entities.RemoveAt(_entities.IndexOf(item));
                                
            _entities.Add(entity);
        }

        public void Delete(TEntity entity)
        {
            _entities.RemoveAt(_entities.IndexOf(entity));
        }
    }
}