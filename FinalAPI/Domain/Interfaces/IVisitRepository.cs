using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    /// <summary>
    /// Defines specific operations for the Visit entity, extending the generic repository.
    /// </summary>
    public interface IVisitRepository : IRepository<Visit>
    {
        // Add specific methods related to Visit filtering, pagination, and calculations
        Task<IEnumerable<Visit>> GetVisitsByDoctorAsync(int doctorId); // Your existing custom method
        Task<IEnumerable<Visit>> GetVisitsByPatientAsync(int patientId); // Your existing custom method

        Task<IEnumerable<Visit>> GetVisitsWithFiltersAsync(
            int? doctorId,
            DateTime? visitDateFrom,
            DateTime? visitDateTo,
            decimal? minFee,
            decimal? maxFee,
            string sortBy,
            string sortDirection,
            int pageNumber,
            int pageSize);

        Task<int> CountDoctorVisitsAsync(int doctorId);
        Task<decimal> CalculateTotalBillingForPatientAsync(int patientId);
        Task<bool> HasVisitOnDateAsync(int patientId, DateTime visitDate);
    }
}