using API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Attachment> Attachments { get; set; }

        public async Task AddAccountAndChildrenAsync(Account account)
        {
            // If account has no children or parents, just add it if it doesn't exist and return
            if (account.Name.IndexOf(".") == -1)
            {
                await Accounts.AddIfNotExistsAsync(account, acc => acc.Name == account.Name);
                return;
            }

            // Store a copy of the original account string so we can use that as a starting point for children
            string accountString = account.Name;

            // Make sure increase balance behaviour is inherited from parent
            Account.IncreaseBalanceBehavior increaseBehaviour;
            string mostAncestralAccountName = accountString.Substring(0, accountString.IndexOf("."));
            Account mostAncestralAccount = await Accounts.SingleOrDefaultAsync(
                acc => acc.Name == mostAncestralAccountName
            );

            if (mostAncestralAccount != null) // Use parent increase behaviour if a parent already exists...
            {
                increaseBehaviour = mostAncestralAccount.IncreaseBalanceOn;
            }
            else // ...otherwise, use the client specified behaviour
            {
                increaseBehaviour = account.IncreaseBalanceOn;
            }

            string original = accountString;
            int rightMostDotIndex;
            /*
             * We start with getting the rightmost parent (for example "A.B" in account string "A.B.C" ["A.B.C" is not
             * a parent since it has no children]) and work our way up to more ancestral accounts (First "A.B", then
             * the parent of "A.B": "A").
             */
            while ((rightMostDotIndex = accountString.LastIndexOf(".")) != -1) // until no more parents
            {
                string parentName = accountString.Substring(0, rightMostDotIndex).Trim();
                string childString = original;
                Account parent = await Accounts.SingleOrDefaultAsync(acc => acc.Name == parentName);

                if (parent == null)
                {
                    parent = new Account()
                    {
                        Name = parentName,
                        IncreaseBalanceOn = increaseBehaviour
                    };
                }

                /*
                 * For each parent we determine its children by using the original account string as a starting point
                 * and working our way upwards until the child has the same account name as the current parent (So with 
                 * the previous example of "A.B.C", the child of "A.B" becomes "A.B.C" and the child of "A" is first
                 * "A.B.C" and then "A.B").
                 */
                while (childString != parent.Name)
                {
                    /*
                     * Commit changes so the next query will be able to find the current child if it exists in the
                     * database.
                     */
                    await SaveChangesAsync();

                    var child = await Accounts.SingleOrDefaultAsync(acc => acc.Name == childString);

                    if (child == null)
                    {
                        child = new Account()
                        {
                            Name = childString,
                            IncreaseBalanceOn = increaseBehaviour
                        };
                        await Accounts.AddAsync(child);
                    }

                    if (child.ParentId == null)
                    {
                        child.ParentId = parent.Id;
                    }

                    childString = childString.Substring(0, childString.LastIndexOf(".")).Trim();
                }

                await SaveChangesAsync();
                await Accounts.AddIfNotExistsAsync(parent, acc => acc.Name == parent.Name);

                accountString = accountString.Substring(0, rightMostDotIndex).Trim();
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Account>()
                .HasIndex(acc => acc.Name)
                .IsUnique();

            builder.Entity<Tag>()
                .HasIndex(tag => tag.Name)
                .IsUnique();

            builder.Entity<Account>()
                .HasData(
                    new Account()
                    {
                        Id = 1,
                        Name = "Asset",
                        Balance = 0,
                        IncreaseBalanceOn = Account.IncreaseBalanceBehavior.OnDebit
                    },
                    new Account()
                    {
                        Id = 2,
                        Name = "Liability",
                        Balance = 0,
                        IncreaseBalanceOn = Account.IncreaseBalanceBehavior.OnCredit
                    },
                    new Account()
                    {
                        Id = 3,
                        Name = "Income",
                        Balance = 0,
                        IncreaseBalanceOn = Account.IncreaseBalanceBehavior.OnCredit
                    },
                    new Account()
                    {
                        Id = 4,
                        Name = "Expense",
                        Balance = 0,
                        IncreaseBalanceOn = Account.IncreaseBalanceBehavior.OnDebit
                    },
                    new Account()
                    {
                        Id = 5,
                        Name = "Capital",
                        Balance = 0,
                        IncreaseBalanceOn = Account.IncreaseBalanceBehavior.OnCredit
                    });
        }
    }

    public static class DbSetExtensions
    {
        public static async Task<T> AddIfNotExistsAsync<T>(this DbSet<T> dbSet, T entity,
                                                           Expression<Func<T, bool>> predicate)
        where T : class, new()
        {
            if (await dbSet.AnyAsync(predicate))
            {
                return null;
            }
            else
            {
                await dbSet.AddAsync(entity);
                return entity;
            }
        }
    }
}
