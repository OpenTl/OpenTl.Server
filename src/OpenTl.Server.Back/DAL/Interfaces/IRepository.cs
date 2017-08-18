namespace OpenTl.Server.Back.DAL.Interfaces
{
    using System.Linq;

    using OpenTl.Server.Back.Entities;

    public interface IRepository<TEntity> where TEntity : IEntity
    {
        IQueryable<TEntity> GetAll();
        
        void Create(TEntity entity);
        
        void Update(TEntity entity);
        
        void Delete(TEntity entity);
    }
}