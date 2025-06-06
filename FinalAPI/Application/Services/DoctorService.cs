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
    /// Manages business logic for Doctor operations.
    /// As per task, only Read and Create operations are allowed for Doctors.
    /// </summary>
    public class DoctorService
    {
        private readonly IRepository<Doctor> _doctorRepository;
        private readonly IVisitRepository _visitRepository; // To count visits

        public DoctorService(IRepository<Doctor> doctorRepository, IVisitRepository visitRepository)
        {
            _doctorRepository = doctorRepository;
            _visitRepository = visitRepository;
        }

        /// <summary>
        /// Creates a new doctor.
        /// </summary>
        /// <param name="doctorDto">The DTO containing doctor creation data.</param>
        /// <returns>The created doctor's read DTO.</returns>
        /// <exception cref="ApplicationException">Thrown if validation fails.</exception>
        public async Task<DoctorReadDto> CreateDoctorAsync(DoctorCreateDto doctorDto)
        {
            if (string.IsNullOrWhiteSpace(doctorDto.FullName))
            {
                throw new AppServiceException("Doctor full name is required.");
            }
            if (string.IsNullOrWhiteSpace(doctorDto.Specialization))
            {
                throw new AppServiceException("Doctor specialization is required.");
            }

            var doctor = new Doctor
            {
                FullName = doctorDto.FullName,
                Specialization = doctorDto.Specialization
            };

            await _doctorRepository.AddAsync(doctor);

            return new DoctorReadDto
            {
                Id = doctor.Id,
                FullName = doctor.FullName,
                Specialization = doctor.Specialization
            };
        }

        /// <summary>
        /// Retrieves a doctor by ID.
        /// </summary>
        /// <param name="id">The doctor ID.</param>
        /// <returns>The doctor's read DTO, or null if not found.</returns>
        public async Task<DoctorReadDto> GetDoctorByIdAsync(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor == null)
            {
                return null; // Explicitly return null if not found
            }

            return new DoctorReadDto
            {
                Id = doctor.Id,
                FullName = doctor.FullName,
                Specialization = doctor.Specialization
            };
        }

        /// <summary>
        /// Retrieves all doctors.
        /// </summary>
        /// <returns>A list of doctor read DTOs.</returns>
        public async Task<IReadOnlyList<DoctorReadDto>> GetAllDoctorsAsync()
        {
            var doctors = (await _doctorRepository.GetAllAsync()).ToList(); // Ensure List for IReadOnlyList conversion
            return doctors.Select(d => new DoctorReadDto
            {
                Id = d.Id,
                FullName = d.FullName,
                Specialization = d.Specialization
            }).ToList();
        }

        /// <summary>
        /// Analyzes the total number of visits for a specific doctor.
        /// </summary>
        /// <param name="doctorId">The ID of the doctor.</param>
        /// <returns>A DTO containing the doctor's visit summary, or null if doctor not found.</returns>
        public async Task<DoctorVisitsSummaryDto> AnalyzeDoctorVisitsAsync(int doctorId)
        {
            var doctor = await _doctorRepository.GetByIdAsync(doctorId);
            if (doctor == null)
            {
                return null; // Explicitly return null if doctor not found
            }

            var totalVisits = await _visitRepository.CountDoctorVisitsAsync(doctorId);

            return new DoctorVisitsSummaryDto
            {
                DoctorId = doctor.Id,
                DoctorFullName = doctor.FullName,
                DoctorSpecialization = doctor.Specialization,
                TotalVisits = totalVisits
            };
        }
    }
}
