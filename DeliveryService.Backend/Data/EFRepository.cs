using DeliveryService.Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using AutoMapper;

namespace DeliveryService.Backend.Data
{
    public class EFRepository<TContext> : IRepository where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly IMapper _mapper;

        public EFRepository(TContext context, IMapper mepper)
        {
            _mapper = mepper;
            _context = context;
        }

        protected virtual IQueryable<TEntity> GetQueryable<TEntity>(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            int? skip = null,
            int? limit = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
        where TEntity : class, IEntity
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (limit.HasValue)
            {
                query = query.Take(limit.Value);
            }
            return query;
        }

        #region get all

        public async Task<IEnumerable<TResult?>> GetAllAsync<TEntity, TResult>(
            int? skip,
            int? limit,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy,
            params Expression<Func<TEntity, object>>[] includeProperties)
            where TEntity : class, IEntity
        {
            return await GetQueryable(null, orderBy, skip, limit, includeProperties).Select(e => _mapper.Map<TResult>(e)).ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(
        int? skip,
        int? limit,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy,
        params Expression<Func<TEntity, object>>[] includeProperties)
        where TEntity : class, IEntity
        {
            return await GetQueryable(null, orderBy, skip, limit, includeProperties).ToListAsync();
        }

        public IEnumerable<TResult?> GetAll<TEntity, TResult>(int? skip, int? limit, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy, params Expression<Func<TEntity, object>>[] includeProperties)
            where TEntity : class, IEntity
        {
            return GetQueryable(null, orderBy, skip, limit, includeProperties).Select(e => _mapper.Map<TResult>(e)).ToList();
        }

        public IEnumerable<TEntity> GetAll<TEntity>(int? skip, int? limit, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy, params Expression<Func<TEntity, object>>[] includeProperties)
            where TEntity : class, IEntity
        {
            return GetQueryable(null, orderBy, skip, limit, includeProperties).ToList();
        }

        #endregion

        #region get with filter skip and limit
        public async Task<IEnumerable<TEntity>> GetAsync<TEntity>(
            Expression<Func<TEntity, bool>>? filter,
            int? skip,
            int? limit,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy,
            params Expression<Func<TEntity, object>>[] includeProperties)
            where TEntity : class, IEntity
        {
            return await GetQueryable<TEntity>(filter, orderBy, skip, limit, includeProperties).ToListAsync();
        }

        public async Task<IEnumerable<TResult?>> GetAsync<TEntity, TResult>(
             Expression<Func<TEntity, bool>>? filter,
             int? skip, int? limit,
             Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy,
             params Expression<Func<TEntity, object>>[] includeProperties)
             where TEntity : class, IEntity
        {
            return await GetQueryable<TEntity>(filter, orderBy, skip, limit, includeProperties).Select(e => _mapper.Map<TResult>(e)).ToListAsync();
        }

        public IEnumerable<TEntity> Get<TEntity>(
           Expression<Func<TEntity, bool>>? filter,
           int? skip,
           int? limit,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy,
           params Expression<Func<TEntity, object>>[] includeProperties)
           where TEntity : class, IEntity
        {
            return GetQueryable<TEntity>(filter, orderBy, skip, limit, includeProperties).ToList();
        }

        public IEnumerable<TResult?> Get<TEntity, TResult>(
             Expression<Func<TEntity, bool>>? filter,
             int? skip, int? limit,
             Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy,
             params Expression<Func<TEntity, object>>[] includeProperties)
             where TEntity : class, IEntity
        {
            return GetQueryable<TEntity>(filter, orderBy, skip, limit, includeProperties).Select(e => _mapper.Map<TResult>(e)).ToList();
        }
        #endregion

        #region get by id 

        public Task<TEntity?> GetByIdAsync<TEntity>(int id, params Expression<Func<TEntity, object>>[] includeProperties)
           where TEntity : class, IEntity
        {
            return GetQueryable<TEntity>(e => e.Id == id, null, null, null, includeProperties).SingleOrDefaultAsync();
        }

        public Task<TResult?> GetByIdAsync<TEntity, TResult>(int id, params Expression<Func<TEntity, object>>[] includeProperties)
            where TEntity : class, IEntity
        {
            return GetQueryable<TEntity>(e => e.Id == id, null, null, null, includeProperties).Select(e => _mapper.Map<TResult>(e)).SingleOrDefaultAsync();
        }

        public TEntity? GetById<TEntity>(int id, params Expression<Func<TEntity, object>>[] includeProperties)
            where TEntity : class, IEntity
        {
            return GetQueryable<TEntity>(e => e.Id == id, null, null, null, includeProperties).SingleOrDefault();
        }

        public TResult? GetById<TEntity, TResult>(int id, params Expression<Func<TEntity, object>>[] includeProperties)
            where TEntity : class, IEntity
        {
            return GetQueryable<TEntity>(e => e.Id == id, null, null, null, includeProperties).Select(e => _mapper.Map<TResult>(e)).SingleOrDefault();
        }

        #endregion

        #region get count
        Task<int> IRepository.GetCountAsync<TEntity>(Expression<Func<TEntity, bool>>? filter)
        {
            return GetQueryable<TEntity>(filter).CountAsync();
        }
        #endregion

        #region get one
        public virtual Task<TEntity?> GetOneAsync<TEntity>(Expression<Func<TEntity, bool>>? filter, params Expression<Func<TEntity, object>>[] includeProperties)
            where TEntity : class, IEntity
        {
            return GetQueryable<TEntity>(filter, null, null, null, includeProperties).SingleOrDefaultAsync();
        }

        public virtual Task<TResult?> GetOneAsync<TEntity, TResult>(Expression<Func<TEntity, bool>>? filter, params Expression<Func<TEntity, object>>[] includeProperties)
           where TEntity : class, IEntity
        {
            return GetQueryable<TEntity>(filter, null, null, null, includeProperties).Select(e => _mapper.Map<TResult>(e)).SingleOrDefaultAsync();
        }

        public virtual TEntity? GetOne<TEntity>(Expression<Func<TEntity, bool>>? filter, params Expression<Func<TEntity, object>>[] includeProperties)
            where TEntity : class, IEntity
        {
            return GetQueryable<TEntity>(filter, null, null, null, includeProperties).SingleOrDefault();
        }

        public virtual TResult? GetOne<TEntity, TResult>(Expression<Func<TEntity, bool>>? filter, params Expression<Func<TEntity, object>>[] includeProperties)
           where TEntity : class, IEntity
        {
            return GetQueryable<TEntity>(filter, null, null, null, includeProperties).Select(e => _mapper.Map<TResult>(e)).SingleOrDefault();
        }
        #endregion

        #region create
        public void Create<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            _context.Set<TEntity>().Add(entity);
        }
        #endregion

        #region update
        void IRepository.Update<TEntity>(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
        #endregion

        #region delete
        public void Delete<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            //_context.Set<TEntity>().Where(filter).ExecuteDelete(); // - Не загружает в память модель, но не работает в InMemory бд из за чего не проходятся тесты
            var dbSet = _context.Set<TEntity>();
            dbSet.RemoveRange(_context.Set<TEntity>().Where(filter));
        }

        void IRepository.Delete<TEntity>(TEntity entity)
        {
            var dbSet = _context.Set<TEntity>();
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
        }
        #endregion

        #region any 
        bool IRepository.Any<TEntity>()
        {
            return _context.Set<TEntity>().Any();
        }

        Task<bool> IRepository.AnyAsync<TEntity>(Expression<Func<TEntity, bool>>? filter)
        {
            return GetQueryable<TEntity>(filter).AnyAsync();
        }

        bool IRepository.Any<TEntity>(Expression<Func<TEntity, bool>>? filter)
        {
            return GetQueryable<TEntity>(filter).Any();
        }

        #endregion

        #region save 

        public void Save()
        {
            _context.SaveChanges();
        }

        public Task SaveAsync()
        {
            return _context.SaveChangesAsync();
        }

        #endregion
    }
}
