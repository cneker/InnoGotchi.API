using System.Linq.Expressions;

namespace InnoGotchi.Application.Contracts.Repositories
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> GetAll(bool trackChanges);
        IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression,
            bool trackCahnges);
        Task Create(T entity);
        void Delete(T entity);
    }
}
