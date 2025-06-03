using FinalAPI.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace FinalAPI.Infrastructure.Data.Configurations
{
    public class PatientConfiguration : EntityTypeConfiguration<Patient>
    {
        public PatientConfiguration()
        {
            ToTable("Patients");

            HasKey(p => p.Id);
            Property(p => p.Id).IsRequired();
            Property(p => p.FullName).IsRequired().HasMaxLength(100);
            Property(p => p.BirthDate).IsRequired().HasColumnType("date");
        }
    }
}