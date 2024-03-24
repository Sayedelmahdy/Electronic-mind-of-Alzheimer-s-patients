using API.Helper;
using BLL.DTOs.FamilyDto;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Family")]
    public class FamilyController : ControllerBase
    {
      
        public IFamilyService _familyService { get; }

        public FamilyController
            (
            IFamilyService familyService 
            )
        {
         
            _familyService = familyService;
        }

        [HttpGet("GetPatientCode")]       
        
        public async Task<IActionResult> GetPatientCode()
        {
            string? token = HttpContextHelper.GetToken(this.HttpContext);
            if (token==null)
            {
                return BadRequest("Token invaild");
            }
            var res = await _familyService.GetPatientCode(token);
            if (res==null)
            {
                return BadRequest("You need to add Patient or assign him to you first");
            }
            return Ok(new {Code = res});
        }
        [HttpPut("AssignPatientToFamily")]
        public async Task<IActionResult> AssignPatientToFamily(AssignPatientDto assignPatientDto)
        {
            string? token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Token invaild");
            }

            var res = await _familyService.AssignPatientToFamily(token, assignPatientDto);
            if (res.HasError)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }
        [HttpPut("AssignPatientToCaregiver/{CaregiverCode}")]
        public async Task<IActionResult> AssignPatientToCaregiver (string CaregiverCode)
        {
            string? token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Token invaild");
            }
            var res = await _familyService.AssignPatientToCaregiver(token, CaregiverCode);
            if (res.HasError)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }
        [HttpPost("AddPatient")]
        public async Task<IActionResult> AddPatient ( AddPatientDto addPatientDto)
        {
            string? token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Token invaild");
            }
            var res = await _familyService.AddPatientAsync(token, addPatientDto);
            if (res.HasError)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }
        [HttpGet("GetPatientProfile")]
        public async Task<IActionResult> GetPatientProfile()
        {

            string? token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Token invaild");
            }
            var res = await _familyService.GetPatientProfile(token);
            if (res.ErrorAppear)
            {
                return BadRequest(new {Message = res.Message})  ;
            }
            return Ok(res); 
        }
        [HttpPut("UpdatePatientProfile")]
        public async Task<IActionResult> UpdatePatientProfile ( UpdatePatientProfileDto updatePatientProfileDto)
        {
            string? token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Token invaild");
            }
            var res = await _familyService.UpdatePatientProfileAsync(token,updatePatientProfileDto);
            if (res.ErrorAppear)
            {
                return BadRequest(new { Message = res.Message });
            }
            return Ok(res);
        }
        [HttpGet("GetMediaForFamily")]
        public async Task<IActionResult> GetMediaForFamily ()
        {
            string? token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Token invaild");
            }
            var res = await _familyService.GetMediaForFamilyAsync(token);
            if (res.Count()==0)
            {
                return NotFound("No Media");
            }
            return Ok(res);
        }
        [HttpPost("UploadMedia")]
        public async Task<IActionResult> UploadMedia([FromForm]AddMediaDto addMediaDto)
        {
            string? token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Token invaild");
            }
            var res = await _familyService.UploadMediaAsync(token,addMediaDto);
            if (res.HasError)
            {
                return BadRequest(res.message);
            }
            return Ok(res);
        }
        [HttpPost("AddAppointment")]
        public async Task<IActionResult> AddAppointment (AddAppointmentDto addAppointmentDto)
        {
            string? token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Token invaild");
            }
            var res = await _familyService.AddAppointmentAsync(token,addAppointmentDto);
            if (res.HasError)
                return BadRequest(res);
            return Ok(res);
        }
        [HttpGet("GetPatientAppointments")]
        public async Task<IActionResult> GetPatientAppointments ()
        {
            string? token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Token invaild");
            }
            var res = await _familyService.GetPatientAppointmentsAsync(token);
            if (res.Count() == 0)
                return NotFound("No Appointments");
            return Ok(res);
        }
        [HttpDelete("DeleteAppointment/{AppointmentId}")]
        public async Task<IActionResult> DeleteAppointment(string AppointmentId )
        {
            string? token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Token invaild");
            }
            var res = await _familyService.DeleteAppointmentAsync(token,AppointmentId);
            if (res.HasError)
                return BadRequest(res);
            return Ok(res); 
        }

    }
}
