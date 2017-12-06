namespace OpenTl.Server.UnitTests.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Moq;

    using OpenTl.Server.Back.Contracts.Entities;
    using OpenTl.Server.Back.DAL.Interfaces;

    public static class RepositoryBuilder
    {
        public static void BuildRepository<TEntity>(this BaseTest baseTest, params TEntity[] entities) where TEntity : IEntity
        {
            var items = new List<TEntity>(entities);
            
            var mRepository = new Mock<IRepository<TEntity>>();
            mRepository.Setup(repository => repository.Get(It.IsAny<Guid>())).Returns<Guid>(id => items.FirstOrDefault(i => i.Id == id));
            mRepository.Setup(repository => repository.GetAll()).Returns(() => items.AsQueryable());
            mRepository.Setup(repository => repository.Create(It.IsAny<TEntity>())).Callback<TEntity>(entity => items.Add(entity));
            mRepository.Setup(repository => repository.Delete(It.IsAny<TEntity>())).Callback<TEntity>(entity => items.RemoveAt(items.IndexOf(entity)));
            
            mRepository.Setup(repository => repository.Update(It.IsAny<TEntity>())).Callback<TEntity>(
                            entity =>
                            {
                                var item = items.Single(e => e.Id.Equals(entity.Id));
                                
                                items.RemoveAt(items.IndexOf(item));
                                
                                items.Add(entity);
                            });

            baseTest.RegisterMockAndInstance(mRepository);
        }
    }
}