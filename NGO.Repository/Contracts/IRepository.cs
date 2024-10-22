using NGO.Common.Models;
using NGO.Data;
using System.Linq.Expressions;

namespace NGO.Repository.Contracts
{
    public interface IRepository<T> : IDisposable where T :  BaseEntity
    {
        /// <summary>
        /// To get all entities.
        /// </summary>
        /// <param name="includes">The 0 or more navigation properties to include for EF eager loading.</param>
        /// <returns>The list of entities</returns>
        List<T> GetAll(params Expression<Func<T,object>>[] includes);
        PagedResultModel<T> GetAll(FilterModel filterModel, params Expression<Func<T, object>>[] includes);

        /// <summary>
        ///To get all entities asynchronously.
        /// </summary>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);

        Task<PagedResultModel<T>> GetAllAsync(FilterModel filterModel,params Expression<Func<T, object>>[] includes);
        /// <summary>
        /// To get specific entries 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        List<T> GetBy(Expression<Func<T, bool>> predicate, params Expression<Func<T,object>>[] includes);
        PagedResultModel<T> GetBy(Expression<Func<T, bool>> predicate, FilterModel filterModel, params Expression<Func<T, object>>[] includes);
        /// <summary>
        /// To get specific entries asynchronously
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="filterModel"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<List<T>> GetByAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<PagedResultModel<T>> GetByAsync(Expression<Func<T, bool>> predicate, FilterModel filterModel, params Expression<Func<T, object>>[] includes);
        /// <summary>
        /// To determine the existance of an entity by the provided condition.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Any(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// To determine the existance of an entity by the provided condition asynchronously.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// To Add an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="suppressIdMapping"></param>
        /// <param name="suppressCreateMapping"></param>
        void Add(T entity, bool suppressIdMapping = false, bool suppressCreateMapping = false);
        /// <summary>
        /// To Update an entity
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);
        /// <summary>
        /// To Delete an entity
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);
        /// <summary>
        /// Sorting Records
        /// </summary>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        Expression<Func<T, object>> GetSortExpression(string sortBy);
        /// <summary>
        /// Save Asynchronously
        /// </summary>
        /// <returns></returns>
        Task<int> SaveAsync();


    }
}
