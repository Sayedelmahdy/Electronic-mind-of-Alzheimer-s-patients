
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        // Async

        Task<T?> GetByIdAsync(string id);

        Task<IEnumerable<T>?> GetAllAsync();
        Task AddAsync(T entity);

        Task DeleteAsync(T entity);
        Task UpdateAsync(T entity);
        public Task<IEnumerable<T>?> WhereAsync(Expression<Func<T, bool>> exp);
        public Task<T?> FindAsync(Expression<Func<T, bool>> expression, string[] includes = null);
        public Task<int?> CountAsync();
        public Task<int?> CountAsync(Expression<Func<T, bool>> expression);
        Task<IQueryable<T>?> SelectAsync(Expression<Func<T, bool>> expression);

        Task<bool?> AnyAsync(Expression<Func<T, bool>> expression);
        Task<IDbContextTransaction> BeginTransactionAsync();


        //==================================================================================

        //Without Async


        T? GetById(string id);

        IEnumerable<T>? GetAll();
        void Add(T entity);
        IEnumerable<IGrouping<TKey, T>> GroupBy<TKey>(Expression<Func<T, TKey>> keySelector);
        void Delete(T entity);

        public IEnumerable<T>? Where(Expression<Func<T, bool>> exp);

        public T? Find(Expression<Func<T, bool>> expression, string[] includes = null);

        public IEnumerable<T>? FindAll(Expression<Func<T, bool>> expression, string[] includes = null);

        public int? Count();

        public int? Count(Expression<Func<T, bool>> expression);

        IQueryable<T>? Select(Expression<Func<T, bool>> expression);

        IQueryable<T>? Include(params Expression<Func<T, object>>[] includeProperties);

        bool Any(Expression<Func<T, bool>> expression);

        public void Attach(T Entity);

        public void Entry(T Entity);

        public IEnumerable<T>? SelectMany(Expression<Func<T, IEnumerable<T>>> selector);
        
    }
}
