using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ReledgerApi.Data.Entities;

namespace ReledgerApi.Data.Extensions
{
    public static class AccountExtensions
    {
        public static void AddIfNotExists(
                this DbSet<Account> accounts,
                Account account,
                Expression<Func<Account, bool>> predicate)
        {
            if (accounts.AsQueryable().Any(predicate))
            {
                accounts.Add(account);
            }
        }
    }
}
