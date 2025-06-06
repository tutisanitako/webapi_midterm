using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class VisitRepository : RepositoryBase<Visit>, IVisitRepository
    {
        public VisitRepository(DbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Visit>> GetVisitsByDoctorAsync(int doctorId)
        {
            return await _context.Set<Visit>()
                .Include(v => v.Patient)
                .Include(v => v.Doctor)
                .Where(v => v.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Visit>> GetVisitsByPatientAsync(int patientId)
        {
            return await _context.Set<Visit>()
                .Include(v => v.Patient)
                .Include(v => v.Doctor)
                .Where(v => v.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Visit>> GetVisitsWithFiltersAsync(
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
            var query = _context.Set<Visit>()
                .Include(v => v.Patient)
                .Include(v => v.Doctor)
                .AsQueryable();

            // Apply filters
            if (doctorId.HasValue)
                query = query.Where(v => v.DoctorId == doctorId.Value);
            if (visitDateFrom.HasValue)
                query = query.Where(v => v.VisitDate >= visitDateFrom.Value);
            if (visitDateTo.HasValue)
                query = query.Where(v => v.VisitDate <= visitDateTo.Value);
            if (minFee.HasValue)
                query = query.Where(v => v.Fee >= minFee.Value);
            if (maxFee.HasValue)
                query = query.Where(v => v.Fee <= maxFee.Value);

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

            return await query.ToListAsync();
        }

        public async Task<int> CountDoctorVisitsAsync(int doctorId)
        {
            return await _context.Set<Visit>()
                .CountAsync(v => v.DoctorId == doctorId);
        }

        public async Task<decimal> CalculateTotalBillingForPatientAsync(int patientId)
        {
            return await _context.Set<Visit>()
                .Where(v => v.PatientId == patientId)
                .SumAsync(v => v.Fee);
        }

        public async Task<bool> HasVisitOnDateAsync(int patientId, DateTime visitDate)
        {
            return await _context.Set<Visit>()
                .AnyAsync(v => v.PatientId == patientId && v.VisitDate.Date == visitDate.Date);
        }
    }
}