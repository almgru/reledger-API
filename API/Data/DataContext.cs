using API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
            // Store a copy of the original account string so we can use that as a starting point for descendants
            string accountString = account.Name;
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
                var descendants = new List<Account>();
                var ancestor = new Account() { Name = accountString.Substring(0, rightMostDotIndex).Trim() };
                var descendantString = original;

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
                        descendant = new Account() { Name = descendantString };
                        await Accounts.AddAsync(descendant);
                    }

                    descendants.Add(descendant);
                    descendantString = descendantString.Substring(0, descendantString.LastIndexOf(".")).Trim();
                }

                ancestor.Descendants = descendants;
                await AddAsync(ancestor);
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
                    new Account() { Id = 1, Name = "Asset", Balance = 0 },
                    new Account() { Id = 2, Name = "Liability", Balance = 0 },
                    new Account() { Id = 3, Name = "Income", Balance = 0 },
                    new Account() { Id = 4, Name = "Expense", Balance = 0 },
                    new Account() { Id = 5, Name = "Capital", Balance = 0 });
        }
    }
}