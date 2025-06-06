using Application.DTOs;
using Application.Services;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace FinalAPI.Controllers
{
    [RoutePrefix("api/patients")]
    [Authorize]
    public class PatientsController : ApiController
    {
        private readonly PatientService _patientService;

        public PatientsController(PatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = "admin")]
        [ResponseType(typeof(System.Collections.Generic.IReadOnlyList<PatientReadDto>))]
        public async Task<IHttpActionResult> GetAll()
        {
            var patients = await _patientService.GetAllAsync();
            return Ok(patients);
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetPatient")]
        [Authorize(Roles = "admin")]
        [ResponseType(typeof(PatientReadDto))]
        public async Task<IHttpActionResult> GetPatient(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return Ok(patient);
        }

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
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IHttpActionResult> UpdatePatient(int id, [FromBody] PatientCreateDto patientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _patientService.UpdatePatientAsync(id, patientDto);
                return Ok();
            }
            catch (AppServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

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
        }
    }
}