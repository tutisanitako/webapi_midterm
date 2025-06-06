using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    // ApplicationException is defined in its own file: FinalAPI.Application/Services/ApplicationException.cs

    /// <summary>
    /// Manages business logic for Visit operations, including CRUD, pagination, filtering, and validation.
    /// </summary>
    public class VisitService
    {
        private readonly IVisitRepository _visitRepository;
        private readonly IRepository<Patient> _patientRepository;
        private readonly IRepository<Doctor> _doctorRepository;

        public VisitService(IVisitRepository visitRepository, IRepository<Patient> patientRepository, IRepository<Doctor> doctorRepository)
        {
            _visitRepository = visitRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
        }

        /// <summary>
        /// Creates a new visit.
        /// Includes validation for fee, patient/doctor existence, and unique visit date per patient.
        /// </summary>
        /// <param name="visitDto">The DTO containing visit creation data.</param>
        /// <returns>The created visit's read DTO.</returns>
        /// <exception cref="ApplicationException">Thrown if validation fails.</exception>
        public async Task<VisitReadDto> CreateVisitAsync(VisitCreateDto visitDto)
        {
            // Validate Visit.Fee
            if (visitDto.Fee <= 0 || visitDto.Fee >= 1000)
            {
                throw new AppServiceException("Visit fee must be greater than 0 and less than 1000.");
            }

            // Check if Patient exists
            var patient = await _patientRepository.GetByIdAsync(visitDto.PatientId);
            if (patient == null)
            {
                throw new AppServiceException($"Patient with ID {visitDto.PatientId} not found.");
            }

            // Check if Doctor exists
            var doctor = await _doctorRepository.GetByIdAsync(visitDto.DoctorId);
            if (doctor == null)
            {
                throw new AppServiceException($"Doctor with ID {visitDto.DoctorId} not found.");
            }

            // Check if the same patient has more than one visit on the same day
            if (await _visitRepository.HasVisitOnDateAsync(visitDto.PatientId, visitDto.VisitDate))
            {
                throw new AppServiceException($"Patient {patient.FullName} already has a visit on {visitDto.VisitDate.ToShortDateString()}.");
            }

            // Doctor specialization validation (as per task description: "Doctor specialization must be entered when adding a visit")
            // This implicitly means the Doctor record itself must exist and have a specialization.
            // Since we checked if doctor exists, and our Doctor entity requires specialization, this is covered.
            if (string.IsNullOrWhiteSpace(doctor.Specialization))
            {
                throw new AppServiceException($"Doctor {doctor.FullName} must have a specialization defined before a visit can be created for them.");
            }

            var visit = new Visit
            {
                PatientId = visitDto.PatientId,
                DoctorId = visitDto.DoctorId,
                VisitDate = visitDto.VisitDate,
                Fee = visitDto.Fee
            };

            await _visitRepository.AddAsync(visit);

            return new VisitReadDto
            {
                Id = visit.Id,
                PatientId = visit.PatientId,
                PatientFullName = patient.FullName,
                PatientBirthDate = patient.BirthDate,
                DoctorId = visit.DoctorId,
                DoctorFullName = doctor.FullName,
                DoctorSpecialization = doctor.Specialization,
                VisitDate = visit.VisitDate,
                Fee = visit.Fee
            };
        }

        /// <summary>
        /// Retrieves a visit by ID.
        /// </summary>
        /// <param name="id">The visit ID.</param>
        /// <returns>The visit's read DTO, or null if not found.</returns>
        public async Task<VisitReadDto> GetVisitByIdAsync(int id)
        {
            var visit = await _visitRepository.GetByIdAsync(id);
            if (visit == null)
            {
                return null; // Explicitly return null if not found
            }

            // To populate PatientFullName and DoctorFullName in VisitReadDto,
            // we need to explicitly load them if they are not included by default in the repository's GetByIdAsync.
            // Our current RepositoryBase doesn't include them, so let's fetch them here.
            var patient = await _patientRepository.GetByIdAsync(visit.PatientId);
            var doctor = await _doctorRepository.GetByIdAsync(visit.DoctorId);

            return new VisitReadDto
            {
                Id = visit.Id,
                PatientId = visit.PatientId,
                PatientFullName = (patient != null) ? patient.FullName : "N/A",
                PatientBirthDate = (patient != null) ? patient.BirthDate : default(DateTime),
                DoctorId = visit.DoctorId,
                DoctorFullName = (doctor != null) ? doctor.FullName : "N/A",
                DoctorSpecialization = (doctor != null) ? doctor.Specialization : "N/A",
                VisitDate = visit.VisitDate,
                Fee = visit.Fee
            };
        }

        /// <summary>
        /// Retrieves a paged and filtered list of visits.
        /// </summary>
        /// <param name="queryParameters">Parameters for filtering, sorting, and pagination.</param>
        /// <returns>A paged result of visit read DTOs.</returns>
        public async Task<PagedResult<VisitReadDto>> GetVisitsAsync(VisitQueryParameters queryParameters)
        {
            // For now, assuming VisitRepository.GetVisitsWithFiltersAsync can provide the total queryable.
            // This is not efficient for large datasets but works for now without deeper EF6 changes.
            var allFilteredVisits = (await _visitRepository.GetVisitsWithFiltersAsync(
                queryParameters.DoctorId,
                queryParameters.VisitDateFrom,
                queryParameters.VisitDateTo,
                queryParameters.MinFee,
                queryParameters.MaxFee,
                null, // No sorting for count
                null, // No sorting for count
                1, // Page 1
                int.MaxValue // Get all for count
            )).ToList();

            var totalCount = allFilteredVisits.Count();

            // Then get the paginated and sorted results
            var visits = (await _visitRepository.GetVisitsWithFiltersAsync(
                queryParameters.DoctorId,
                queryParameters.VisitDateFrom,
                queryParameters.VisitDateTo,
                queryParameters.MinFee,
                queryParameters.MaxFee,
                queryParameters.SortBy,
                queryParameters.SortDirection,
                queryParameters.PageNumber,
                queryParameters.PageSize
            )).ToList(); // Convert to List to ensure full enumeration

            var visitReadDtos = new List<VisitReadDto>();
            foreach (var visit in visits)
            {
                var patient = await _patientRepository.GetByIdAsync(visit.PatientId);
                var doctor = await _doctorRepository.GetByIdAsync(visit.DoctorId);

                visitReadDtos.Add(new VisitReadDto
                {
                    Id = visit.Id,
                    PatientId = visit.PatientId,
                    PatientFullName = (patient != null) ? patient.FullName : "N/A",
                    PatientBirthDate = (patient != null) ? patient.BirthDate : default(DateTime),
                    DoctorId = visit.DoctorId,
                    DoctorFullName = (doctor != null) ? doctor.FullName : "N/A",
                    DoctorSpecialization = (doctor != null) ? doctor.Specialization : "N/A",
                    VisitDate = visit.VisitDate,
                    Fee = visit.Fee
                });
            }

            var totalPages = (int)Math.Ceiling((double)totalCount / queryParameters.PageSize);

            return new PagedResult<VisitReadDto>
            {
                Items = visitReadDtos,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize
            };
        }


        /// <summary>
        /// Updates an existing visit.
        /// Includes validation for fee and patient/doctor existence.
        /// </summary>
        /// <param name="visitDto">The DTO containing updated visit data.</param>
        /// <returns>True if updated successfully, false if visit not found.</returns>
        /// <exception cref="ApplicationException">Thrown if validation fails.</exception>
        public async Task<bool> UpdateVisitAsync(VisitUpdateDto visitDto)
        {
            var existingVisit = await _visitRepository.GetByIdAsync(visitDto.Id);
            if (existingVisit == null)
            {
                return false; // Visit not found
            }

            // Validate Visit.Fee
            if (visitDto.Fee <= 0 || visitDto.Fee >= 1000)
            {
                throw new AppServiceException("Visit fee must be greater than 0 and less than 1000.");
            }

            // Check if Patient exists
            var patient = await _patientRepository.GetByIdAsync(visitDto.PatientId);
            if (patient == null)
            {
                throw new AppServiceException($"Patient with ID {visitDto.PatientId} not found.");
            }

            // Check if Doctor exists
            var doctor = await _doctorRepository.GetByIdAsync(visitDto.DoctorId);
            if (doctor == null)
            {
                throw new AppServiceException($"Doctor with ID {visitDto.DoctorId} not found.");
            }

            // Check if the same patient has more than one visit on the same day (excluding the current visit being updated)
            // This is a crucial validation point for updates.
            if (existingVisit.PatientId != visitDto.PatientId || existingVisit.VisitDate.Date != visitDto.VisitDate.Date)
            {
                if (await _visitRepository.HasVisitOnDateAsync(visitDto.PatientId, visitDto.VisitDate))
                {
                    throw new AppServiceException($"Patient {patient.FullName} already has a visit on {visitDto.VisitDate.ToShortDateString()}.");
                }
            }

            // Doctor specialization validation
            if (string.IsNullOrWhiteSpace(doctor.Specialization))
            {
                throw new AppServiceException($"Doctor {doctor.FullName} must have a specialization defined before a visit can be updated for them.");
            }

            existingVisit.PatientId = visitDto.PatientId;
            existingVisit.DoctorId = visitDto.DoctorId;
            existingVisit.VisitDate = visitDto.VisitDate;
            existingVisit.Fee = visitDto.Fee;

            await _visitRepository.UpdateAsync(existingVisit);
            return true;
        }

        /// <summary>
        /// Deletes a visit by ID.
        /// </summary>
        /// <param name="id">The ID of the visit to delete.</param>
        /// <returns>True if deleted successfully, false if visit not found.</returns>
        public async Task<bool> DeleteVisitAsync(int id)
        {
            var visit = await _visitRepository.GetByIdAsync(id);
            if (visit == null)
            {
                return false; // Visit not found
            }

            await _visitRepository.DeleteAsync(id);
            return true;
        }
    }
}
