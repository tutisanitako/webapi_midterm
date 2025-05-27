using System.Data.Entity; // For DbContext
using Wordle_hw.Models;

namespace Wordle_hw.Data // You might want to create a new folder called 'Data'
{
    public class WordleDbContext : DbContext
    {
        public WordleDbContext() : base("name=WordleDbContext") // Name of your connection string
        {
            // Optional: Configure database initializer
            // Database.SetInitializer(new CreateDatabaseIfNotExists<WordleDbContext>());
            // Or for Code First Migrations:
            // Database.SetInitializer(new MigrateDatabaseToLatestVersion<WordleDbContext, Wordle_hw.Migrations.Configuration>());
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Guess> Guesses { get; set; }
        public DbSet<Statistic> Statistics { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configure one-to-one relationship for User and Statistic
            modelBuilder.Entity<User>()
                .HasOptional(u => u.Statistics) // User has optional Statistic
                .WithRequired(s => s.User);    // Statistic requires a User

            // You can add more fluent API configurations here if needed
            // For example, to ensure specific string lengths or indexes
            // modelBuilder.Entity<User>().Property(u => u.Email).IsRequired().HasMaxLength(256);
            // modelBuilder.Entity<Game>().HasMany(g => g.Guesses).WithRequired(gu => gu.Game);
            base.OnModelCreating(modelBuilder);
        }
    }
}