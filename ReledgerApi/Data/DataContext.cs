using Microsoft.EntityFrameworkCore;
using ReledgerApi.Data.Entities;

namespace ReledgerApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options) { }

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
                        Name = "Checking"
                    },
                    new Account()
                    {
                        Id = 2,
                        Name = "Expenses"
                    },
                    new Account()
                    {
                        Id = 3,
                        Name = "Savings"
                    });
        }
    }
}
