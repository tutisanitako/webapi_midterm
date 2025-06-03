using FinalAPI.Domain.Entities;
using FinalAPI.Infrastructure.Data;

namespace FinalAPI.Infrastructure.Repositories
{
    public class PatientRepository : RepositoryBase<Patient>
    {
        public PatientRepository(HealthDbContext context) : base(context)
        {
        }
    }
}