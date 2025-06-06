using Application.DTOs;
using Application.Services;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace FinalAPI.Controllers
{
    /// <summary>
    /// Controller for managing Patient operations.
    /// Provides full CRUD operations for patients.
    /// </summary>
    [RoutePrefix("api/patients")]
    [Authorize] // All endpoints require authentication
    public class PatientsController : ApiController
    {
        private readonly PatientService _patientService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientsController"/> class.
        /// </summary>
        /// <param name="patientService">The patient service to inject.</param>
        public PatientsController(PatientService patientService)
        {
            _patientService = patientService;
        }

        /// <summary>
        /// Gets all patients.
        /// Accessible by admin and doctor roles.
        /// </summary>
        /// <returns>List of patients</returns>
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "admin,doctor")]
        [ResponseType(typeof(System.Collections.Generic.IReadOnlyList<PatientReadDto>))]
        public async Task<IHttpActionResult> GetAllPatients()
        {
            try
            {
                var patients = await _patientService.GetAllPatientsAsync();
                return Ok(patients);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Gets a specific patient by ID.
        /// Accessible by admin and doctor roles.
        /// </summary>
        /// <param name="id">Patient ID</param>
        /// <returns>Patient details</returns>
        [HttpGet]
        [Route("{id:int}")]
        [Authorize(Roles = "admin,doctor")]
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
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Creates a new patient.
        /// Only admin can create patients.
        /// </summary>
        /// <param name="patientDto">Patient creation data</param>
        /// <returns>Created patient details</returns>
        [HttpPost]
        [Route("")]
        [Authorize(Roles = "admin")]
        [ResponseType(typeof(PatientReadDto))]
        public async Task<IHttpActionResult> CreatePatient(PatientCreateDto patientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdPatient = await _patientService.CreatePatientAsync(patientDto);
                return CreatedAtRoute("DefaultApi", new { id = createdPatient.Id }, createdPatient);
            }
            catch (AppServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Updates an existing patient.
        /// Only admin can update patients.
        /// </summary>
        /// <param name="id">Patient ID</param>
        /// <param name="patientDto">Updated patient data</param>
        /// <returns>Success or error response</returns>
        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IHttpActionResult> UpdatePatient(int id, PatientUpdateDto patientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != patientDto.Id)
            {
                return BadRequest("ID in URL does not match ID in request body.");
            }

            try
            {
                var success = await _patientService.UpdatePatientAsync(patientDto);
                if (!success)
                {
                    return NotFound();
                }
                return Ok();
            }
            catch (AppServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Deletes a patient.
        /// Only admin can delete patients.
        /// </summary>
        /// <param name="id">Patient ID</param>
        /// <returns>Success or error response</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IHttpActionResult> DeletePatient(int id)
        {
            try
            {
                var success = await _patientService.DeletePatientAsync(id);
                if (!success)
                {
                    return NotFound();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Gets billing summary for a specific patient.
        /// Accessible by admin and doctor roles.
        /// </summary>
        /// <param name="id">Patient ID</param>
        /// <returns>Patient billing summary</returns>
        [HttpGet]
        [Route("{id:int}/billing")]
        [Authorize(Roles = "admin,doctor")]
        [ResponseType(typeof(BillingSummaryDto))]
        public async Task<IHttpActionResult> GetPatientBilling(int id)
        {
            try
            {
                var billing = await _patientService.CalculateTotalBillingForPatientAsync(id);
                if (billing == null)
                {
                    return NotFound();
                }
                return Ok(billing);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}