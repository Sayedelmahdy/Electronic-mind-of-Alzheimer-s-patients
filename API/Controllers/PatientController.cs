using BLL.DTOs;
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

        [HttpGet("GetPatientFamilies")]
        public async Task<IActionResult> GetPatientFamilies()
        {
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Invalid Token");
            }
            var res = await _patientService.GetFamiliesAsync(token);
            if (!res.Any() || res == null)
            {
                return NotFound("Have No Families :)");
            }
            return Ok(res.ToList());
        }

        [HttpPut("UpdatePatientProfile")]
        public async Task<IActionResult> UpdatePatientProfile([FromBody] UpdatePatientProfileDto updatePatientProfileDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
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

        [HttpGet("GetFamilyLocation/{familyId}")]
        public async Task<IActionResult> GetFamilyLocation(string familyId)
        {
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Invalid Token");
            }

            var res = await _patientService.GetFamilyLocation(token, familyId);
            if (res.Code != 200)
            {
                return BadRequest(res.Message);
            }
            return Ok(res);
        }

        [HttpPost("MarkMedicationReminder")]
        public async Task<IActionResult> MarkMedicationReminder([FromBody] MarkMedictaionDto medicationReminderDto)
        {
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Invalid Token");
            }

            var res = await _patientService.MarkMedicationReminderAsync(token, medicationReminderDto);
            if (res.HasError)
            {
                return BadRequest(res.message);
            }
            return Ok(res.message);
        }

        [HttpPost("AddSecretFile")]
        public async Task<IActionResult> AddSecretFile([FromForm] PostSecretFileDto addSecretFileDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Invalid Token");
            }
            var res = await _patientService.AddSecretFileAsync(token, addSecretFileDto);
            if (res.HasError)
            {
                return BadRequest(res.message);
            }
            return Ok(res.message);
        }
        [HttpPost("AskToSeeSecretFile")]
        public async Task<IActionResult> AskToSeeSecretFile([FromForm] AskToViewDto askToViewDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Invalid Token");
            }
            var res = await _patientService.AskToViewSecretFileAsync(token, askToViewDto.Video);
            if (res.HasError)
            {
                return BadRequest(res.message);
            }
            return Ok(res.message);
        }

        [HttpGet("GetSecretFile")]
        public async Task<IActionResult> GetSecretFile()
        {
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Invalid Token");
            }
            var res = await _patientService.GetSecretFilesAsync(token);
            if (res == null)
            {
                return NotFound("No Secret Files Found");
            }
            return StatusCode(res.Code,res);
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
        [HttpGet("GetMedia")]
        public async Task<IActionResult> GetMedia()
        {
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Invalid Token");
            }
            var res = await _patientService.GetMediaAsync(token);
            if(!res.Any() || res == null)
            {
                return NotFound("No Media Yet .. ");
            }
            return Ok(res.ToList());
        }
        [HttpGet("GetAllGameScores")]
        public async Task<IActionResult> GetAllGameScores()
        {
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Invalid Token");
            }
            var res = await _patientService.GetGameScoresAsync(token);
            if( res == null || res.GameScore == null || !res.GameScore.Any())
            {
                return NotFound("No Game Scores Yet .. ");
            }
            return Ok(res);
        }
        [HttpPost("AddGameScore")]
        public async Task<IActionResult> AddGameScore([FromBody] PostGameScoreDto addGameScoreDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Invalid Token");
            }
            var res = await _patientService.AddGameScoreAsync(token, addGameScoreDto);
            if (res.HasError)
            {
                return BadRequest(res.message);
            }
            return Ok(res);
        }

    }
}
