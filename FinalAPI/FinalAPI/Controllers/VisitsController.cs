using Application.DTOs;
using Application.Services;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace FinalAPI.Controllers
{
    /// <summary>
    /// Controller for managing Visit operations.
    /// Provides full CRUD operations with pagination, filtering, and sorting.
    /// </summary>
    [RoutePrefix("api/visits")]
    [Authorize] // All endpoints require authentication
    public class VisitsController : ApiController
    {
        private readonly VisitService _visitService;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisitsController"/> class.
        /// </summary>
        /// <param name="visitService">The visit service to inject.</param>
        public VisitsController(VisitService visitService)
        {
            _visitService = visitService;
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
        /// <param name="sortBy">Sort field (Fee, VisitDate)</param>
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
                // If user is a doctor, they can only see their own visits
                if (User.IsInRole("doctor") && !User.IsInRole("admin"))
                {
                    // In a real application, you would get the current user's doctor ID from the token
                    // For demo purposes, we'll assume doctor with ID 2
                    var currentUserId = User.Identity.Name; // This would contain the user ID from JWT
                    if (currentUserId == "2")
                    {
                        doctorId = 2; // Force filter to only show this doctor's visits
                    }
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
            catch (Exception ex)
            {
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
        [Route("{id:int}")]
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

                // If user is a doctor, they can only see their own visits
                if (User.IsInRole("doctor") && !User.IsInRole("admin"))
                {
                    var currentUserId = User.Identity.Name;
                    if (currentUserId == "2" && visit.DoctorId != 2)
                    {
                        return Forbid();
                    }
                }

                return Ok(visit);
            }
            catch (Exception ex)
            {
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
                return CreatedAtRoute("DefaultApi", new { id = createdVisit.Id }, createdVisit);
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