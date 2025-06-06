using Domain.Entities;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema; // Needed for IndexAnnotation
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.Configurations
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

            // UNIQUE CONSTRAINT FOR PatientId + VisitDate (EF6 way)
            // This creates a unique non-clustered index named "IX_PatientId_VisitDate"
            Property(v => v.VisitDate)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_PatientId_VisitDate", 1) { IsUnique = true }));

            Property(v => v.PatientId)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_PatientId_VisitDate", 2) { IsUnique = true }));
            // Note: The order (1 and 2) in IndexAttribute specifies the order of columns in the composite index.
            // It's important for `IsUnique = true` to be set on both properties for a composite unique index.
        }
    }
}
