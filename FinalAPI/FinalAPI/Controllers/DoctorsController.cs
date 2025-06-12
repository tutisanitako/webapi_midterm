using Application.DTOs;
using Application.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Infrastructure.Repositories;
using Infrastructure.Data;
using System.Data.Entity;
using Domain.Entities;
using Domain.Interfaces;
using System.Security.Claims;

namespace FinalAPI.Controllers
{
    [RoutePrefix("api/doctors")]
    [Authorize]
    public class DoctorsController : ApiController
    {
        private readonly DoctorService _doctorService;
        private readonly VisitService _visitService; 
        public DoctorsController()
        {
            var dbContext = new HealthDbContext();

            var doctorRepository = new DoctorRepository(dbContext);
            var visitRepository = new VisitRepository(dbContext);
            var patientRepository = new PatientRepository(dbContext);

            _doctorService = new DoctorService(doctorRepository, visitRepository);
            _visitService = new VisitService(visitRepository, patientRepository, doctorRepository);
        }

        /// <summary>
        /// Retrieves all doctors.
        /// Accessible by admin and doctors (doctors can view all profiles).
        /// </summary>
        /// <returns>A list of doctor read DTOs.</returns>
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "admin,doctor")]
        [ResponseType(typeof(System.Collections.Generic.IReadOnlyList<DoctorReadDto>))]
        public async Task<IHttpActionResult> GetAllDoctors()
        {
            try
            {
                var doctors = await _doctorService.GetAllDoctorsAsync();
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllDoctors: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Retrieves a doctor by ID.
        /// Accessible by admin and doctors (doctors can view any profile by ID).
        /// </summary>
        /// <param name="id">The doctor ID.</param>
        /// <returns>The doctor's read DTO, or not found.</returns>
        [HttpGet]
        [Route("{id:int}", Name = "GetDoctor")]
        [Authorize(Roles = "admin,doctor")]
        [ResponseType(typeof(DoctorReadDto))]
        public async Task<IHttpActionResult> GetDoctor(int id)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(id);
                if (doctor == null)
                {
                    return NotFound();
                }
                return Ok(doctor);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetDoctor: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Creates a new doctor.
        /// Only accessible by admin.
        /// </summary>
        /// <param name="doctorDto">The DTO containing doctor creation data.</param>
        /// <returns>The created doctor's read DTO.</returns>
        [HttpPost]
        [Route("")]
        [Authorize(Roles = "admin")]
        [ResponseType(typeof(DoctorReadDto))]
        public async Task<IHttpActionResult> CreateDoctor([FromBody] DoctorCreateDto doctorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdDoctor = await _doctorService.CreateDoctorAsync(doctorDto);
                return CreatedAtRoute("GetDoctor", new { id = createdDoctor.Id }, createdDoctor);
            }
            catch (AppServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateDoctor: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Analyzes the number of visits for a specific doctor.
        /// Admin can see any doctor's summary; a doctor can only see their own summary.
        /// </summary>
        /// <param name="id">The ID of the doctor to analyze.</param>
        /// <returns>A DTO containing the doctor's visit summary, or not found/forbidden.</returns>
        [HttpGet]
        [Route("{id:int}/visits-summary")]
        [Authorize(Roles = "admin,doctor")]
        [ResponseType(typeof(DoctorVisitsSummaryDto))]
        public async Task<IHttpActionResult> GetDoctorVisitsSummary(int id)
        {
            var isAdmin = User.IsInRole("admin");

            if (!isAdmin)
            {
                var claimsPrincipal = User as ClaimsPrincipal;
                if (claimsPrincipal == null)
                {
                    return Content(System.Net.HttpStatusCode.Forbidden, "Invalid user principal format. Access denied.");
                }
                var doctorIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "DoctorId")?.Value;

                if (string.IsNullOrEmpty(doctorIdClaim) || !int.TryParse(doctorIdClaim, out int loggedInDoctorId))
                {
                    return Content(System.Net.HttpStatusCode.Forbidden, "Doctor ID not found in token claims. Access denied.");
                }

                if (id != loggedInDoctorId)
                {
                    return Content(System.Net.HttpStatusCode.Forbidden, "Access denied. You can only access your own data.");
                }
            }

            try
            {
                var summary = await _doctorService.AnalyzeDoctorVisitsAsync(id);
                if (summary == null)
                {
                    return NotFound();
                }
                return Ok(summary);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetDoctorVisitsSummary: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return InternalServerError(ex);
            }
        }
    }
}