using FinalAPI.Domain.Entities;
using FinalAPI.Domain.Interfaces;
using FinalAPI.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace FinalAPI.Infrastructure.Repositories
{
    public class VisitRepository : RepositoryBase<Visit>, IVisitRepository
    {
        public VisitRepository(HealthDbContext context) : base(context)
        {
        }

        public Task<IEnumerable<Visit>> GetVisitsByDoctorAsync(int doctorId)
        {
            var visits = _context.Visits
                .Include("Patient")
                .Include("Doctor")
                .Where(v => v.DoctorId == doctorId)
                .ToList();
            return Task.FromResult<IEnumerable<Visit>>(visits);
        }

        public Task<IEnumerable<Visit>> GetVisitsByPatientAsync(int patientId)
        {
            var visits = _context.Visits
                .Include("Patient")
                .Include("Doctor")
                .Where(v => v.PatientId == patientId)
                .ToList();
            return Task.FromResult<IEnumerable<Visit>>(visits);
        }

        public Task<IEnumerable<Visit>> GetVisitsWithFiltersAsync(
            int? doctorId,
            DateTime? visitDateFrom,
            DateTime? visitDateTo,
            decimal? minFee,
            decimal? maxFee,
            string sortBy,
            string sortDirection,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Visits
                .Include("Patient")
                .Include("Doctor")
                .AsQueryable();

            // Apply filters
            if (doctorId.HasValue)
                query = query.Where(v => v.DoctorId == doctorId);
            if (visitDateFrom.HasValue)
                query = query.Where(v => v.VisitDate >= visitDateFrom);
            if (visitDateTo.HasValue)
                query = query.Where(v => v.VisitDate <= visitDateTo);
            if (minFee.HasValue)
                query = query.Where(v => v.Fee >= minFee);
            if (maxFee.HasValue)
                query = query.Where(v => v.Fee <= maxFee);

            // Apply sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortBy.ToLower() == "fee")
                    query = sortDirection.ToLower() == "desc"
                        ? query.OrderByDescending(v => v.Fee)
                        : query.OrderBy(v => v.Fee);
                else if (sortBy.ToLower() == "visitdate")
                    query = sortDirection.ToLower() == "desc"
                        ? query.OrderByDescending(v => v.VisitDate)
                        : query.OrderBy(v => v.VisitDate);
            }

            // Apply pagination
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return Task.FromResult<IEnumerable<Visit>>(query.ToList());
        }

        public Task<int> CountDoctorVisitsAsync(int doctorId)
        {
            var count = _context.Visits
                .Count(v => v.DoctorId == doctorId);
            return Task.FromResult(count);
        }

        public Task<decimal> CalculateTotalBillingForPatientAsync(int patientId)
        {
            var total = _context.Visits
                .Where(v => v.PatientId == patientId)
                .Sum(v => v.Fee);
            return Task.FromResult(total);
        }

        public Task<bool> HasVisitOnDateAsync(int patientId, DateTime visitDate)
        {
            var hasVisit = _context.Visits
                .Any(v => v.PatientId == patientId && v.VisitDate.Date == visitDate.Date);
            return Task.FromResult(hasVisit);
        }
    }
}