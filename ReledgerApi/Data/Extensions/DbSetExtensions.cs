using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ReledgerApi.Data.Extensions
{
    public static class DbSetExtensions
    {
        public static async Task<bool> NoneAsync<T>(
            this DbSet<T> dbSet,
            Expression<Func<T, bool>> predicate,
            CancellationToken token
        ) where T : class =>
            !(await dbSet.AnyAsync(predicate, token));
    }
}
