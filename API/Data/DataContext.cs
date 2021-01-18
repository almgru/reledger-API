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

        public async Task AddAccountAndDescendantsAsync(Account account)
        {
            // TODO: Refactor this

            // If account has no descendants or ancestors, just add it if it doesn't exist and return
            if (account.Name.IndexOf(".") == -1)
            {
                await Accounts.AddIfNotExistsAsync(account, acc => acc.Name == account.Name);
                return;
            }

            // Store a copy of the original account string so we can use that as a starting point for descendants
            string accountString = account.Name;

            // Make sure increase balance behaviour is inherited from parent
            Account.IncreaseBalanceBehaviour increaseBehaviour;
            string mostAncestralAccountName = accountString.Substring(0, accountString.IndexOf("."));
            Account mostAncestralAccount = await Accounts.SingleOrDefaultAsync(
                acc => acc.Name == mostAncestralAccountName
            );

            if (mostAncestralAccount != null) // Use ancestor increase behaviour if an ancestor already exists...
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
             * We start with by getting the rightmost ancestor (for example "A.B" in account string "A.B.C" ["A.B.C"
             * is not an ancestor since it has no descendants]) and work our way up to more ancestral accounts
             * (First "A.B", then the ancestor of "A.B": "A").
             *
             * For each ancestor we determine its descendants by using the original account string as a starting
             * point and working our way upwards until the descendant has the same account name as the current
             * ancestor (So the descendant of "A.B" becomes "A.B.C" and the descendant of "A" is first "A.B.C" and
             * then "A.B").
             */
            while ((rightMostDotIndex = accountString.LastIndexOf(".")) != -1) // until no more ancestors
            {
                string ancestorName = accountString.Substring(0, rightMostDotIndex).Trim();
                string descendantString = original;
                Account ancestor = await Accounts.SingleOrDefaultAsync(acc => acc.Name == ancestorName);
                var descendants = new List<Account>();

                if (ancestor == null)
                {
                    ancestor = new Account()
                    {
                        Name = ancestorName,
                        IncreaseBalanceOn = increaseBehaviour
                    };
                }

                var parents = new List<Account>();
                while (descendantString != ancestor.Name)
                {
                    /*
                     * Commit changes so the next query will be able to find the current descendant if it exists in the
                     * database.
                     */
                    await SaveChangesAsync();

                    var descendant = await Accounts.SingleOrDefaultAsync(acc => acc.Name == descendantString);

                    if (descendant == null)
                    {
                        parents = new List<Account>();
                        descendant = new Account()
                        {
                            Name = descendantString,
                            IncreaseBalanceOn = increaseBehaviour
                        };
                        await Accounts.AddAsync(descendant);
                    }
                    else
                    {
                        parents = new List<Account>(descendant.ParentAccounts);
                    }

                    parents.Add(ancestor);
                    descendant.ParentAccounts = parents;
                    descendantString = descendantString.Substring(0, descendantString.LastIndexOf(".")).Trim();
                }

                await SaveChangesAsync();
                await Accounts.AddIfNotExistsAsync(ancestor, acc => acc.Name == ancestor.Name);

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
                        IncreaseBalanceOn = Account.IncreaseBalanceBehaviour.OnDebit
                    },
                    new Account()
                    {
                        Id = 2,
                        Name = "Liability",
                        Balance = 0,
                        IncreaseBalanceOn = Account.IncreaseBalanceBehaviour.OnCredit
                    },
                    new Account()
                    {
                        Id = 3,
                        Name = "Income",
                        Balance = 0,
                        IncreaseBalanceOn = Account.IncreaseBalanceBehaviour.OnCredit
                    },
                    new Account()
                    {
                        Id = 4,
                        Name = "Expense",
                        Balance = 0,
                        IncreaseBalanceOn = Account.IncreaseBalanceBehaviour.OnDebit
                    },
                    new Account()
                    {
                        Id = 5,
                        Name = "Capital",
                        Balance = 0,
                        IncreaseBalanceOn = Account.IncreaseBalanceBehaviour.OnCredit
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