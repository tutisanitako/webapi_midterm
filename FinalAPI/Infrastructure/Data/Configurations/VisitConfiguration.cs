using Domain.Entities;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
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

            HasRequired(v => v.Patient)
                .WithMany(p => p.Visits)
                .HasForeignKey(v => v.PatientId)
                .WillCascadeOnDelete(false);

            HasRequired(v => v.Doctor)
                .WithMany(d => d.Visits)
                .HasForeignKey(v => v.DoctorId)
                .WillCascadeOnDelete(false);

            Property(v => v.VisitDate)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_PatientId_VisitDate", 1) { IsUnique = true }));

            Property(v => v.PatientId)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_PatientId_VisitDate", 2) { IsUnique = true }));
        }
    }
}