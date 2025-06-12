using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    /// <summary>
    /// Manages business logic for Patient operations.
    /// </summary>
    public class PatientService
    {
        private readonly IRepository<Patient> _patientRepository;
        private readonly IVisitRepository _visitRepository;

        public PatientService(IRepository<Patient> patientRepository, IVisitRepository visitRepository)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _visitRepository = visitRepository ?? throw new ArgumentNullException(nameof(visitRepository));
        }

        public async Task<PatientReadDto> CreatePatientAsync(PatientCreateDto patientDto)
        {
            if (string.IsNullOrWhiteSpace(patientDto.FullName))
                throw new AppServiceException("Patient full name is required.");
            if (patientDto.BirthDate == default(DateTime))
                throw new AppServiceException("Patient birth date is required.");
            if (patientDto.BirthDate > DateTime.Today)
                throw new AppServiceException("Patient birth date is invalid.");

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

        public async Task<PatientReadDto> GetPatientByIdAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                return null;
            }

            return new PatientReadDto
            {
                Id = patient.Id,
                FullName = patient.FullName,
                BirthDate = patient.BirthDate
            };
        }

        public async Task<IReadOnlyList<PatientReadDto>> GetAllAsync()
        {
            var patients = await _patientRepository.GetAllAsync();
            return patients.Select(p => new PatientReadDto
            {
                Id = p.Id,
                FullName = p.FullName,
                BirthDate = p.BirthDate
            }).ToList();
        }

        public async Task UpdatePatientAsync(int id, PatientUpdateDto patientDto)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                throw new AppServiceException("Patient not found.");

            if (string.IsNullOrWhiteSpace(patientDto.FullName))
                throw new AppServiceException("Patient full name is required.");
            if (patientDto.BirthDate == default(DateTime) || patientDto.BirthDate > DateTime.Today)
                throw new AppServiceException("Patient birth date is invalid.");

            patient.FullName = patientDto.FullName;
            patient.BirthDate = patientDto.BirthDate;

            await _patientRepository.UpdateAsync(patient);
        }

        public async Task DeletePatientAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                throw new AppServiceException("Patient not found.");
            }

            await _patientRepository.DeleteAsync(id);
        }

        public async Task<BillingSummaryDto> CalculateTotalBillingForPatientAsync(int patientId)
        {
            var patient = await _patientRepository.GetByIdAsync(patientId);
            if (patient == null)
            {
                return null;
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