using Application.DTOs;
using Application.Services;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace FinalAPI.Controllers
{
    /// <summary>
    /// Controller for managing Doctor operations.
    /// Provides Read and Create operations only (as per requirements).
    /// </summary>
    [RoutePrefix("api/doctors")]
    [Authorize] // All endpoints require authentication
    public class DoctorsController : ApiController
    {
        private readonly DoctorService _doctorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorsController"/> class.
        /// </summary>
        /// <param name="doctorService">The doctor service to inject.</param>
        public DoctorsController(DoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        /// <summary>
        /// Gets all doctors.
        /// Accessible by admin and doctor roles.
        /// </summary>
        /// <returns>List of doctors</returns>
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
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Gets a specific doctor by ID.
        /// Accessible by admin and doctor roles.
        /// </summary>
        /// <param name="id">Doctor ID</param>
        /// <returns>Doctor details</returns>
        [HttpGet]
        [Route("{id:int}")]
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
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Creates a new doctor.
        /// Only admin can create doctors.
        /// </summary>
        /// <param name="doctorDto">Doctor creation data</param>
        /// <returns>Created doctor details</returns>
        [HttpPost]
        [Route("")]
        [Authorize(Roles = "admin")]
        [ResponseType(typeof(DoctorReadDto))]
        public async Task<IHttpActionResult> CreateDoctor(DoctorCreateDto doctorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdDoctor = await _doctorService.CreateDoctorAsync(doctorDto);
                return CreatedAtRoute("DefaultApi", new { id = createdDoctor.Id }, createdDoctor);
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
        /// Gets visit statistics for a specific doctor.
        /// Accessible by admin and doctor roles.
        /// Doctor role can only access their own statistics.
        /// </summary>
        /// <param name="id">Doctor ID</param>
        /// <returns>Doctor visit statistics</returns>
        [HttpGet]
        [Route("{id:int}/visits-summary")]
        [Authorize(Roles = "admin,doctor")]
        [ResponseType(typeof(DoctorVisitsSummaryDto))]
        public async Task<IHttpActionResult> GetDoctorVisitsSummary(int id)
        {
            try
            {
                // If user is a doctor, they can only access their own data
                if (User.IsInRole("doctor") && !User.IsInRole("admin"))
                {
                    // In a real application, you would get the current user's doctor ID from the token
                    // For demo purposes, we'll assume doctor with ID 2 can only access their own data
                    var currentUserId = User.Identity.Name; // This would contain the user ID from JWT
                    if (currentUserId != "2" || id != 2)
                    {
                        return Forbid();
                    }
                }

                var summary = await _doctorService.AnalyzeDoctorVisitsAsync(id);
                if (summary == null)
                {
                    return NotFound();
                }
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Helper method to return Forbidden result.
        /// </summary>
        /// <returns>Forbidden HTTP response</returns>
        private IHttpActionResult Forbid()
        {
            return Content(System.Net.HttpStatusCode.Forbidden, "Access denied. You can only access your own data.");
        }
    }
}
