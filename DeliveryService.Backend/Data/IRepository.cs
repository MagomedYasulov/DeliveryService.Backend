using DeliveryService.Backend.Data.Entities;
using System.Linq.Expressions;
using System.Security.Principal;

namespace DeliveryService.Backend.Data
{
    public interface IRepository
    {
        Task<IEnumerable<TResult?>> GetAllAsync<TEntity, TResult>(
        int? skip = null,
        int? limit = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        params Expression<Func<TEntity, object>>[] includeProperties
        )
        where TEntity : class, IEntity;

        Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(
        int? skip = null,
        int? limit = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        params Expression<Func<TEntity, object>>[] includeProperties
        )
        where TEntity : class, IEntity;

        IEnumerable<TResult?> GetAll<TEntity, TResult>(
        int? skip = null,
        int? limit = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        params Expression<Func<TEntity, object>>[] includeProperties
        )
        where TEntity : class, IEntity;

        IEnumerable<TEntity> GetAll<TEntity>(
        int? skip = null,
        int? limit = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        params Expression<Func<TEntity, object>>[] includeProperties
        )
        where TEntity : class, IEntity;

        Task<IEnumerable<TEntity>> GetAsync<TEntity>(
            Expression<Func<TEntity, bool>>? filter = null,
            int? skip = null,
            int? limit = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties
            )
            where TEntity : class, IEntity;

        Task<IEnumerable<TResult?>> GetAsync<TEntity, TResult>(
            Expression<Func<TEntity, bool>>? filter = null,
            int? skip = null,
            int? limit = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
            where TEntity : class, IEntity;

        IEnumerable<TEntity> Get<TEntity>(
            Expression<Func<TEntity, bool>>? filter = null,
            int? skip = null,
            int? limit = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties
            )
            where TEntity : class, IEntity;

        IEnumerable<TResult?> Get<TEntity, TResult>(
            Expression<Func<TEntity, bool>>? filter = null,
            int? skip = null,
            int? limit = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
            where TEntity : class, IEntity;

        Task<TEntity?> GetOneAsync<TEntity>(
            Expression<Func<TEntity, bool>>? filter = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
            where TEntity : class, IEntity;

        Task<TResult?> GetOneAsync<TEntity, TResult>(
         Expression<Func<TEntity, bool>>? filter = null,
         params Expression<Func<TEntity, object>>[] includeProperties)
         where TEntity : class, IEntity;

        TEntity? GetOne<TEntity>(
            Expression<Func<TEntity, bool>>? filter = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
            where TEntity : class, IEntity;

        TResult? GetOne<TEntity, TResult>(
         Expression<Func<TEntity, bool>>? filter = null,
         params Expression<Func<TEntity, object>>[] includeProperties)
         where TEntity : class, IEntity;

        #region get by id

        Task<TEntity?> GetByIdAsync<TEntity>(int id, params Expression<Func<TEntity, object>>[] includeProperties)
            where TEntity : class, IEntity;

        Task<TResult?> GetByIdAsync<TEntity, TResult>(int id, params Expression<Func<TEntity, object>>[] includeProperties)
            where TEntity : class, IEntity;

        TEntity? GetById<TEntity>(int id, params Expression<Func<TEntity, object>>[] includeProperties)
            where TEntity : class, IEntity;

        TResult? GetById<TEntity, TResult>(int id, params Expression<Func<TEntity, object>>[] includeProperties)
            where TEntity : class, IEntity;

        #endregion

        Task<int> GetCountAsync<TEntity>(Expression<Func<TEntity, bool>>? filter = null) where TEntity : class, IEntity;

        Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>>? filter = null) where TEntity : class, IEntity;

        public void Create<TEntity>(TEntity entity) where TEntity : class, IEntity;

        void Update<TEntity>(TEntity entity) where TEntity : class, IEntity;

        void Delete<TEntity>(TEntity entity) where TEntity : class, IEntity;

        void Delete<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;

        Task SaveAsync();

        void Save();

        bool Any<TEntity>() where TEntity : class, IEntity;

        bool Any<TEntity>(Expression<Func<TEntity, bool>>? filter = null) where TEntity : class, IEntity;
    }
}
