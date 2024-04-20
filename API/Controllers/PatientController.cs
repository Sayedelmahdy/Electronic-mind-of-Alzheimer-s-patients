using BLL.DTOs;
using BLL.DTOs.FamilyDto;
using BLL.DTOs.PatientDto;
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
        /// <summary>
        /// Retrieves the profile of the patient .
        /// </summary>
        /// <remarks>
        /// This endpoint allows Patients to retrieve the profile information of themselves. The controller verifies the validity of the provided token to ensure authentication. If the token is invalid, a BadRequest response is returned.
        /// Upon successful validation of the token, the controller proceeds to fetch the patient's profile information, their profile information, including age, phone number, diagnosis date, Full Name ,and patient Id , is returned in the response.
        /// </remarks>
        /// <returns>
        /// If the profile is successfully retrieved, returns the patient's profile information in the response body. If the token is invalid or the patient is not Found, returns a BadRequest response with an appropriate error message.
        /// </returns>
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
        /// <summary>
        /// Retrieves the familiy members associated with the patient.
        /// </summary>
        /// <remarks>
        /// This endpoint allows patients to view their associated families. The controller verifies the validity of the provided token to ensure authentication. If the token is invalid, a BadRequest response is returned.
        /// Upon successful validation of the token, the controller retrieves all families where the patient's ID matches the `PatientId` property in the family entity. The response includes the family's ID, full name, relationship to the patient, and image URL (if available).
        /// </remarks>
        /// <returns>
        /// If families are found, returns a list of `GetFamiliesDto` objects representing each family. If the token is invalid or no families are associated with the patient, returns a NotFound response.
        /// </returns>
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
        /// <summary>
        /// Updates the profile of the patient.
        /// </summary>
        /// <param name="updatePatientProfileDto">The data required to update the patient's profile.</param>
        /// <remarks>
        /// This endpoint allows patients to modify their profile information. The controller verifies the validity of the provided token to ensure authentication. If the token is invalid, a BadRequest response is returned.
        /// Upon successful token validation, the controller retrieves the patient's information using the ID extracted from the token. It then updates the patient's profile with the provided details in the `UpdatePatientProfileDto` object. The updated information may include age, phone number, diagnosis date, and maximum distance (optional).
        /// If the update process encounters an error, such as invalid patient ID or update failure, the controller returns a BadRequest response with an appropriate error message. Otherwise, a success message is returned in the response body.
        /// </remarks>
        /// <returns>
        /// If the profile is updated successfully, returns a success message in the response body. If the token is invalid, the patient is not found, or the update fails, returns a BadRequest response with an error message.
        /// </returns>
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
        /// <summary>
        /// Retrieves the location of a specific family associated with the patient.
        /// </summary>
        /// <param name="familyId">The unique identifier of the family.</param>
        /// <remarks>
        /// This endpoint allows patients to retrieve the location information of a family they are associated with. The controller verifies the validity of the provided token to ensure authentication. If the token is invalid, a BadRequest response is returned.
        /// Upon successful token validation, the controller retrieves the family information using the provided `familyId`. It then checks if the retrieved family belongs to the patient based on the patient ID extracted from the token. If the family is not found, invalid, or doesn't belong to the patient, a BadRequest response is returned with an appropriate error message.
        /// If the family is found and valid, the controller returns the family's location information, including latitude and longitude.
        /// </remarks>
        /// <returns>
        /// If the family location is found, returns a `GetFamilyLocationDto` object containing the latitude, longitude, success code, and message. If the token is invalid, the family is not found, or there's an error, returns a BadRequest response with an error message.
        /// </returns>
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
        /// <summary>
        /// Marks a medication reminder as taken or skipped.
        /// </summary>
        /// <param name="medicationReminderDto">Data object containing the medication reminder ID and mark status.</param>
        /// <remarks>
        /// This endpoint allows patients to mark a specific medication reminder as taken or skipped. The controller verifies the validity of the provided token to ensure authentication. If the token is invalid, a BadRequest response is returned.
        /// Upon successful token validation, the controller retrieves the medication reminder using the provided `MedictaionId` from the request body and the patient ID extracted from the token. If the reminder is not found or doesn't belong to the patient, a BadRequest response is returned with an error message.
        /// If the reminder is found and valid, the controller creates a new `Mark_Medicine_Reminder` record with the reminder ID, mark time (current date and time), and the provided `IsTaken` flag indicating if the medication was taken or not. Finally, a success message is returned in the response body.
        /// </remarks>
        /// <returns>
        /// If the medication reminder is marked successfully, returns a success message in the response body. If the token is invalid, the reminder is not found, or there's an error, returns a BadRequest response with an error message.
        /// </returns>
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
        /// <summary>
        /// Uploads a secret file associated with the patient.
        /// </summary>
        /// <param name="addSecretFileDto">Data object containing details about the secret file.</param>
        /// <remarks>
        /// This endpoint allows patients to upload a secret file to their profile. The controller verifies the validity of the provided token to ensure authentication. If the token is invalid, a BadRequest response is returned.
        /// Upon successful token validation, the controller processes the uploaded file. It creates a unique file ID, constructs the file path within a patient-specific directory (under the web root path and "SecretFiles" subfolder), and saves the file content to the designated location.
        /// If the file upload or directory creation fails, a BadRequest response is returned with an error message. Otherwise, a new `SecretAndImportantFile` object is created with the uploaded file's details (filename, description, path, extension, permission end date, and patient ID). This object is then added to the database using the `_secret.AddAsync` method.
        /// If the file is successfully added to the database, a success message is returned in the response body. If any errors occur during the process, a BadRequest response is returned with a generic error message.
        /// </remarks>
        /// <returns>
        /// If the secret file is uploaded and added to the database successfully, returns a success message in the response body. If the token is invalid, there's an upload error, or an error occurs while adding the file to the database, returns a BadRequest response with an error message.
        /// </returns>
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
        /// <summary>
        /// Uploads a video file as a request to view secret files.
        /// </summary>
        /// <param name="askToViewDto">Data object containing the video file.</param>
        /// <remarks>
        /// This endpoint allows patients to submit a video request to view their secret files. The controller verifies the validity of the provided token to ensure authentication. If the token is invalid, a BadRequest response is returned.
        /// Upon successful token validation, the controller processes the uploaded video file. It checks if the file is null or empty. If so, a BadRequest response is returned with an "Empty video file" error message.
        /// If the video file is valid, the controller calls the `SaveVideoAsync` method (implementation details not provided) to save the video and obtain the file path. It then retrieves the patient information using the patient ID extracted from the token.
        /// If the video is saved successfully, the controller sends an email notification to a designated recipient (currently "hazemzizo@gmail.com") with the patient's full name and a link to the uploaded video. The link generation logic is assumed to be implemented in the `GetMediaUrl` method (implementation details not provided).
        /// If the email notification is sent successfully, a success message is returned in the response body. Otherwise, a BadRequest response is returned with an error message indicating the video upload succeeded but email sending failed.
        /// If any errors occur during video saving or email sending, a BadRequest response is returned with a generic error message.
        /// </remarks>
        /// <returns>
        /// If the video is uploaded successfully and the email notification is sent, returns a success message in the response body. If the token is invalid, the video is empty, there's an upload error, or email sending fails, returns a BadRequest response with an error message.
        /// </returns>
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
        /// <summary>
        /// Retrieves a list of secret files associated with the patient.
        /// </summary>
        /// <remarks>
        /// This endpoint allows patients to retrieve information about their secret files. The controller verifies the validity of the provided token to ensure authentication. If the token is invalid, a BadRequest response is returned.
        /// Upon successful token validation, the controller retrieves the patient ID from the token. It then checks if the patient exists and if any secret files are associated with the patient.
        /// If the patient or their secret files are not found, a NotFound response is returned with an appropriate message.
        /// If the patient and their secret files are found, the controller iterates through the files and builds a list of `GetSecretFileDto` objects containing details like secret ID, filename, description, document URL (if the patient has permission to view), document extension (if the patient has permission to view), and a flag indicating if confirmation is needed to view the file.
        /// The list of `GetSecretFileDto` objects is then returned in the response body along with a success code (200 OK).
        /// </remarks>
        /// <returns>
        /// If the patient and their secret files are found, returns a list of `GetSecretFileDto` objects containing secret file details. If the token is invalid, the patient or their secret files are not found, returns an appropriate error response.
        /// </returns>
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
        /// <summary>
        /// Retrieves a list of appointments for the patient.
        /// </summary>
        /// <remarks>
        /// This endpoint allows patients to view their upcoming appointments. The controller verifies the validity of the provided token to ensure authentication. If the token is invalid, a BadRequest response is returned.
        /// Upon successful token validation, the controller retrieves the patient ID from the token. It then checks if the patient exists and if the patient has any appointments.
        /// If the patient is not found or has no appointments, a NotFound response is returned with a "No Appointments" message.
        /// If the patient and their appointments are found, the controller iterates through the appointments and builds a list of `GetAppointmentDto` objects containing details like appointment ID, date, location, notes, and the full name of the family member who created the appointment (assuming appointments are associated with a family).
        /// The list of `GetAppointmentDto` objects is then returned in the response body along with a success code (200 OK).
        /// </remarks>
        /// <returns>
        /// If the patient and their appointments are found, returns a list of `GetAppointmentDto` objects containing appointment details. If the token is invalid, the patient or their appointments are not found, returns an appropriate error response.
        /// </returns>
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
        /// <summary>
        /// Retrieves a list of medication reminders for the patient.
        /// </summary>
        /// <remarks>
        /// This endpoint allows patients to view their medication reminders. The controller verifies the validity of the provided token to ensure authentication. If the token is invalid, a BadRequest response is returned.
        /// Upon successful token validation, the controller retrieves the patient ID from the token. It then checks if the patient exists and if the patient has any medication reminders.
        /// If the patient is not found or has no medication reminders, a NotFound response is returned with a "No Medicines Yet" message.
        /// If the patient and their medication reminders are found, the controller iterates through the reminders and builds a list of `MedicationReminderGetDto` objects containing details like medication name, reminder ID, start date, dosage, repeat information (if applicable), end date (if applicable), and medication type.
        /// The list of `MedicationReminderGetDto` objects is then returned in the response body along with a success code (200 OK).
        /// 
        /// **Enumeration Descriptions:**
        /// 
        /// * **RepeatType:**
        ///   - **Once (0):** Medication reminder occurs only once.
        ///   - **Twice (1):** Medication reminder occurs twice a day.
        ///   - **Three_Times (2):** Medication reminder occurs three times a day.
        ///   - **Four_Times (3):** Medication reminder occurs four times a day.
        /// * **MedcineType:**
        ///   - **Bottle (0):** Medication comes in a bottle.
        ///   - **Pill (1):** Medication comes in pill form.
        ///   - **Syringe (2):** Medication comes in a syringe.
        ///   - **Tablet (3):** Medication comes in tablet form.
        /// </remarks>
        /// <returns>
        /// If the patient and their medication reminders are found, returns a list of `MedicationReminderGetDto` objects containing medication details. If the token is invalid, the patient or their medication reminders are not found, returns an appropriate error response.
        /// </returns>
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
        /// <summary>
        /// Retrieves a list of media files associated with the patient.
        /// </summary>
        /// <remarks>
        /// This endpoint allows patients to view their uploaded media files. The controller verifies the validity of the provided token to ensure authentication. If the token is invalid, a BadRequest response is returned.
        /// Upon successful token validation, the controller retrieves the patient ID from the token. It then checks if the patient exists and if the patient has any media files.
        /// If the patient is not found or has no media files, a NotFound response is returned with a "No Media Yet" message.
        /// If the patient and their media files are found, the controller iterates through the files and builds a list of `GetMediaforPatientDto` objects containing details like caption, media URL (assuming a function `GetMediaUrl` exists to generate the URL), media ID, upload date, media extension, and the full name of the family member who uploaded the media (assuming media is associated with a family).
        /// The list of `GetMediaforPatientDto` objects is then returned in the response body along with a success code (200 OK).
        /// </remarks>
        /// <returns>
        /// If the patient and their media files are found, returns a list of `GetMediaforPatientDto` objects containing media details. If the token is invalid, the patient or their media files are not found, returns an appropriate error response.
        /// </returns>
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
        /// <summary>
        /// Retrieves a list of game scores for the patient and provides a recommendation for the next difficulty level based on their current score.
        /// </summary>
        /// <remarks>
        /// This endpoint allows patients to view their game scores and receive a recommendation for the next difficulty level based on their current score. The controller verifies the validity of the provided token to ensure authentication. If the token is invalid, a BadRequest response is returned.
        /// Upon successful token validation, the controller retrieves the patient ID from the token. It then checks if the patient exists.
        /// If the patient is not found, a NotFound response is returned with a "No Game Scores Yet" message.
        /// If the patient is found, the controller retrieves the patient's game scores and builds a list of `GameScoreDto` objects containing details like game score ID, difficulty level, and patient's score.
        /// The controller then analyzes the patient's current score stored in the database and recommends a difficulty level (0 - Easy, 1 - Medium, 2 - Hard) based on the following logic:
        ///   - If the current score is between 200 and 400 (exclusive), a difficulty recommendation of **1 (Medium)** is provided.
        ///   - If the current score is 400 or higher, a difficulty recommendation of **2 (Hard)** is provided.
        ///   - If the current score is less than 200, a difficulty recommendation of **0 (Easy)** is provided.
        /// Finally, the controller returns a `GetGameScoresDto` object containing the list of game scores and the recommended difficulty level in the response body along with a success code (200 OK).
        /// </remarks>
        /// <returns>
        /// If the patient and their game scores are found, returns a `GetGameScoresDto` object containing game scores and a difficulty recommendation. If the token is invalid or the patient is not found, returns an appropriate error response.
        /// </returns>
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
        /// <summary>
        /// Adds a new game score for the patient.
        /// </summary>
        /// <remarks>
        /// This endpoint allows patients to submit their game scores. The controller first validates the request body to ensure it meets the expected format. If validation fails, a BadRequest response is returned with details about the validation errors.
        /// Upon successful validation, the controller verifies the validity of the provided token to ensure authentication. If the token is invalid, a BadRequest response is returned.
        /// The controller then retrieves the patient ID from the token and checks if the patient exists. If the patient ID is invalid or the patient is not found, a BadRequest response is returned with an appropriate error message.
        /// If the patient is found, a new `GameScore` object is created based on the information in the request body (difficulty and patient score). The new game score is added to the database.
        /// The controller then calculates the score update based on the difficulty and patient's score using the following logic:
        ///   - Difficulty:
        ///     - **Easy (0):** Base score of 3. If patient score is 3 or higher, score is calculated as patient score * 10. Otherwise, score is calculated as (3 - patient score) * -10.
        ///     - **Medium (1):** Base score of 6. Similar logic as Easy difficulty with target score of 6.
        ///     - **Hard (2):** Base score of 9. Similar logic as Easy difficulty with target score of 9.
        /// The patient's current score is updated by adding the calculated score. The patient's maximum score is also updated if the current score is higher than the existing maximum score.
        /// Finally, the controller returns a `GlobalResponse` object in the response body along with a success code (200 OK). The response object contains a message indicating if there were any errors (HasError property) and a detailed message about the score update (message property).
        /// </remarks>
        /// <param name="addGameScoreDto">Request body containing the game score details (difficulty and patient score).</param>
        /// <returns>
        /// If the game score is added successfully, returns a `GlobalResponse` object with success message. If validation fails, the token is invalid, the patient is not found, or there is an error adding the score, returns a `GlobalResponse` object with error message.
        /// </returns>

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
        [HttpGet("GetCurrentAndMaxScore")]
        public async Task<IActionResult> GetCurrentAndMaxScore()
        {
            var token = HttpContextHelper.GetToken(this.HttpContext);
            if (token == null) 
            {
                return BadRequest("Invalid Token");
            }
            var res = await _patientService.GetRecommendedScoreAsync(token);
            if (res == null)
            {
                return NotFound("No Game Scores Yet .. ");
            }
            return Ok(res);
        }
    }
}
