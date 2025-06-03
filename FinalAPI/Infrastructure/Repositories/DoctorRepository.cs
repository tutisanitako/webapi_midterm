using FinalAPI.Domain.Entities;
using FinalAPI.Infrastructure.Data;

namespace FinalAPI.Infrastructure.Repositories
{
    public class DoctorRepository : RepositoryBase<Doctor>
    {
        public DoctorRepository(HealthDbContext context) : base(context)
        {
        }
    }
}