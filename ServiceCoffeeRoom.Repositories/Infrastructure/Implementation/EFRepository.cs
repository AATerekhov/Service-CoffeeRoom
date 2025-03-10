using Microsoft.EntityFrameworkCore;
using ServiceCoffeeRoom.Core.Abstraction;
using ServiceCoffeeRoom.Infrastructure.Entityframework;
using ServiceСoffeeRoom.Domain.Base;
using System.Linq.Expressions;

namespace ServiceCoffeeRoom.Repositories.Infrastructure.Implementation
{
    public class EFRepository<TEntity, TId>(ApplicationDbContext context) : IRepository<TEntity, TId> 
        where TEntity : Entity<TId> where TId : struct
    {
        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken token = default)
        {
            context.Add(entity);
            await context.SaveChangesAsync(token);
            return entity;
        }

        public async Task<bool> DeleteAsync(TEntity entity, CancellationToken token = default)
        {
            context.Remove(entity);
            await  context.SaveChangesAsync(token);
            return true;
        }

        public async Task<bool> DeleteAsync(TId id, CancellationToken token = default)
        {
            var entity = await GetByIdAsync(id);
            if (entity is null)
                return false;
            await DeleteAsync(entity, token);
            return true;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken token = default) 
            => (await context.Set<TEntity>().ToListAsync(token)).AsEnumerable();

        public Task<IEnumerable<TEntity>> GetByFilter(Expression<Func<TEntity, bool>> expression, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken token = default) 
            => await context.Set<TEntity>().FindAsync(id, token);


        public async Task<bool> UpdateAsync(TEntity entity, CancellationToken token = default)
        {
            context.Update(entity);
            await context.SaveChangesAsync(token);
            return true;
        }

    }
}
