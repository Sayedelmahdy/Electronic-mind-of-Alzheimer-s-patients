using DAL.Context;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly DBContext _dBContext;

        public BaseRepository(DBContext dBContext)
        {
            _dBContext = dBContext;

        }
        public void Add(T entity)
        {
            _dBContext.Add(entity);
            _dBContext.SaveChanges();

        }

        public async Task AddAsync(T entity)
        {
            _dBContext.Add(entity);

            await _dBContext.SaveChangesAsync();


        }

        public bool Any(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return _dBContext.Set<T>().Any(expression);


        }

        public async Task<bool?> AnyAsync(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return await _dBContext.Set<T>().AnyAsync(expression);
        }

        public void Attach(T Entity)
        {
            _dBContext.Set<T>().Attach(Entity);

        }
        public int? Count()
        {
            return _dBContext.Set<T>().Count();
        }

        public int? Count(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return _dBContext.Set<T>().Count(expression);

        }

        public async Task<int?> CountAsync()
        {
            return await _dBContext.Set<T>().CountAsync();

        }

        public async Task<int?> CountAsync(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return await _dBContext.Set<T>().CountAsync(expression);
        }

        public void Delete(T entity)
        {
            _dBContext.Set<T>().Remove(entity);
            _dBContext.SaveChanges();
        }

        public async Task DeleteAsync(T entity)
        {
            _dBContext.Set<T>().Remove(entity);
            await _dBContext.SaveChangesAsync();
        }

        public void Entry(T Entity)
        {
            _dBContext.Set<T>().Entry(Entity).State = EntityState.Unchanged;
        }

        public T? Find(System.Linq.Expressions.Expression<Func<T, bool>> expression, string[] includes = null)
        {
            IQueryable<T> query = _dBContext.Set<T>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return query.SingleOrDefault(expression);
        }

        public IEnumerable<T>? FindAll(System.Linq.Expressions.Expression<Func<T, bool>> expression, string[] includes = null)
        {
            IQueryable<T> query = _dBContext.Set<T>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return query.Where(expression).ToList();
        }

        
        public async Task<T?> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> expression, string[] includes = null)
        {


            IQueryable<T> query = _dBContext.Set<T>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.SingleOrDefaultAsync(expression);

        }

        public IEnumerable<T>? GetAll()
        {
            return _dBContext.Set<T>().ToList();
        }

        public async Task<IEnumerable<T>?> GetAllAsync()
        {
            return await _dBContext.Set<T>().ToListAsync();
        }

        public T? GetById(string id)
        {
            var x = _dBContext.Set<T>().Find(id);
            if (x == null)
                return null;
            return x;
        }

        public async Task<T?> GetByIdAsync(string id)
        {

            var x = await _dBContext.Set<T>().FindAsync(id);
            if (x == null)
                return null;
            return x;

        }


        public IQueryable<T>? Include(params System.Linq.Expressions.Expression<Func<T, object>>[] includeProperties)
        {

            IQueryable<T> query = _dBContext.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }


        public IQueryable<T>? Select(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return _dBContext.Set<T>().Where(expression);
        }

        public async Task<IQueryable<T>?> SelectAsync(Expression<Func<T, bool>> expression)
        {
            return (IQueryable<T>)await _dBContext.Set<T>().Where(expression).ToListAsync();

        }

        public IEnumerable<T>? SelectMany(System.Linq.Expressions.Expression<Func<T, IEnumerable<T>>> selector)
        {
            return _dBContext.Set<T>().SelectMany(selector);
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _dBContext.Set<T>().Update(entity);
            await _dBContext.SaveChangesAsync();
        }



        public IEnumerable<T>? Where(System.Linq.Expressions.Expression<Func<T, bool>> exp)
        {
            return _dBContext.Set<T>().Where(exp);
        }

        public async Task<IEnumerable<T>?> WhereAsync(System.Linq.Expressions.Expression<Func<T, bool>> exp)
        {
            return await _dBContext.Set<T>().Where(exp).ToListAsync();
        }
        public IEnumerable<IGrouping<TKey, T>> GroupBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return _dBContext.Set<T>().GroupBy(keySelector);
        }
        public async Task<IDbContextTransaction>  BeginTransactionAsync()
        {
           return await  _dBContext.Database.BeginTransactionAsync();
        }
    }
}
