using BLL.DTOs.FamilyDto;
using BLL.Helper;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    [Authorize(Roles ="Patient")]
    public class PatientController : ControllerBase
    {
        public IPatientService _patientService { get;  }
        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }
        [HttpGet("GetPatientProfile")]
        public async Task<IActionResult> GetPatientProfile()
        {
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Invalid Token");
            }
            var res = await _patientService.GetPatientProfileAsync(token);
            if(res.HasError)
            {
                return BadRequest(res.Message);
            }
            return Ok(res);
        }
        [HttpPut("UpdatePatientProfile")]
        public async Task<IActionResult> UpdatePatientProfile([FromBody] UpdatePatientProfileDto updatePatientProfileDto)
        {
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Invalid Token");
            }
            var res = await _patientService.UpdateProfileAsync(token, updatePatientProfileDto);
            if (res.HasError)
            {
                return BadRequest(res.message);
            }
            return Ok(res);
        }
        [HttpGet("GetAllAppointments")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Invalid Token");
            }
            var res = await _patientService.GetAppointmentAsync(token);
            if(!res.Any() || res == null)
            {
                return NotFound("Have No Appointments :)");
            }
            return Ok(res.ToList());
        }
        [HttpGet("GetAllMedicines")]
        public async Task<IActionResult> GetAllMedicines()
        {
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Invalid Token");
            }
            var res = await _patientService.GetMedicationRemindersAsync(token);
            if(!res.Any() || res == null)
            {
                return NotFound("No Medicines Yet .. ");
            }
            return Ok(res.ToList());
        }
    }
}
