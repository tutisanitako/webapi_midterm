using FinalAPI.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace FinalAPI.Infrastructure.Data.Configurations
{
    public class DoctorConfiguration : EntityTypeConfiguration<Doctor>
    {
        public DoctorConfiguration()
        {
            ToTable("Doctors");

            HasKey(d => d.Id);
            Property(d => d.Id).IsRequired();
            Property(d => d.FullName).IsRequired().HasMaxLength(100);
            Property(d => d.Specialization).IsRequired().HasMaxLength(50);
        }
    }
}