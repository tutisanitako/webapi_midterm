using Application.DTOs;
using Application.Services;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Infrastructure.Repositories;
using Infrastructure.Data;
using System.Data.Entity;
using Domain.Entities;
using Domain.Interfaces;

namespace FinalAPI.Controllers
{
    [RoutePrefix("api/patients")]
    [Authorize]
    public class PatientsController : ApiController
    {
        private readonly PatientService _patientService;

        private readonly IVisitRepository _visitRepository;

        public PatientsController()
        {
            var dbContext = new HealthDbContext();

            var patientRepository = new PatientRepository(dbContext);
            _visitRepository = new VisitRepository(dbContext); 
            var doctorRepository = new DoctorRepository(dbContext); 

            _patientService = new PatientService(patientRepository, _visitRepository);
        }

        /// <summary>
        /// Retrieves all patients.
        /// Only accessible by admin.
        /// </summary>
        /// <returns>A list of patient read DTOs.</returns>
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "admin")]
        [ResponseType(typeof(System.Collections.Generic.IReadOnlyList<PatientReadDto>))]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var patients = await _patientService.GetAllAsync();
                return Ok(patients);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAll Patients: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Retrieves a patient by ID.
        /// Only accessible by admin.
        /// </summary>
        /// <param name="id">The patient ID.</param>
        /// <returns>The patient's read DTO, or not found.</returns>
        [HttpGet]
        [Route("{id:int}", Name = "GetPatient")]
        [Authorize(Roles = "admin")]
        [ResponseType(typeof(PatientReadDto))]
        public async Task<IHttpActionResult> GetPatient(int id)
        {
            try
            {
                var patient = await _patientService.GetPatientByIdAsync(id);
                if (patient == null)
                {
                    return NotFound();
                }
                return Ok(patient);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPatient: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Creates a new patient.
        /// Only accessible by admin.
        /// </summary>
        /// <param name="patientDto">The DTO containing patient creation data.</param>
        /// <returns>The created patient's read DTO.</returns>
        [HttpPost]
        [Route("")]
        [Authorize(Roles = "admin")]
        [ResponseType(typeof(PatientReadDto))]
        public async Task<IHttpActionResult> CreatePatient([FromBody] PatientCreateDto patientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdPatient = await _patientService.CreatePatientAsync(patientDto);
                return CreatedAtRoute("GetPatient", new { id = createdPatient.Id }, createdPatient);
            }
            catch (AppServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreatePatient: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Updates an existing patient.
        /// Only accessible by admin.
        /// </summary>
        /// <param name="id">The ID of the patient to update.</param>
        /// <param name="patientDto">The DTO containing updated patient data.</param>
        /// <returns>Success or error response.</returns>
        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IHttpActionResult> UpdatePatient(int id, [FromBody] PatientUpdateDto patientDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != patientDto.Id)
                return BadRequest("ID in URL does not match ID in request body.");

            try
            {
                await _patientService.UpdatePatientAsync(id, patientDto);
                return Ok();
            }
            catch (AppServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdatePatient: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return InternalServerError(ex);
            }
        }


        /// <summary>
        /// Deletes a patient by ID.
        /// Only accessible by admin.
        /// </summary>
        /// <param name="id">The ID of the patient to delete.</param>
        /// <returns>Success or error response.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IHttpActionResult> DeletePatient(int id)
        {
            try
            {
                await _patientService.DeletePatientAsync(id);
                return Ok();
            }
            catch (AppServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeletePatient: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Calculates and retrieves the total billing summary for a specific patient.
        /// Only accessible by admin.
        /// </summary>
        /// <param name="id">The ID of the patient.</param>
        /// <returns>A DTO containing the patient's billing summary, or not found.</returns>
        [HttpGet]
        [Route("{id:int}/billing-summary")] // NEW ROUTE
        [Authorize(Roles = "admin")] // This data is sensitive, typically admin-only
        [ResponseType(typeof(BillingSummaryDto))]
        public async Task<IHttpActionResult> GetPatientBillingSummary(int id)
        {
            try
            {
                var billingSummary = await _patientService.CalculateTotalBillingForPatientAsync(id);
                if (billingSummary == null)
                {
                    return NotFound();
                }
                return Ok(billingSummary);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPatientBillingSummary: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return InternalServerError(ex);
            }
        }
    }
}