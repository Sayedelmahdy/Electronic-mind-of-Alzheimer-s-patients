using BLL.Helper;
using BLL.DTOs;
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
        /// <summary>
        /// Controller endpoint to retrieve the caregiver code associated with the provided token.
        /// </summary>
        /// <remarks>
        /// This endpoint allows clients to obtain the caregiver code using a valid authentication token. 
        /// The caregiver code is essential for identifying and authenticating caregivers within the system.
        /// </remarks>
        /// <returns>
        /// Returns the caregiver code if the token is valid and associated with a caregiver. 
        /// Returns BadRequest if the token is invalid. Returns NotFound if no caregiver code is found for the provided token.
        /// </returns>
        [HttpGet("GetCaregiverCode")]
        public async Task<IActionResult> GetCaregiverCode()
        {
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Invalid Token");
            }
            var code = await _caregiverService.GetCaregiverCode(token);
            if (code == null)
            {
                return NotFound("Invalid Code");
            }
            return Ok(code);
        }
        /// <summary>
        /// Controller endpoint to retrieve the list of patients assigned to the caregiver associated with the provided token.
        /// </summary>
        /// <remarks>
        /// This endpoint allows clients to obtain the list of patients assigned to a caregiver using a valid authentication token. 
        /// The endpoint checks the authorization token to ensure that the request is made by an authenticated caregiver.
        /// </remarks>
        /// <returns>
        /// Returns the list of patients assigned to the caregiver if the token is valid and associated with a caregiver and if patients are assigned.
        /// Returns BadRequest if the token is invalid. Returns NotFound if no patients are assigned to the caregiver.
        /// </returns>
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
        /// <summary>
        /// Controller endpoint to add a medication reminder for a specific patient.
        /// </summary>
        /// <param name="patientId">The ID of the patient for whom the medication reminder is being added.</param>
        /// <param name="medicationReminderDto">The data transfer object containing information about the medication reminder.</param>
        /// <remarks>
        /// This endpoint allows caregivers to add medication reminders for their assigned patients. 
        /// Caregivers must provide a valid authentication token to access this endpoint.
        /// The endpoint verifies the caregiver's authorization and the patient's assignment before adding the medication reminder.
        /// </remarks>
        /// <returns>
        /// Returns Ok if the medication reminder is added successfully. 
        /// Returns BadRequest if the token is invalid or if there is an error in the request body.
        /// Returns StatusCode 400 with an error message if there is an error during the addition of the medication reminder.
        /// </returns>
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
        /// <summary>
        /// Controller endpoint to retrieve medication reminders for a specific patient.
        /// </summary>
        /// <param name="PatientId">The ID of the patient for whom medication reminders are being retrieved.</param>
        /// <remarks>
        /// This endpoint allows caregivers to retrieve medication reminders for a specific patient assigned to them. 
        /// Caregivers must provide a valid authentication token to access this endpoint.
        /// The endpoint verifies the caregiver's authorization and the patient's assignment before retrieving the medication reminders.
        /// </remarks>
        /// <returns>
        /// Returns Ok with the list of medication reminders if any are found for the specified patient.
        /// Returns BadRequest if the token is invalid.
        /// Returns NotFound if no medication reminders are found for the specified patient.
        /// </returns>
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
        /// <summary>
        /// Controller endpoint to update a medication reminder for a specific patient.
        /// </summary>
        /// <param name="ReminderId">The ID of the medication reminder to be updated.</param>
        /// <param name="medication">The data transfer object containing updated information about the medication reminder.</param>
        /// <remarks>
        /// This endpoint allows caregivers to update medication reminders for their assigned patients. 
        /// Caregivers must provide a valid authentication token to access this endpoint.
        /// The endpoint verifies the caregiver's authorization and the existence of the medication reminder before updating it.
        /// </remarks>
        /// <returns>
        /// Returns Ok if the medication reminder is updated successfully. 
        /// Returns BadRequest if the token is invalid or if there is an error in the request body.
        /// </returns>
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
        /// <summary>
        /// Controller endpoint to delete a medication reminder.
        /// </summary>
        /// <param name="ReminderID">The ID of the medication reminder to be deleted.</param>
        /// <remarks>
        /// This endpoint allows caregivers to delete medication reminders for their assigned patients. 
        /// Caregivers must provide a valid authentication token to access this endpoint.
        /// The endpoint verifies the caregiver's authorization and the existence of the medication reminder before deleting it.
        /// </remarks>
        /// <returns>
        /// Returns Ok if the medication reminder is deleted successfully. 
        /// Returns BadRequest if the token is invalid or if there is an error in the request body.
        /// </returns>
        [HttpDelete("DeleteMedicationReminder/{ReminderID}")]
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
        /// <summary>
        /// Retrieves the game scores for a specific patient.
        /// </summary>
        /// <param name="PatientId">The unique identifier of the patient.</param>
        /// <returns>
        /// Returns a list of game scores associated with the specified patient. If no game scores are found for the patient, returns a 404 Not Found response.
        /// </returns>
        /// <remarks>
        /// This endpoint fetches the game scores for a particular patient based on their unique identifier. If the patient does not exist or has no associated game scores, an empty list is returned.
        /// </remarks>
        [HttpGet("GetGameScoreforPatinet/{PatientId}")]
        public async Task<IActionResult> GetGameScoreForPatient(string PatientId)
        {
            var res = await _caregiverService.GetGameScoresAsync(PatientId);
            if (!res.Any())
            {
                return NotFound("No Game Score Found for this Patient ");
            }
            return Ok(res.ToList());
        }
        /// <summary>
        /// Retrieves all reports associated with a specific patient.
        /// </summary>
        /// <param name="PatientId">The unique identifier of the patient.</param>
        /// <returns>
        /// Returns a list of reports related to the specified patient. If no reports are found for the patient, returns a 404 Not Found response.
        /// </returns>
        /// <remarks>
        /// This endpoint fetches all reports for a particular patient based on their unique identifier. The request requires a valid authentication token. If the token is invalid, a 400 Bad Request response is returned. If the caregiver associated with the token does not have permission to access the patient's reports, an empty list is returned.
        /// </remarks>
        [HttpGet("GetAllReportsForPatinet/{PatientId}")]
        public async Task<IActionResult> GetAllReportsForPatient(string PatientId)
        {
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Invalid Token");
            }
            var res = await _caregiverService.getallReport(token, PatientId);
            if (!res.Any())
            {
                return NotFound("No Report for Patient ");
            }
            return Ok(res.ToList());
        }
        /// <summary>
        /// Creates a new report card.
        /// </summary>
        /// <param name="reportCardDto">The data required to create the report card.</param>
        /// <returns>
        /// Returns a success message if the report card is created successfully. If the token is invalid, a 400 Bad Request response is returned. If there is any error during report card creation, a 400 Bad Request response with an error message is returned.
        /// </returns>
        /// <remarks>
        /// This endpoint allows caregivers to create a new report card for a patient. The request must include valid authentication token for authorization. If the token is missing or invalid, a 400 Bad Request response is returned. The caregiver associated with the token must have permission to create reports for the specified patient. If the patient does not exist or is not associated with the caregiver, a 400 Bad Request response with an error message is returned.
        /// </remarks>
        [HttpPost("CreateReport")]
        public async Task<IActionResult> CreateReport([FromBody] ReportCardDto reportCardDto)
        {
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Invalid Token");
            }
            var res = await _caregiverService.CreateReportCardAsync(token, reportCardDto);
            if(res.HasError)
            {
                return BadRequest(res.message);
            }
            return Ok(res.message);
        }
        [HttpDelete("DeleteReport/{ReportId}")]
        public async Task<IActionResult> DeleteReport(string ReportId)
        {
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Invalid Token");
            }
            var res = await _caregiverService.DeleteReport(token, ReportId);
            if (res.HasError)
            {
                return BadRequest(res.message);
            }
            return Ok(res.message);
        }
    }
}
