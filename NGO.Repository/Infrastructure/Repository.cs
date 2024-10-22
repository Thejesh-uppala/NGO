using Microsoft.EntityFrameworkCore;
using NGO.Common;
using NGO.Common.Constraints;
using NGO.Common.Extentions;
using NGO.Common.Models;
using NGO.Data;
using NGO.Repository.Contracts;
using System.Linq.Expressions;
using System.Reflection;

namespace NGO.Repository.Infrastructure
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly NGOContext _nGODbContext;
        private readonly ApplicationContext _applicationContext;
        protected readonly DbSet<T> dbset;

        public Repository(NGOContext nGODbContext, ApplicationContext applicationContext)
        {
            _nGODbContext = nGODbContext;
            _applicationContext = applicationContext;
            if (_nGODbContext == null)
            {
                throw new Exception("Invalid tenant db context.");
            }
            this.dbset = _nGODbContext.Set<T>();
        }
        public virtual void Add(T entity, bool suppressIdMapping = false, bool suppressCreateMapping = false)
        {
            if (entity is IAuditable auditable)
            {
                auditable.CreatedBy = auditable.UpdatedBy = _applicationContext.UserId;
                auditable.UpdatedOn = DateTime.UtcNow;
            }

            if (!suppressCreateMapping && entity is IAuditable createdOn)
            {
                createdOn.CreatedOn = DateTime.UtcNow;
            }
            this.dbset.Add(entity);
        }

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return this.AnyAsync(predicate).Result;
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await this.dbset.AnyAsync(this.ApplyFilters(predicate)).ConfigureAwait(false);
        }

        protected Expression<Func<T, bool>> ApplyFilters(Expression<Func<T, bool>> expression = null)
        {
            // By default consider all.
            Expression<Func<T, bool>> filter = x => true;

            if (!_applicationContext.SuppressSoftDeleteFilters)
            {
                // Apply the soft deletion filter if applicable
                if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
                {
                    filter = x => !((ISoftDelete)x).IsDeleted;
                }
            }

            //Need To Check
            // Apply the tenant filter if applicable

            //if (typeof(ITenant).IsAssignableFrom(typeof(T)))
            //{
            //    filter = filter.And(x => ((ITenant)x).TenantId == _applicationContext.TenantId);
            //}

            return expression == null ? filter : expression.And(filter);
        }
        public List<T> GetBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            return this.GetByAsync(predicate, includes).Result;
        }

        /// <summary>
        /// To get the entities by provided condition.
        /// </summary>
        /// <param name="predicate">The conditions to filter with.</param>
        /// <returns>The list of entities</returns>
        public PagedResultModel<T> GetBy(Expression<Func<T, bool>> predicate, FilterModel filterModel, params Expression<Func<T, object>>[] includes)
        {
            return this.GetFilteredRecords(filterModel, predicate, includes).Result;
        }


        /// <summary>
        /// To get the entities by provided condition asynchronously.
        /// </summary>
        /// <param name="predicate">The conditions to filter with.</param>
        /// <param name="includes">The 0 or more navigation properties to include for EF eager loading.</param>
        /// <returns>The list of entities</returns>
        public async Task<List<T>> GetByAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            if (includes != null)
            {
                return await this.Include(this.dbset.Where(this.ApplyFilters(predicate)), includes).ToListAsync().ConfigureAwait(false);
            }

            return await this.dbset.Where(this.ApplyFilters(predicate)).ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="filterModel"></param>
        /// <param name="includes"></param>
        /// <returns></returns>GetSortExpression
        public async Task<PagedResultModel<T>> GetByAsync(Expression<Func<T, bool>> predicate, FilterModel filterModel, params Expression<Func<T, object>>[] includes)
        {
            return await this.GetFilteredRecords(filterModel, predicate, includes);
        }
        public virtual void Delete(T entity)
        {
            var entry = _nGODbContext.Entry(entity);
            entry.State = EntityState.Deleted;
        }

        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);

        }

        private void Dispose(bool dispose)
        {
            if (dispose)
            {
                //Do Nothing
            }
        }

        public virtual List<T> GetAll(params Expression<Func<T, object>>[] includes)
        {
            return this.GetAllAsync(includes).Result;
        }

        public virtual PagedResultModel<T> GetAll(FilterModel filterModel, params Expression<Func<T, object>>[] includes)
        {
            return this.GetAllAsync(filterModel,includes).Result;
        }

        public virtual async Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            if(includes != null)
            {
                return await this.Include(this.dbset.Where(this.ApplyFilters()),includes).ToListAsync().ConfigureAwait(false);
            }
            return await this.dbset.Where(this.ApplyFilters()).ToListAsync().ConfigureAwait(false);
        }

        private IQueryable<T> Include(IQueryable<T> query, Expression<Func<T, object>>[] includes)
        {
            foreach (var include in includes)
            {
                query = this.IncludeProperty(query, include);
            }
            return query;
        }

        private IQueryable<T> IncludeProperty(IQueryable<T> query, Expression<Func<T, object>> include)
        {
            return query.Include(include);
        }

        public virtual async Task<PagedResultModel<T>> GetAllAsync(FilterModel filterModel, params Expression<Func<T, object>>[] includes)
        {
            return await this.GetFilteredRecords(filterModel,x=>true,includes);
        }

        public Expression<Func<T, object>> GetSortExpression(string sortBy)
        {
            var entityType = typeof(T);
            if (sortBy.Contains('.'))
            {
                var param = Expression.Parameter(entityType, "instance");
                string[] childProperties = sortBy.Split('.');

                Expression parent = param;

                foreach (var childProperty in childProperties)
                {
                    parent = Expression.Property(parent, childProperty);
                }

                var sortExpression = Expression.Lambda<Func<T, object>>(parent, param);
                return sortExpression;
            }
            else if (entityType.GetProperties().Any(x => x.Name.ToLower() == sortBy.ToLower() && x.CanRead))
            {
                PropertyInfo propertyInfo = entityType.GetProperty(sortBy);
                ParameterExpression parameterExpression = Expression.Parameter(entityType, "instance");
                MemberExpression memberExpression = Expression.Property(parameterExpression, sortBy);
                Expression conversion = Expression.Convert(memberExpression, typeof(object));
                var result = Expression.Lambda<Func<T, object>>(conversion, parameterExpression);
                return result;
            }

            return null;
        }

        public async Task<int> SaveAsync()
        {
            return await _nGODbContext.SaveChangesAsync();
        }

        public void Update(T entity)
        {
            if (entity is IAuditable auditable)
            {
                auditable.UpdatedBy = _applicationContext.UserId;
                auditable.UpdatedOn = DateTime.UtcNow;
            }
            var entry = _nGODbContext.Entry(entity);
            this.dbset.Attach(entity);
            entry.State = EntityState.Modified;
        }
        private async Task<PagedResultModel<T>> GetFilteredRecords(FilterModel filterModel, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            PagedResultModel<T> pagedResult = new PagedResultModel<T>();

            var skip = (filterModel.PageNumber) * filterModel.PageSize;

            Expression<Func<T, bool>> searchFilterExpression = null;
            var generalFilterExpression = this.ApplyFilters(predicate);
            var sortExpression = GetSortExpression(filterModel.SortMember);

            if (filterModel.Filters?.Count > 0)
            {
                searchFilterExpression = FilterExpression(filterModel.Filters);
                pagedResult.TotalRecords = this.dbset.Where(generalFilterExpression).Where(searchFilterExpression).Count();
            }
            else
            {
                pagedResult.TotalRecords = (this.dbset.Where(generalFilterExpression)).Count();
            }

            pagedResult.TotalPages = ((pagedResult.TotalRecords + filterModel.PageSize - 1) / filterModel.PageSize);

            IQueryable<T> source;

            if (filterModel.Filters?.Count > 0)
            {
                source = this.dbset.Where(generalFilterExpression).Where(searchFilterExpression);
            }
            else
            {
                source = this.dbset.Where(generalFilterExpression);
            }

            if (includes != null)
            {
                source = this.Include(source, includes);
            }

            if (filterModel.SortDescending)
            {
                pagedResult.Records = await source
                   .OrderByDescending(sortExpression)
                   .Skip(skip).Take(filterModel.PageSize).ToListAsync().ConfigureAwait(false);
            }
            else
            {
                pagedResult.Records = await source
                    .OrderBy(sortExpression)
                    .Skip(skip).Take(filterModel.PageSize).ToListAsync().ConfigureAwait(false);
            }

            return pagedResult;
        }

        private Expression<Func<T, bool>>? FilterExpression(List<FilterOption> filters)
        {
            Expression<Func<T, bool>> filterExpression = x => true;

            foreach (var filter in filters)
            {
                var entityType = typeof(T);
                if (filter.Property.Contains('.'))
                {
                    var parameterExpression = Expression.Parameter(entityType, "instance");
                    string[] childProperties = filter.Property.Split('.');

                    Expression memberExpression = parameterExpression;

                    foreach (var childProperty in childProperties)
                    {
                        memberExpression = Expression.Property(memberExpression, childProperty);
                    }

                    filterExpression = filterExpression.And(this.GetFilterExpression(filter.Value, parameterExpression, memberExpression, filter.Operator));
                }
                else if (entityType.GetProperties().Any(x => x.Name.ToLower() == filter.Property.ToLower() && x.CanRead))
                {
                    ParameterExpression parameterExpression = Expression.Parameter(entityType, "instance");
                    MemberExpression memberExpression = Expression.Property(parameterExpression, filter.Property);

                    filterExpression = filterExpression.And(this.GetFilterExpression(filter.Value, parameterExpression, memberExpression, filter.Operator));
                }
            }

            return filterExpression;
        }

        private Expression<Func<T, bool>> GetFilterExpression(string filterValue, ParameterExpression parameterExpression, Expression memberExpression, string filterOperator)
        {
            ConstantExpression constantExpression = Expression.Constant(Convert.ChangeType(filterValue, memberExpression.Type));
            Expression expression;

            if (filterOperator != null)
            {
                if (memberExpression.Type == typeof(DateTime))
                {
                    expression = this.GetDateFilterExpression(memberExpression, filterOperator, constantExpression);
                }
                else
                {
                    switch (filterOperator)
                    {
                        case Constants.Operators.Equal:
                            expression = Expression.Equal(memberExpression, constantExpression);
                            break;
                        case Constants.Operators.LessThan:
                            expression = Expression.LessThan(memberExpression, constantExpression);
                            break;
                        case Constants.Operators.GreaterThan:
                            expression = Expression.GreaterThan(memberExpression, constantExpression);
                            break;
                        case Constants.Operators.LessThanOrEquals:
                            expression = Expression.LessThanOrEqual(memberExpression, constantExpression);
                            break;
                        case Constants.Operators.GreaterThanOrEquals:
                            expression = Expression.GreaterThanOrEqual(memberExpression, constantExpression);
                            break;
                        default:
                            throw new ApplicationException("Invalid operator!");
                    }
                }
            }
            else
            {
                MethodInfo containsMethodInfo = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                expression = Expression.Call(memberExpression, containsMethodInfo, constantExpression);
            }

            return Expression.Lambda<Func<T, bool>>(expression, parameterExpression);
        }

        private Expression GetDateFilterExpression(Expression memberExpression, string filterOperator, ConstantExpression constantExpression)
        {
            Expression expression;

            switch (filterOperator)
            {
                case Constants.Operators.Equal:
                    expression = Expression.Equal(Expression.MakeMemberAccess(memberExpression, typeof(DateTime).GetMember("Date").Single()), constantExpression);
                    break;
                case Constants.Operators.LessThan:
                    expression = Expression.LessThan(Expression.MakeMemberAccess(memberExpression, typeof(DateTime).GetMember("Date").Single()), constantExpression);
                    break;
                case Constants.Operators.GreaterThan:
                    expression = Expression.GreaterThan(Expression.MakeMemberAccess(memberExpression, typeof(DateTime).GetMember("Date").Single()), constantExpression);
                    break;
                case Constants.Operators.LessThanOrEquals:
                    expression = Expression.LessThanOrEqual(Expression.MakeMemberAccess(memberExpression, typeof(DateTime).GetMember("Date").Single()), constantExpression);
                    break;
                case Constants.Operators.GreaterThanOrEquals:
                    expression = Expression.GreaterThanOrEqual(Expression.MakeMemberAccess(memberExpression, typeof(DateTime).GetMember("Date").Single()), constantExpression);
                    break;
                default:
                    throw new ApplicationException("Invalid operator!");
            }

            return expression;
        }

    }
}
