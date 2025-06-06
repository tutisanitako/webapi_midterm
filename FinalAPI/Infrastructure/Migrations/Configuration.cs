namespace Infrastructure.Migrations
{
    using System.Data.Entity.Migrations;
    using Infrastructure.Data;

    internal sealed class Configuration : DbMigrationsConfiguration<HealthDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false; // Recommended to set to false for explicit control
        }

        protected override void Seed(HealthDbContext context)
        {
            // Add seed data here if needed
        }
    }
}