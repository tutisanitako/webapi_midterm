using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    /// <summary>
    /// Defines specific operations for the Visit entity, extending the generic repository.
    /// </summary>
    public interface IVisitRepository : IRepository<Visit>
    {
        Task<IEnumerable<Visit>> GetVisitsByDoctorAsync(int doctorId);
        Task<IEnumerable<Visit>> GetVisitsByPatientAsync(int patientId);

        Task<IQueryable<Visit>> GetFilteredVisitsQueryable( 
            int? doctorId,
            DateTime? visitDateFrom,
            DateTime? visitDateTo,
            decimal? minFee,
            decimal? maxFee);

        Task<int> GetFilteredVisitsCountAsync(
            int? doctorId,
            DateTime? visitDateFrom,
            DateTime? visitDateTo,
            decimal? minFee,
            decimal? maxFee);


        Task<int> CountDoctorVisitsAsync(int doctorId);
        Task<decimal> CalculateTotalBillingForPatientAsync(int patientId);
        Task<bool> HasVisitOnDateAsync(int patientId, DateTime visitDate);
    }
}