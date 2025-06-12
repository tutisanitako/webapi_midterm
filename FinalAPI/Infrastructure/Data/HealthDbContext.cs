using System.Data.Entity;
using Domain.Entities;
using Infrastructure.Data.Configurations;
using System.Configuration;

namespace Infrastructure.Data
{
    /// <summary>
    /// Represents the database context for the Health Billing System,
    /// using Entity Framework 6 (EF6) Code-First approach.
    /// </summary>
    public class HealthDbContext : DbContext
    {

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
        }

        /// <summary>
        /// Overrides the OnModelCreating method to apply entity configurations.
        /// This is where you define table names, primary keys, relationships,
        /// and column properties using Fluent API or by applying EntityTypeConfiguration classes.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.ManyToManyCascadeDeleteConvention>();

            modelBuilder.Configurations.Add(new PatientConfiguration());
            modelBuilder.Configurations.Add(new DoctorConfiguration());
            modelBuilder.Configurations.Add(new VisitConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}