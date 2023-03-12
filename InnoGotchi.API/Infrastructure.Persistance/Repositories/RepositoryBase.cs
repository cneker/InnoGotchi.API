using InnoGotchi.Application.Contracts.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistance.Repositories
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private AppDbContext _repositoryContext;

        public RepositoryBase(AppDbContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public IQueryable<T> GetAll(bool trackChanges) =>
            trackChanges ?
                _repositoryContext.Set<T>() :
                _repositoryContext.Set<T>().AsNoTracking();

        public IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression,
            bool trackCahnges) =>
            trackCahnges ?
                _repositoryContext.Set<T>().Where(expression) :
                _repositoryContext.Set<T>().Where(expression).AsNoTracking();

        public async Task CreateAsync(T entity) =>
            await _repositoryContext.Set<T>().AddAsync(entity);

        public void Delete(T entity) =>
            _repositoryContext.Set<T>().Remove(entity);
    }
}
