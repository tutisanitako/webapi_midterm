using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class DoctorRepository : RepositoryBase<Doctor>
    {
        public DoctorRepository(HealthDbContext context) : base(context)
        {
        }
    }
}