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
    /// Manages business logic for Patient operations.
    /// </summary>
    public class PatientService
    {
        private readonly IRepository<Patient> _patientRepository;
        private readonly IVisitRepository _visitRepository; // To check total billing

        public PatientService(IRepository<Patient> patientRepository, IVisitRepository visitRepository)
        {
            _patientRepository = patientRepository;
            _visitRepository = visitRepository;
        }

        /// <summary>
        /// Creates a new patient.
        /// </summary>
        /// <param name="patientDto">The DTO containing patient creation data.</param>
        /// <returns>The created patient's read DTO.</returns>
        /// <exception cref="ApplicationException">Thrown if validation fails (e.g., invalid birth date).</exception>
        public async Task<PatientReadDto> CreatePatientAsync(PatientCreateDto patientDto)
        {
            // Basic validation example
            if (string.IsNullOrWhiteSpace(patientDto.FullName))
            {
                throw new AppServiceException("Patient full name is required.");
            }
            if (patientDto.BirthDate == default(DateTime) || patientDto.BirthDate > DateTime.Today)
            {
                throw new AppServiceException("Patient birth date is invalid.");
            }

            var patient = new Patient
            {
                FullName = patientDto.FullName,
                BirthDate = patientDto.BirthDate
            };

            await _patientRepository.AddAsync(patient);

            return new PatientReadDto
            {
                Id = patient.Id,
                FullName = patient.FullName,
                BirthDate = patient.BirthDate
            };
        }

        /// <summary>
        /// Retrieves a patient by ID.
        /// </summary>
        /// <param name="id">The patient ID.</param>
        /// <returns>The patient's read DTO, or null if not found.</returns>
        public async Task<PatientReadDto> GetPatientByIdAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                return null; // Explicitly return null if not found
            }

            return new PatientReadDto
            {
                Id = patient.Id,
                FullName = patient.FullName,
                BirthDate = patient.BirthDate
            };
        }

        /// <summary>
        /// Retrieves all patients.
        /// </summary>
        /// <returns>A list of patient read DTOs.</returns>
        public async Task<IReadOnlyList<PatientReadDto>> GetAllPatientsAsync()
        {
            var patients = await _patientRepository.GetAllAsync();
            return patients.Select(p => new PatientReadDto
            {
                Id = p.Id,
                FullName = p.FullName,
                BirthDate = p.BirthDate
            }).ToList();
        }

        /// <summary>
        /// Updates an existing patient.
        /// </summary>
        /// <param name="patientDto">The DTO containing updated patient data.</param>
        /// <returns>True if updated successfully, false if patient not found.</returns>
        /// <exception cref="ApplicationException">Thrown if validation fails.</exception>
        public async Task<bool> UpdatePatientAsync(PatientUpdateDto patientDto)
        {
            var patient = await _patientRepository.GetByIdAsync(patientDto.Id);
            if (patient == null)
            {
                return false; // Patient not found
            }

            // Validation before updating
            if (string.IsNullOrWhiteSpace(patientDto.FullName))
            {
                throw new AppServiceException("Patient full name is required.");
            }
            if (patientDto.BirthDate == default(DateTime) || patientDto.BirthDate > DateTime.Today)
            {
                throw new AppServiceException("Patient birth date is invalid.");
            }

            patient.FullName = patientDto.FullName;
            patient.BirthDate = patientDto.BirthDate;

            await _patientRepository.UpdateAsync(patient);
            return true;
        }

        /// <summary>
        /// Deletes a patient by ID.
        /// </summary>
        /// <param name="id">The ID of the patient to delete.</param>
        /// <returns>True if deleted successfully, false if patient not found.</returns>
        public async Task<bool> DeletePatientAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                return false; // Patient not found
            }

            await _patientRepository.DeleteAsync(id);
            return true;
        }

        /// <summary>
        /// Calculates the total billing for a specific patient.
        /// </summary>
        /// <param name="patientId">The ID of the patient.</param>
        /// <returns>A DTO containing the patient's billing summary, or null if patient not found.</returns>
        public async Task<BillingSummaryDto> CalculateTotalBillingForPatientAsync(int patientId)
        {
            var patient = await _patientRepository.GetByIdAsync(patientId);
            if (patient == null)
            {
                return null; // Explicitly return null if patient not found
            }

            var totalFee = await _visitRepository.CalculateTotalBillingForPatientAsync(patientId);

            return new BillingSummaryDto
            {
                PatientId = patient.Id,
                PatientFullName = patient.FullName,
                TotalPaid = totalFee
            };
        }
    }
}
