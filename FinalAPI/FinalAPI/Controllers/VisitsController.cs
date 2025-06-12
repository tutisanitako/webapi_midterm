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
    /// <summary>
    /// Controller for managing Visit operations.
    /// Provides full CRUD operations with pagination, filtering, and sorting.
    /// </summary>
    [RoutePrefix("api/visits")]
    [Authorize]
    public class VisitsController : ApiController
    {
        private readonly VisitService _visitService;
        private readonly DoctorService _doctorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisitsController"/> class.
        /// (Uses Poor Man's DI, instantiating dependencies directly)
        /// </summary>
        public VisitsController()
        {
            var dbContext = new HealthDbContext();

            var visitRepository = new VisitRepository(dbContext);
            var patientRepository = new PatientRepository(dbContext);
            var doctorRepository = new DoctorRepository(dbContext);

            _visitService = new VisitService(visitRepository, patientRepository, doctorRepository);
            _doctorService = new DoctorService(doctorRepository, visitRepository);
        }

        /// <summary>
        /// Gets visits with pagination, filtering, and sorting.
        /// Admin can see all visits, doctor can only see their own visits.
        /// </summary>
        /// <param name="doctorId">Filter by doctor ID</param>
        /// <param name="visitDateFrom">Filter by start date</param>
        /// <param name="visitDateTo">Filter by end date</param>
        /// <param name="minFee">Minimum fee filter</param>
        /// <param name="maxFee">Maximum fee filter</param>
        /// <param name="sortBy">Sort field (Fee, VisitDate, PatientFullName, DoctorFullName)</param>
        /// <param name="sortDirection">Sort direction (asc, desc)</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of visits</returns>
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "admin,doctor")]
        [ResponseType(typeof(PagedResult<VisitReadDto>))]
        public async Task<IHttpActionResult> GetVisits(
            int? doctorId = null,
            DateTime? visitDateFrom = null,
            DateTime? visitDateTo = null,
            decimal? minFee = null,
            decimal? maxFee = null,
            string sortBy = "VisitDate",
            string sortDirection = "asc",
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                // 1. Input Validation for Pagination and Sorting Parameters
                if (pageNumber < 1)
                {
                    return BadRequest("Page number must be 1 or greater.");
                }
                if (pageSize < 1)
                {
                    return BadRequest("Page size must be 1 or greater.");
                }

                var validSortFields = new[] { "Fee", "VisitDate", "PatientFullName", "DoctorFullName" };
                if (!string.IsNullOrEmpty(sortBy) && !validSortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
                {
                    return BadRequest($"Invalid sortBy field. Allowed values are: {string.Join(", ", validSortFields)}");
                }
                var validSortDirections = new[] { "asc", "desc" };
                if (!string.IsNullOrEmpty(sortDirection) && !validSortDirections.Contains(sortDirection, StringComparer.OrdinalIgnoreCase))
                {
                    return BadRequest("Invalid sortDirection. Allowed values are: 'asc' or 'desc'.");
                }


                // 2. Authorization Logic: Doctor can only see their own visits
                if (User.IsInRole("doctor") && !User.IsInRole("admin"))
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

                    if (doctorId.HasValue && doctorId.Value != loggedInDoctorId)
                    {
                        return Forbid();
                    }
                    doctorId = loggedInDoctorId;
                }

                var queryParameters = new VisitQueryParameters
                {
                    DoctorId = doctorId,
                    VisitDateFrom = visitDateFrom,
                    VisitDateTo = visitDateTo,
                    MinFee = minFee,
                    MaxFee = maxFee,
                    SortBy = sortBy,
                    SortDirection = sortDirection,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var visits = await _visitService.GetVisitsAsync(queryParameters);
                return Ok(visits);
            }
            catch (AppServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetVisits: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Gets a specific visit by ID.
        /// Admin can see all visits, doctor can only see their own visits.
        /// </summary>
        /// <param name="id">Visit ID</param>
        /// <returns>Visit details</returns>
        [HttpGet]
        [Route("{id:int}", Name = "GetVisit")]
        [Authorize(Roles = "admin,doctor")]
        [ResponseType(typeof(VisitReadDto))]
        public async Task<IHttpActionResult> GetVisit(int id)
        {
            try
            {
                var visit = await _visitService.GetVisitByIdAsync(id);
                if (visit == null)
                {
                    return NotFound();
                }

                if (User.IsInRole("doctor") && !User.IsInRole("admin"))
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

                    if (visit.DoctorId != loggedInDoctorId)
                    {
                        return Forbid(); 
                    }
                }

                return Ok(visit);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetVisit: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Creates a new visit.
        /// Only admin can create visits.
        /// </summary>
        /// <param name="visitDto">Visit creation data</param>
        /// <returns>Created visit details</returns>
        [HttpPost]
        [Route("")]
        [Authorize(Roles = "admin")]
        [ResponseType(typeof(VisitReadDto))]
        public async Task<IHttpActionResult> CreateVisit(VisitCreateDto visitDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdVisit = await _visitService.CreateVisitAsync(visitDto);
                return CreatedAtRoute("GetVisit", new { id = createdVisit.Id }, createdVisit);
            }
            catch (AppServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateVisit: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Updates an existing visit.
        /// Only admin can update visits.
        /// </summary>
        /// <param name="id">Visit ID</param>
        /// <param name="visitDto">Updated visit data</param>
        /// <returns>Success or error response</returns>
        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IHttpActionResult> UpdateVisit(int id, VisitUpdateDto visitDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != visitDto.Id)
            {
                return BadRequest("ID in URL does not match ID in request body.");
            }

            try
            {
                var success = await _visitService.UpdateVisitAsync(visitDto);
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
                Console.WriteLine($"Error in UpdateVisit: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Deletes a visit.
        /// Only admin can delete visits.
        /// </summary>
        /// <param name="id">Visit ID</param>
        /// <returns>Success or error response</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IHttpActionResult> DeleteVisit(int id)
        {
            try
            {
                var success = await _visitService.DeleteVisitAsync(id);
                if (!success)
                {
                    return NotFound(); 
                }
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteVisit: {ex.Message}\nStackTrace: {ex.StackTrace}");
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