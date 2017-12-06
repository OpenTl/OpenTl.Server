namespace OpenTl.Server.Back.DAL.Interfaces
{
    using System;
    using System.Linq;

    using OpenTl.Server.Back.Contracts.Entities;

    public interface IRepository<TEntity> where TEntity : IEntity
    {

        IQueryable<TEntity> GetAll();
        
        TEntity Get(Guid id);
        
        void Create(TEntity entity);
        
        void Update(TEntity entity);
        
        void Delete(TEntity entity);
    }
}