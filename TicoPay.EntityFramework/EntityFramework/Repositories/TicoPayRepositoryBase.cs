using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;

namespace TicoPay.EntityFramework.Repositories
{
    public abstract class TicoPayRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<TicoPayDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected TicoPayRepositoryBase(IDbContextProvider<TicoPayDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class TicoPayRepositoryBase<TEntity> : TicoPayRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected TicoPayRepositoryBase(IDbContextProvider<TicoPayDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }
}
