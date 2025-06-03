using FinalAPI.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace FinalAPI.Domain.Interfaces
{
    public interface IVisitRepository : IRepository<Visit>
    {
        Task<IEnumerable<Visit>> GetVisitsByDoctorAsync(int doctorId);
        Task<IEnumerable<Visit>> GetVisitsByPatientAsync(int patientId);
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