using Application.DTOs;
using Application.Services;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace FinalAPI.Controllers
{
    [RoutePrefix("api/doctors")]
    [Authorize]
    public class DoctorsController : ApiController
    {
        private readonly DoctorService _doctorService;

        public DoctorsController(DoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = "admin,doctor")]
        [ResponseType(typeof(System.Collections.Generic.IReadOnlyList<DoctorReadDto>))]
        public async Task<IHttpActionResult> GetAllDoctors()
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            return Ok(doctors);
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetDoctor")]
        [Authorize(Roles = "admin,doctor")]
        [ResponseType(typeof(DoctorReadDto))]
        public async Task<IHttpActionResult> GetDoctor(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            return Ok(doctor);
        }

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
        }

        [HttpGet]
        [Route("{id:int}/visits-summary")]
        [Authorize(Roles = "admin,doctor")]
        [ResponseType(typeof(DoctorVisitsSummaryDto))]
        public async Task<IHttpActionResult> GetDoctorVisitsSummary(int id)
        {
            var userId = User.Identity.Name;
            var isAdmin = User.IsInRole("admin");

            if (!isAdmin && id.ToString() != userId)
            {
                return Content(System.Net.HttpStatusCode.Forbidden, "Access denied. You can only access your own data.");
            }

            var summary = await _doctorService.AnalyzeDoctorVisitsAsync(id);
            if (summary == null)
            {
                return NotFound();
            }
            return Ok(summary);
        }
    }
}