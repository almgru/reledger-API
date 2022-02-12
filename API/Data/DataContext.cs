using Microsoft.EntityFrameworkCore;
using API.Data.Entities;
using API.Constants;

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
                        IncreaseBalanceOn = IncreaseBalanceBehavior.OnDebit
                    },
                    new Account()
                    {
                        Id = 2,
                        Name = "Liability",
                        Balance = 0,
                        IncreaseBalanceOn = IncreaseBalanceBehavior.OnCredit
                    },
                    new Account()
                    {
                        Id = 3,
                        Name = "Income",
                        Balance = 0,
                        IncreaseBalanceOn = IncreaseBalanceBehavior.OnCredit
                    },
                    new Account()
                    {
                        Id = 4,
                        Name = "Expense",
                        Balance = 0,
                        IncreaseBalanceOn = IncreaseBalanceBehavior.OnDebit
                    },
                    new Account()
                    {
                        Id = 5,
                        Name = "Capital",
                        Balance = 0,
                        IncreaseBalanceOn = IncreaseBalanceBehavior.OnCredit
                    });
        }
    }
}
