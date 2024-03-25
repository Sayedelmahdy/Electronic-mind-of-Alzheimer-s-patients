using API.Helper;
using BLL.DTOs.CaregiverDto;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Roles ="Caregiver")]
    [Route("/[controller]")]
    [ApiController]
    public class CaregiverController : ControllerBase
    {
        public ICaregiverService _caregiverService { get; }
        public CaregiverController(ICaregiverService caregiverService)
        {
            _caregiverService = caregiverService;
        }
        [HttpGet("GetCaregiverCode")]
        public async Task<IActionResult> GetCaregiverCode()
        {
            var token =HttpContextHelper.GetToken(this.HttpContext);
            if(token == null)
            {
                return BadRequest("Invalid Token");
            }
            var code =  await _caregiverService.GetCaregiverCode(token);
            if(code == null)
            {
                return NotFound("Invalid Code");
            }
            return Ok(code);
        }
        [HttpGet("GetAssignedPatients")]
        public async Task<IActionResult> GetAssignedPatients()
        {
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Invalid Token");
            }
            var res = await _caregiverService.GetAssignedPatientsAsync(token);
            if (res == null || !res.Any())
            {
                return NotFound("No Patients Assigned for Caregiver Yet .");
            }
            return Ok(res.ToList());
        }
        [HttpPost("AddMedicationReminder/{patientId}")]
        public async Task<IActionResult> AddMedicationReminder([FromBody] MedicationReminderPostDto medicationReminderDto, string patientId)
        {
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Invalid Token");
            }
            var res = await _caregiverService.AddMedicationReminder(token, patientId, medicationReminderDto);
            if (res.HasError)
            {
                return StatusCode(400, res.message);
            }
            return Ok(res.message);
        }
        [HttpGet("GetMedicationRemindersForPatient/{PatientId}")]
        public async Task<IActionResult> GetMedicationRemindersForPatient(string PatientId)
        {
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Invalid Token");
            }
            var res = await _caregiverService.GetMedicationRemindersAsync(token, PatientId);
            if (res == null || !res.Any())
            {
                return NotFound("No Medication Reminders ");
            }
            return Ok(res.ToList());
        }
        [HttpPut("UpdateMedicationReminderForPatient/{ReminderId}")]
        public async Task<IActionResult> UpdateMedicationReminder(string ReminderId, [FromBody] MedicationReminderUpdateDto medication)
        {
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Invalid Token");
            }
            var res = await _caregiverService.UpdateMedicationReminderAsync(token, ReminderId, medication);
            if (res.HasError)
            {
                return BadRequest(res.message);
            }
            return Ok(res.message);
        }
        [HttpDelete("DeleteMedicationReminder/{ReminderId}")]
        public async Task<IActionResult> DeleteMedicationReminder(string ReminderID)
        {
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Invalid Token");
            }
            var res = await _caregiverService.DeleteMedicationReminderAsync(token, ReminderID);
            if (res.HasError)
            {
                return BadRequest(res.message);
            }
            return Ok(res.message);
        }
    }
}
