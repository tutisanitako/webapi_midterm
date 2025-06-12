using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity; 
using System.Linq; 
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Repository for managing Visit entities, implementing IVisitRepository.
    /// Provides specific query methods beyond generic CRUD for visits,
    /// including filtered and paginated queries for efficient data retrieval.
    /// Inherits from RepositoryBase for common CRUD operations.
    /// </summary>
    public class VisitRepository : RepositoryBase<Visit>, IVisitRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisitRepository"/> class.
        /// </summary>
        /// <param name="context">The database context (HealthDbContext) to be used by the repository.</param>
        public VisitRepository(DbContext context) : base(context)
        {
        }

        /// <summary>
        /// Asynchronously retrieves all visits associated with a specific doctor,
        /// including their related Patient and Doctor entities.
        /// </summary>
        /// <param name="doctorId">The ID of the doctor.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of visits.</returns>
        public async Task<IEnumerable<Visit>> GetVisitsByDoctorAsync(int doctorId)
        {
            return await _context.Set<Visit>()
                .Include(v => v.Patient)
                .Include(v => v.Doctor)
                .Where(v => v.DoctorId == doctorId)
                .ToListAsync();
        }

        /// <summary>
        /// Asynchronously retrieves all visits associated with a specific patient,
        /// including their related Patient and Doctor entities.
        /// </summary>
        /// <param name="patientId">The ID of the patient.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of visits.</returns>
        public async Task<IEnumerable<Visit>> GetVisitsByPatientAsync(int patientId)
        {
            return await _context.Set<Visit>()
                .Include(v => v.Patient)
                .Include(v => v.Doctor)
                .Where(v => v.PatientId == patientId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a filtered IQueryable for visits, allowing for further composition
        /// (e.g., sorting, pagination) in the application service layer without premature execution.
        /// Eagerly loads Patient and Doctor entities.
        /// </summary>
        /// <param name="doctorId">Optional doctor ID to filter by.</param>
        /// <param name="visitDateFrom">Optional start date to filter visits from.</param>
        /// <param name="visitDateTo">Optional end date to filter visits to (inclusive of the entire day).</param>
        /// <param name="minFee">Optional minimum fee to filter by.</param>
        /// <param name="maxFee">Optional maximum fee to filter by.</param>
        /// <returns>A Task containing an IQueryable of Visit entities with applied filters.</returns>
        public Task<IQueryable<Visit>> GetFilteredVisitsQueryable( // Note: Returns Task<IQueryable> to align with async pattern, but the IQueryable itself is built synchronously.
            int? doctorId,
            DateTime? visitDateFrom,
            DateTime? visitDateTo,
            decimal? minFee,
            decimal? maxFee)
        {
            var query = _context.Set<Visit>()
                .Include(v => v.Patient)
                .Include(v => v.Doctor)
                .AsQueryable();

            if (doctorId.HasValue)
                query = query.Where(v => v.DoctorId == doctorId.Value);

            if (visitDateFrom.HasValue)
                query = query.Where(v => v.VisitDate >= visitDateFrom.Value);

            if (visitDateTo.HasValue)
            {
                query = query.Where(v => v.VisitDate < DbFunctions.AddDays(visitDateTo.Value, 1));
            }

            if (minFee.HasValue)
                query = query.Where(v => v.Fee >= minFee.Value);

            if (maxFee.HasValue)
                query = query.Where(v => v.Fee <= maxFee.Value);

            return Task.FromResult(query);
        }

        /// <summary>
        /// Asynchronously gets the total count of visits based on the specified filters.
        /// This method executes a database COUNT(*) query efficiently.
        /// </summary>
        /// <param name="doctorId">Optional doctor ID to filter by.</param>
        /// <param name="visitDateFrom">Optional start date to filter visits from.</param>
        /// <param name="visitDateTo">Optional end date to filter visits to (inclusive of the entire day).</param>
        /// <param name="minFee">Optional minimum fee to filter by.</param>
        /// <param name="maxFee">Optional maximum fee to filter by.</param>
        /// <returns>A Task that represents the asynchronous operation, containing the total count of filtered visits.</returns>
        public async Task<int> GetFilteredVisitsCountAsync(
            int? doctorId,
            DateTime? visitDateFrom,
            DateTime? visitDateTo,
            decimal? minFee,
            decimal? maxFee)
        {
            var query = _context.Set<Visit>().AsQueryable();

            if (doctorId.HasValue)
                query = query.Where(v => v.DoctorId == doctorId.Value);

            if (visitDateFrom.HasValue)
                query = query.Where(v => v.VisitDate >= visitDateFrom.Value);

            if (visitDateTo.HasValue)
            {
                query = query.Where(v => v.VisitDate < DbFunctions.AddDays(visitDateTo.Value, 1));
            }

            if (minFee.HasValue)
                query = query.Where(v => v.Fee >= minFee.Value);

            if (maxFee.HasValue)
                query = query.Where(v => v.Fee <= maxFee.Value);

            return await query.CountAsync(); 
        }


        /// <summary>
        /// Asynchronously counts the total number of visits for a specific doctor.
        /// </summary>
        /// <param name="doctorId">The ID of the doctor.</param>
        /// <returns>A task that represents the asynchronous operation, containing the total visit count for the doctor.</returns>
        public async Task<int> CountDoctorVisitsAsync(int doctorId)
        {
            return await _context.Set<Visit>()
                .CountAsync(v => v.DoctorId == doctorId);
        }

        /// <summary>
        /// Asynchronously calculates the total billing amount (sum of fees) for a specific patient.
        /// </summary>
        /// <param name="patientId">The ID of the patient.</param>
        /// <returns>A task that represents the asynchronous operation, containing the total billing amount for the patient.</returns>
        public async Task<decimal> CalculateTotalBillingForPatientAsync(int patientId)
        {
            return await _context.Set<Visit>()
                .Where(v => v.PatientId == patientId)
                .SumAsync(v => v.Fee);
        }

        /// <summary>
        /// Asynchronously checks if a patient already has a visit scheduled on a specific date.
        /// Uses DbFunctions.TruncateTime to compare only the date parts.
        /// </summary>
        /// <param name="patientId">The ID of the patient.</param>
        /// <param name="visitDate">The date to check for an existing visit (time part will be ignored).</param>
        /// <returns>A task that represents the asynchronous operation, containing true if a visit exists on that date, false otherwise.</returns>
        public async Task<bool> HasVisitOnDateAsync(int patientId, DateTime visitDate)
        {
            return await _context.Set<Visit>()
                .AnyAsync(v => v.PatientId == patientId && DbFunctions.TruncateTime(v.VisitDate) == DbFunctions.TruncateTime(visitDate));
        }
    }
}