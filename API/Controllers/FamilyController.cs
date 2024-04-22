using BLL.Helper;
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
        /// <summary>
        /// Retrieves the patient code associated with the provided token.
        /// </summary>
        /// <returns>
        /// If successful, returns the patient code. Otherwise, returns a BadRequest response with an error message.
        /// </returns>
        [HttpGet("GetPatientCode")]

        public async Task<IActionResult> GetPatientCode()
        {
            string? token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Token invaild");
            }
            var res = await _familyService.GetPatientCode(token);
            if (res == null)
            {
                return BadRequest("You need to add Patient or assign him to you first");
            }
            return Ok(new { Code = res });
        }
        /// <summary>
        /// Retrieves the locations of patients for the current day.
        /// </summary>
        /// <param name="">The HTTP context.</param>
        /// <remarks>
        /// This endpoint fetches the locations of patients for the current day based on the provided token. It checks for the validity of the token and retrieves the family ID associated with it. Then, it fetches the family details and uses the patient ID to query the locations for the current day from the database. The retrieved locations are returned as a collection of LocationDto objects containing latitude and longitude coordinates.
        /// </remarks>
        /// <returns>
        /// If successful, returns the locations of patients for the current day. If the token is invalid or no patient locations are found for the day, returns a BadRequest response.
        /// </returns>
        [HttpGet("GetPatinetLocationsToday")]
        public async Task<IActionResult> GetPatinetLocationsToday()
        {
            string? token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Token invaild");
            }
            var res = await _familyService.GetPatientLocationsTodayAsync(token);
            if (res == null)
            {
                return BadRequest("No Patient Locations Today");
            }
            return Ok(res);

        }
        /// <summary>
        /// Assigns a patient to a family.
        /// </summary>
        /// <param name="assignPatientDto">The data transfer object containing the patient code and relationship details.</param>
        /// <remarks>
        /// This endpoint allows assigning a patient to a family. It first verifies the validity of the provided token. Then, it retrieves the family ID associated with the token. If the family ID is found and the family doesn't already have a patient assigned to it, the endpoint proceeds to validate the patient code provided in the DTO. If the patient code is valid, the patient is assigned to the family along with the specified relationship details. After successful assignment, the endpoint may perform additional tasks, such as sending an image to AI for recognition. 
        /// </remarks>
        /// <returns>
        /// If successful, returns a message indicating successful assignment. If the token is invalid, the family ID is invalid, the family already has a patient assigned, or the provided patient code is invalid, returns a BadRequest response.
        /// </returns>

        [HttpPut("AssignPatientToFamily")]
        public async Task<IActionResult> AssignPatientToFamily(AssignPatientDto assignPatientDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
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
        /// <summary>
        /// Assigns a patient to a caregiver.
        /// </summary>
        /// <param name="CaregiverCode">The unique code identifying the caregiver.</param>
        /// <remarks>
        /// This endpoint facilitates the assignment of a patient to a caregiver. It begins by validating the provided token to ensure its authenticity. Upon successful validation, the family ID associated with the token is retrieved. If no family ID is found or the family doesn't have a patient yet, the endpoint returns a BadRequest response indicating the absence of a patient. Subsequently, the caregiver code provided in the request is verified to ensure its validity. If the caregiver code is invalid, the endpoint returns a BadRequest response indicating an invalid caregiver code. Once all validations pass, the patient associated with the family is updated with the caregiver's ID, signifying successful assignment. 
        /// </remarks>
        /// <returns>
        /// If successful, returns a message confirming the assignment of the patient to the caregiver. If the token is invalid, the family doesn't have a patient yet, or the provided caregiver code is invalid, returns a BadRequest response.
        /// </returns>

        [HttpPut("AssignPatientToCaregiver/{CaregiverCode}")]
        public async Task<IActionResult> AssignPatientToCaregiver(string CaregiverCode)
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
        /// <summary>
        /// Adds a new patient to the system.
        /// </summary>
        /// <param name="addPatientDto">The data required to add a new patient.</param>
        /// <remarks>
        /// This endpoint enables the addition of a new patient to the system. The request must include essential information such as the patient's full name, email, password, phone number, age, diagnosis date, geographical coordinates, avatar image, and relationality. Upon receiving the request, the controller verifies the validity of the provided token to ensure authentication. If the token is invalid, a BadRequest response is returned.
        /// If the token is valid, the controller proceeds to create a new patient entity using the provided data. It generates a username based on the email address, assigns the patient to the "patient" role, and sends a confirmation email for email verification.
        /// After successfully creating the patient entity, the controller updates the family entity associated with the user, assigning the patient to the family and specifying the relationship. Additionally, the patient's avatar image is stored in the system, and an email confirmation link is sent for account verification.
        /// If any step in the process fails, such as creating the user or sending the confirmation email, the controller rolls back the changes and returns a BadRequest response with an appropriate error message.
        /// </remarks>
        /// <returns>
        /// If the patient is added successfully, returns a message confirming the addition. If the token is invalid or any step fails during the process, returns a BadRequest response with an appropriate error message.
        /// </returns>

        [HttpPost("AddPatient")]
        public async Task<IActionResult> AddPatient([FromForm] AddPatientDto addPatientDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
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
        /// <summary>
        /// Retrieves the profile of the patient associated with the authenticated family.
        /// </summary>
        /// <remarks>
        /// This endpoint allows authenticated families to retrieve the profile information of the patient they have created. The controller verifies the validity of the provided token to ensure authentication. If the token is invalid, a BadRequest response is returned.
        /// Upon successful validation of the token, the controller proceeds to fetch the patient's profile information. If the patient is associated with the authenticated family, their profile information, including age, phone number, diagnosis date, and maximum distance, is returned in the response.
        /// If the patient is not associated with the authenticated family, indicating that the family did not create the patient, an error message is returned in the response, specifying that the user cannot update the patient's profile.
        /// </remarks>
        /// <returns>
        /// If the profile is successfully retrieved, returns the patient's profile information in the response body. If the token is invalid or the patient is not associated with the authenticated family, returns a BadRequest response with an appropriate error message.
        /// </returns>

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
                return BadRequest(new { Message = res.Message });
            }
            return Ok(res);
        }
        /// <summary>
        /// Updates the profile of the patient associated with the authenticated family.
        /// </summary>
        /// <param name="updatePatientProfileDto">The data required to update the patient's profile.</param>
        /// <remarks>
        /// This endpoint allows authenticated families to update the profile information of the patient they have created. The controller verifies the validity of the provided token to ensure authentication. If the token is invalid, a BadRequest response is returned.
        /// Upon successful validation of the token, the controller proceeds to update the patient's profile information based on the data provided in the request body. If the patient is associated with the authenticated family, their profile information, including age, phone number, diagnosis date, and maximum distance, is updated accordingly.
        /// If the patient is not associated with the authenticated family, indicating that the family did not create the patient, an error message is returned in the response, specifying that the user cannot update the patient's profile.
        /// </remarks>
        /// <returns>
        /// If the profile is successfully updated, returns the updated patient's profile information in the response body. If the token is invalid or the patient is not associated with the authenticated family, returns a BadRequest response with an appropriate error message.
        /// </returns>
        [HttpPut("UpdatePatientProfile")]
        public async Task<IActionResult> UpdatePatientProfile(UpdatePatientProfileDto updatePatientProfileDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            string? token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Token invaild");
            }
            var res = await _familyService.UpdatePatientProfileAsync(token, updatePatientProfileDto);
            if (res.ErrorAppear)
            {
                return BadRequest(new { Message = res.Message });
            }
            return Ok(res);
        }
        /// <summary>
        /// Retrieves media items associated with the authenticated family.
        /// </summary>
        /// <param name="token">The authentication token obtained from the request.</param>
        /// <remarks>
        /// This endpoint allows authenticated families to retrieve media items associated with their account. The controller verifies the validity of the provided token to ensure authentication. If the token is invalid, an empty list of media items is returned.
        /// Upon successful validation of the token, the controller proceeds to fetch media items belonging to the authenticated family based on their FamilyId. It retrieves media items from the database and maps them to DTOs for presentation.
        /// The DTO includes details such as the media ID, upload date, caption, media URL, and file extension for each media item. This information is used to populate the response body.
        /// </remarks>
        /// <returns>
        /// Returns a list of media items associated with the authenticated family in the response body. If the token is invalid or no media items are found for the family, an empty list is returned.
        /// </returns>

        [HttpGet("GetMediaForFamily")]
        public async Task<IActionResult> GetMediaForFamily()
        {
            string? token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Token invaild");
            }
            var res = await _familyService.GetMediaForFamilyAsync(token);
            if (res.Count() == 0)
            {
                return NotFound("No Media");
            }
            return Ok(res);
        }
        /// <summary>
        /// Uploads media files to the system associated with the authenticated family.
        /// </summary>
        /// <param name="addMediaDto">The DTO containing the media file and its associated caption.</param>
        /// <remarks>
        /// This endpoint allows authenticated families to upload media files to the system. Families need to provide a valid authentication token in the request header to access this functionality.
        /// Upon receiving the request, the controller verifies the validity of the provided token. If the token is invalid, a "Token invalid" error message is returned.
        /// If the token is valid, the controller proceeds to handle the media upload process. It first retrieves the FamilyId associated with the token. If no FamilyId is found, it indicates that the family does not exist or is not authenticated.
        /// The controller then retrieves the PatientId associated with the FamilyId. If no PatientId is found, it indicates that the family does not have a patient yet, and an appropriate error message is returned.
        /// If both the FamilyId and PatientId are valid, the controller saves the uploaded media file to the system. It generates a unique MediaId for the file and constructs the file path based on the FamilyId and MediaId. The media file is then saved to the designated directory on the server.
        /// After successfully saving the media file, the controller creates a new Media object and populates its properties with relevant information such as the MediaId, Caption, file path, upload date, and associated FamilyId and PatientId.
        /// The newly created Media object is then added to the database. Upon successful addition, a success message indicating that the media was added successfully is returned.
        /// </remarks>
        /// <returns>
        /// Returns an Ok response with a success message if the media upload process is successful. If the authentication token is invalid, a BadRequest response with an error message is returned.
        /// </returns>
        [HttpPost("UploadMedia")]
        public async Task<IActionResult> UploadMedia([FromForm] AddMediaDto addMediaDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            string? token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Token invaild");
            }
            var res = await _familyService.UploadMediaAsync(token, addMediaDto);
            if (res.HasError)
            {
                return BadRequest(res.message);
            }
            return Ok(res);
        }
        /// <summary>
        /// Adds a new appointment for the patient associated with the authenticated family.
        /// </summary>
        /// <param name="addAppointmentDto">The DTO containing details of the appointment to be added.</param>
        /// <remarks>
        /// This endpoint allows authenticated families to add appointments for their associated patients. Families need to provide a valid authentication token in the request header to access this functionality.
        /// Upon receiving the request, the controller verifies the validity of the provided token. If the token is invalid, a "Token invalid" error message is returned.
        /// If the token is valid, the controller proceeds to add the appointment. It first retrieves the FamilyId associated with the token. If no FamilyId is found, it indicates that the family does not exist or is not authenticated.
        /// The controller then checks if the family has a patient associated with it. If no patient is associated, it indicates that the person does not have a patient yet, and an appropriate error message is returned.
        /// If both the FamilyId and PatientId are valid, the controller creates a new Appointment object and populates its properties with details such as the appointment date, location, notes, and associated FamilyId and PatientId.
        /// The newly created Appointment object is then added to the database. Upon successful addition, a success message indicating that the appointment was added successfully is returned.
        /// Additionally, the controller notifies the connected appointment hub clients, specifically the group associated with the patient, about the newly added appointment.
        /// </remarks>
        /// <returns>
        /// Returns an Ok response with a success message if the appointment is added successfully. If the authentication token is invalid or if the family does not have a patient associated with it, a BadRequest response with an appropriate error message is returned.
        /// </returns>
        [HttpPost("AddAppointment")]
        public async Task<IActionResult> AddAppointment (AddAppointmentDto addAppointmentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
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
        /// <summary>
        /// Retrieves the appointments associated with the patient of the authenticated family.
        /// </summary>
        /// <remarks>
        /// This endpoint allows authenticated families to retrieve the appointments of their associated patients. Families need to provide a valid authentication token in the request header to access this functionality.
        /// Upon receiving the request, the controller verifies the validity of the provided token. If the token is invalid, a "Token invalid" error message is returned.
        /// If the token is valid, the controller proceeds to retrieve the appointments associated with the patient of the authenticated family. It first retrieves the FamilyId associated with the token. If no FamilyId is found, it indicates that the family does not exist or is not authenticated.
        /// The controller then checks if the family has a patient associated with it. If no patient is associated, or if the family itself does not exist, an empty list of appointments is returned.
        /// If both the FamilyId and PatientId are valid, the controller retrieves the appointments associated with the patient from the database. It then maps each appointment to a DTO (Data Transfer Object) containing details such as the appointment ID, date, location, notes, the name of the family who created the appointment, and a flag indicating whether the authenticated family has the permission to delete the appointment.
        /// The list of mapped appointment DTOs is returned as a response. If no appointments are found, a NotFound response with a "No Appointments" message is returned.
        /// </remarks>
        /// <returns>
        /// Returns an Ok response with the list of appointments associated with the patient if appointments are found. If the authentication token is invalid or if the family does not have a patient associated with it, an empty list is returned. If no appointments are found, a NotFound response with an appropriate message is returned.
        /// </returns>
        [HttpGet("GetPatientAppointments")]
        public async Task<IActionResult> GetPatientAppointments()
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
        /// <summary>
        /// Deletes a specific appointment associated with the authenticated family.
        /// </summary>
        /// <param name="AppointmentId">The unique identifier of the appointment to be deleted.</param>
        /// <remarks>
        /// This endpoint allows authenticated families to delete a specific appointment associated with their account. Families need to provide a valid authentication token in the request header to access this functionality.
        /// Upon receiving the request, the controller verifies the validity of the provided token. If the token is invalid, a "Token invalid" error message is returned.
        /// If the token is valid, the controller proceeds to check if the appointment to be deleted belongs to the authenticated family. If the appointment does not belong to the family or if the appointment ID is invalid, a corresponding error message is returned.
        /// If the appointment ID is valid and it belongs to the authenticated family, the controller deletes the appointment from the database.
        /// Upon successful deletion, an Ok response with a success message is returned.
        /// </remarks>
        /// <returns>
        /// Returns an Ok response with a success message upon successfully deleting the appointment. If the authentication token is invalid or if the appointment does not belong to the authenticated family, an appropriate error message is returned.
        /// </returns>
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
        /// <summary>
        /// Retrieves the reports associated with the patient of the authenticated family.
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPatientReports")]
       public async Task<IActionResult> GetPatientReports()
        {
            string? token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null)
            {
                return BadRequest("Token invaild");
            }
            var res = await _familyService.GetPatientReportsAsync(token);
            if (res.Count() == 0)
            {
                return NotFound("No Reports");
            }
            return Ok(res);
        }

    }
}
