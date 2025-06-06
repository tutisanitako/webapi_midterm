using System.Data.Entity; // For DbContext, DbSet, and DbModelBuilder
using Domain.Entities;
using Infrastructure.Data.Configurations; // To apply entity configurations
using System.Configuration; // To read connection string from Web.config

namespace Infrastructure.Data
{
    /// <summary>
    /// Represents the database context for the Health Billing System,
    /// using Entity Framework 6 (EF6) Code-First approach.
    /// </summary>
    public class HealthDbContext : DbContext
    {
        // Define DbSet properties for each of your domain entities
        // These will correspond to tables in your database.
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Visit> Visits { get; set; }

        /// <summary>
        /// Constructor for HealthDbContext.
        /// It explicitly calls the base DbContext constructor with the connection string name
        /// defined in the Web.config file.
        /// </summary>
        public HealthDbContext() : base("name=HealthDbContext")
        {
            // You can configure database initializers here if needed.
            // For example, to create the database if it doesn't exist,
            // or to migrate to the latest version.
            // Database.SetInitializer(new CreateDatabaseIfNotExists<HealthDbContext>()); // Already configured in Web.config
            // Database.SetInitializer(new MigrateDatabaseToLatestVersion<HealthDbContext, Migrations.Configuration>());
        }

        /// <summary>
        /// Overrides the OnModelCreating method to apply entity configurations.
        /// This is where you define table names, primary keys, relationships,
        /// and column properties using Fluent API or by applying EntityTypeConfiguration classes.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Disable cascade delete for all relationships by default, then enable selectively if needed.
            // This is generally a good practice to prevent accidental data loss.
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.ManyToManyCascadeDeleteConvention>();

            // Apply entity-specific configurations from the Configurations folder.
            // These configurations help in mapping your domain entities to database tables
            // with specific properties and relationships (e.g., column types, max lengths, unique indexes).
            modelBuilder.Configurations.Add(new PatientConfiguration());
            modelBuilder.Configurations.Add(new DoctorConfiguration());
            modelBuilder.Configurations.Add(new VisitConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
