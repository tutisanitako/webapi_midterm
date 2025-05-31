using System.Data.Entity;
using Wordle_hw.Models.Entities;

namespace Wordle_hw.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("DefaultConnection")
        {
            // This initializer creates the database if it doesn't exist,
            // based on your DbContext and entities. For migrations, you often
            // let migrations handle initial creation too, but this is fine.
            Database.SetInitializer(new CreateDatabaseIfNotExists<AppDbContext>());
        }

        // Define your DbSet properties for each entity that will be mapped to a database table
        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Guess> Guesses { get; set; }
        public DbSet<Statistic> Statistics { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // --- User configurations ---
            // Ensure Username is unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Ensure Email is unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // --- Game configurations ---
            // A Game requires a User.
            // A User can have many Games.
            // The foreign key is Game.UserId.
            // Cascade delete is set to false to prevent accidental deletion of a User
            // when their Games are deleted.
            modelBuilder.Entity<Game>()
                .HasRequired(g => g.User)
                .WithMany(u => u.Games)
                .HasForeignKey(g => g.UserId)
                .WillCascadeOnDelete(false);

            // --- Guess configurations ---
            // A Guess requires a Game.
            // A Game can have many Guesses.
            // The foreign key is Guess.GameId.
            // Cascade delete is set to true, meaning if a Game is deleted, its associated
            // Guesses will also be deleted.
            modelBuilder.Entity<Guess>()
                .HasRequired(g => g.Game)
                .WithMany(g => g.Guesses)
                .HasForeignKey(g => g.GameId)
                .WillCascadeOnDelete(true);

            // --- Statistics configurations ---
            // Explicitly define Statistic.Id as its primary key (good practice, though often by convention)
            modelBuilder.Entity<Statistic>()
                .HasKey(s => s.Id);

            // Configure the relationship from the Statistic (dependent) side to the User (principal) side.
            // A Statistic requires a User.
            // The foreign key property in Statistic is 'UserId'.
            modelBuilder.Entity<Statistic>()
                .HasRequired(s => s.User)       // Each Statistic must belong to a User
                .WithMany()                     // User does NOT have a collection of Statistics (since it's 0..1),
                                                // but WithMany() is used here to avoid shared-PK assumptions
                                                // and allow explicit FK mapping.
                .HasForeignKey(s => s.UserId)   // Explicitly define UserId as the foreign key property on Statistic
                .WillCascadeOnDelete(false);    // Prevents deleting a User from automatically deleting their Statistics

            // Configure the inverse navigation property from User to Statistic.
            // A User has an optional Statistic (0 or 1).
            modelBuilder.Entity<User>()
                .HasOptional(u => u.Statistics); // User has an optional Statistic (no HasForeignKey needed here as it's defined on the other side)

            // Call the base method to complete model creation
            base.OnModelCreating(modelBuilder);
        }
    }
}