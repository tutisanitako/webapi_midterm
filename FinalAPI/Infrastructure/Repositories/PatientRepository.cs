using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class PatientRepository : RepositoryBase<Patient>
    {
        public PatientRepository(HealthDbContext context) : base(context)
        {
        }
    }
}