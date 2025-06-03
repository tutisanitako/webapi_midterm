using FinalAPI.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace FinalAPI.Infrastructure.Data.Configurations
{
    public class VisitConfiguration : EntityTypeConfiguration<Visit>
    {
        public VisitConfiguration()
        {
            ToTable("Visits");

            HasKey(v => v.Id);
            Property(v => v.Id).IsRequired();
            Property(v => v.PatientId).IsRequired();
            Property(v => v.DoctorId).IsRequired();
            Property(v => v.VisitDate).IsRequired().HasColumnType("datetime");
            Property(v => v.Fee).IsRequired().HasColumnType("decimal").HasPrecision(18, 2);

            // Foreign key relationships
            HasRequired(v => v.Patient)
                .WithMany(p => p.Visits)
                .HasForeignKey(v => v.PatientId)
                .WillCascadeOnDelete(false);

            HasRequired(v => v.Doctor)
                .WithMany(d => d.Visits)
                .HasForeignKey(v => v.DoctorId)
                .WillCascadeOnDelete(false);

            // Unique constraint for PatientId + VisitDate
            HasIndex(v => new { v.PatientId, v.VisitDate })
                .IsUnique();
        }
    }
}