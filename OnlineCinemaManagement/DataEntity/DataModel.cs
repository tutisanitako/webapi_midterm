using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace DataEntity
{
    public partial class DataModel : DbContext
    {
        public DataModel()
            : base("name=dataModel")
        {
        }

        public virtual DbSet<Bookings> Bookings { get; set; }
        public virtual DbSet<Genres> Genres { get; set; }
        public virtual DbSet<Halls> Halls { get; set; }
        public virtual DbSet<Movies> Movies { get; set; }
        public virtual DbSet<SeatAssignments> SeatAssignments { get; set; }
        public virtual DbSet<Showtimes> Showtimes { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Genres>()
                .HasMany(e => e.Movies)
                .WithOptional(e => e.Genres)
                .HasForeignKey(e => e.Genre);

            modelBuilder.Entity<Showtimes>()
                .Property(e => e.TicketPrice)
                .HasPrecision(10, 2);
        }
    }
}
