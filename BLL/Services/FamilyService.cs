using BLL.DTOs;
using BLL.DTOs.FamilyDto;
using BLL.Helper;
using BLL.Hubs;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace BLL.Services
{
    public class FamilyService : IFamilyService
    {
        private readonly IBaseRepository<Family> _family;
        private readonly IBaseRepository<Patient> _patient;
        private readonly IBaseRepository<Caregiver> _caregiver;
        private readonly IBaseRepository<Appointment> _Appointments;
        private readonly IBaseRepository<Location> _location;
        private readonly IBaseRepository<PersonWithoutAccount> _personWithoutAccount;
        private readonly IDecodeJwt _jwtDecode;
        private readonly IWebHostEnvironment _env;
        private readonly JWT _jwt;
        private readonly IMailService _mailService;
        private readonly IHubContext<AppointmentHub> _appointmentHub;
        private readonly IBaseRepository<Media> _Media;
        private readonly UserManager<User> _userManager;
        private readonly IBaseRepository<Report> _report;
        private readonly Mail _mail;

        public FamilyService(
            IBaseRepository<Media> Media,
            IBaseRepository<Family> family,
            IBaseRepository<Patient> patient,
            IBaseRepository<Caregiver> caregiver,

            IBaseRepository<Appointment> appointment,
            IBaseRepository<Location> location,
            IDecodeJwt jwtDecode,
            IWebHostEnvironment env,
              IOptions<Mail> Mail,
              IOptions<JWT> JWT,
              IMailService mailService,
              IHubContext<AppointmentHub> appointmentHub,
            UserManager<User> user,
            IBaseRepository<Report> report,
            IBaseRepository<PersonWithoutAccount> personWithoutAccount
            )
        {
            _personWithoutAccount = personWithoutAccount;
            _Media = Media;
            _family = family;
            _patient = patient;
            _caregiver = caregiver;
            _mail = Mail.Value;
            _Appointments = appointment;
            _jwtDecode = jwtDecode;
            _env = env;
            _jwt = JWT.Value;

            _mailService = mailService;
            _appointmentHub = appointmentHub;
            _userManager = user;
            _location = location;
            _report = report;
        }
        /// <summary>
        /// Retrieves the patient code associated with the provided token.
        /// </summary>
        /// <returns>
        /// If successful, returns the patient code. Otherwise, returns a BadRequest response with an error message.
        /// </returns>
        public async Task<string?> GetPatientCode(string token)
        {
            string? FamilyId = _jwtDecode.GetUserIdFromToken(token);
            if (FamilyId == null)
            {
                return null;
            }
            var Family = await _family.GetByIdAsync(FamilyId);


            return Family?.PatientId;

        }

        public async Task<GetPatientProfile?> UpdatePatientProfileAsync(string token, UpdatePatientProfileDto updatePatientProfileDto)
        {
            string? FamilyId = _jwtDecode.GetUserIdFromToken(token);
            if (FamilyId == null)
            {
                return null;
            }
            var Family = await _family.Include(p => p.patient).FirstOrDefaultAsync(p => p.Id == FamilyId);
            if (Family.patient.FamilyCreatedId != FamilyId)
            {
                return new GetPatientProfile
                {
                    ErrorAppear = true,
                    Message = "This user doesn't Create this Patient to update his profile",
                };
            }
            var patient = await _patient.GetByIdAsync(Family.patient.Id);
            patient.Age = updatePatientProfileDto.Age;
            patient.PhoneNumber = updatePatientProfileDto.PhoneNumber;
            patient.DiagnosisDate = updatePatientProfileDto.DiagnosisDate.ToDateTime(TimeOnly.MinValue);
            patient.MaximumDistance = updatePatientProfileDto.MaximumDistance;
            await _patient.UpdateAsync(patient);
            return new GetPatientProfile
            {
                ErrorAppear = false,
                Message = "Patient Updated Succesfully",
                Age = patient.Age,
                PhoneNumber = patient.PhoneNumber,
                DiagnosisDate = patient.DiagnosisDate.Value.Date.ToShortDateString(),
                Email = patient.Email,
                FullName = patient.FullName,
                relationality = Family.Relationility,
            };

        }
        public async Task<GetPatientProfile> GetPatientProfile(string token)
        {
            string? FamilyId = _jwtDecode.GetUserIdFromToken(token);
            if (FamilyId == null)
            {
                return new GetPatientProfile
                {
                    ErrorAppear = true,
                    Message = "Token Doesn't have FamilyId"
                };
            }
            if (_family.GetById(FamilyId).PatientId == null)
            {
                return new GetPatientProfile
                {
                    ErrorAppear = true,
                    Message = "This Person Doesn't have Patient yet"
                };
            }
            var res = await _family.Include(p => p.patient).FirstOrDefaultAsync(p => p.Id == FamilyId);
            return new GetPatientProfile
            {
                Message = "Patient Profile returned Succesfully",
                FullName = res.patient.FullName,
                Age = res.patient.Age,
                Email = res.patient.Email,
                PhoneNumber = res.patient.PhoneNumber,
                relationality = res.Relationility,
                DiagnosisDate = res.patient.DiagnosisDate.Value.ToShortDateString(),
                MaxDistance = res.patient.MaximumDistance

            };
        }
        public async Task<GlobalResponse> AddPatientAsync(string token, AddPatientDto addPatientDto)
        {
            string? FamilyId = _jwtDecode.GetUserIdFromToken(token);
            if (FamilyId == null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "Token Not Have ID"
                };
            }
            if (await _userManager.FindByEmailAsync(addPatientDto.Email) is not null)
                return new GlobalResponse { message = "Email is already registered!", HasError = true };


            IdentityResult? result = null;


            string htmlContent = @"<!DOCTYPE html>
                <html lang=""en"">

                <head>
                    <meta charset=""utf-8"" />
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Email Confirmation</title>
                        <style>
                            body {
                         font-family: 'Arial', sans-serif;
                         background-color: #f8f8f8;
                    margin: 0;
                    padding: 0;
                }

                .container {
                    position: relative;
                    width: 80%;
                    max-width: 600px;
                    margin: 20px auto;
                    background-color: #fff;
                    padding: 20px;
                    border-radius: 8px;
                    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                    border: 1px solid #ddd; /* Add border for an elegant frame */
                    overflow: hidden; /* Clear the float */
                }

                h1 {
                    color: #3498db;
                    margin-bottom: 20px;
                }

                p {
                    font-size: 16px;
                    color: #333;
                    margin-bottom: 10px;
                }

                a {
                    text-decoration: none;
                    color: #3498db;
                    font-weight: bold;
                }

                a:hover {
                    text-decoration: underline;
                }

                .button {
                    display: inline-block;
                    padding: 10px 20px;
                    font-size: 14px;
                    font-weight: bold;
                    text-align: center;
                    text-decoration: none;
                    background-color: #3498db;
                    color: #fff;
                    border-radius: 5px;
                }

                .footer {
                    margin-top: 20px;
                    font-size: 12px;
                    color: #777;
                }

                /* Emotes on the side */
                .emotes {
                    position: absolute;
                    top: 0;
                    right: 0;
                    padding: 10px;
                }

                .emotes img {
                    width: 30px;
                    height: 30px;
                    margin-left: 10px;
                }
            </style>
        </head>

        <body>
            <div class=""container"">
                <div class=""emotes"">
                    <img src=""https://drive.google.com/uc?export=view&id=1j1q86kEhug5VC18WGIrL1U_m9vlPAjxU"" alt=""Congratulations Emote 1"">
                    <img src=""https://drive.google.com/uc?export=view&id=1dn-moFQyJ_hlehjv4-9Mc5I6L6VhAe7Q"" alt=""Congratulations Emote 2"">
                </div>
                <h1>Welcome to the Electronic Mind of Alzheimer Patient</h1>
                <p>Dear {FullName},</p>
                <p>We are thrilled to have you on board! To ensure the security of your account, please confirm your email address by clicking the link below:</p>
                <p><a class=""button"" href='{url}'>Confirm Your Email</a></p>
                <p>If you did not create an account or need further assistance, please disregard this email.</p>
                <div class=""footer"">
                    <p>Best regards,</p>
                    <p>The Electronic Mind Team</p>
                </div>
            </div>
        </body>

        </html>


        ";
            Patient patient = new Patient
            {
                Email = addPatientDto.Email,
                Age = addPatientDto.Age,
                FullName = addPatientDto.FullName,
                UserName = addPatientDto.Email.Split('@')[0],
                DiagnosisDate = addPatientDto.DiagnosisDate.ToDateTime(TimeOnly.MinValue),
                PhoneNumber = addPatientDto.PhoneNumber,
                FamilyCreatedId = FamilyId,
                MaximumDistance = addPatientDto.MaximumDistance,
                MainLatitude = addPatientDto.MainLatitude,
                MainLongitude = addPatientDto.MainLongitude,
            };
            var Created = await _userManager.CreateAsync(patient, addPatientDto.Password);
            if (Created == null || !Created.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in Created.Errors)
                    errors += $"{error.Description},";
                return new GlobalResponse
                {

                    HasError = true,
                    message = errors
                };
            }
            await _userManager.AddToRoleAsync(patient, "patient");
            string MediaId = Guid.NewGuid().ToString();

            string filePath = Path.Combine("User Avatar", $"{patient.Id}_{MediaId}{Path.GetExtension(addPatientDto.Avatar.FileName)}");
            string directoryPath = Path.Combine(_env.WebRootPath, "User Avatar");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            using (FileStream filestream = File.Create(Path.Combine(_env.WebRootPath, filePath)))
            {
                addPatientDto.Avatar.CopyTo(filestream);
                filestream.Flush();
            }
            patient.imageUrl = filePath;
            await _userManager.UpdateAsync(patient);
            var ResultOfAi = await RegisterPatientToAi(patient.Id, addPatientDto.Avatar);
            if (!ResultOfAi)
            {
                File.Delete(Path.Combine(_env.WebRootPath, filePath));
                await _userManager.DeleteAsync(patient);
                return new GlobalResponse
                {
                    HasError = true,
                    message = "something went wrong now get back after sometime,Ai Service is down"
                };
            }
            var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(patient);

            var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
            var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

            string url = $"{_mail.ServerLink}/api/Authentication/confirmemail?userid={patient.Id}&token={validEmailToken}";
            htmlContent = htmlContent.Replace("{FullName}", patient.FullName).Replace("{url}", url);
            //var res=  await _mailService.SendEmailAsync(patient.Email, _mail.FromMail, _mail.Password, "Confirm your email", htmlContent);
            //Todo: send email when project is ready
            var family = await _family.GetByIdAsync(FamilyId);
            /* if (!res || family==null)
             {
              await  _userManager.DeleteAsync(patient);
                 return new GlobalResponse
                 {
                     HasError = true,
                     message = "something went wrong now get back after sometime"
                 };
             }*/

            family.Relationility = addPatientDto.relationality;
            family.PatientId = patient.Id;
            family.DescriptionForPatient = addPatientDto.DescriptionForPatient;

            await _family.UpdateAsync(family);

            /* var Result = await RegisterFamilyToAi(family.PatientId, family.Id, family.imageUrl);
             if (!Result)
             {
                 File.Delete(Path.Combine(_env.WebRootPath, filePath));
                 await _userManager.DeleteAsync(patient);
                 return new GlobalResponse()
                 {
                     HasError = true,
                     message = "something went wrong now get back after sometime ,Ai Service is down"
                 };
             }*/
            return new GlobalResponse
            {
                HasError = false,
                message = "Patient Added Succsfully, and you need to send a training images"
            };
        }

        public async Task<GlobalResponse> AssignPatientToFamily(string token, AssignPatientDto assignPatientDto)
        {
            string? FamilyId = _jwtDecode.GetUserIdFromToken(token);
            if (FamilyId == null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "Token Not Have ID"
                };
            }
            Family? family = await _family.GetByIdAsync(FamilyId);
            if (family.PatientId != null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "family alredy have assigned to patient"
                };

            }
            if (family == null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "invalid Family ID"
                };
            }
            if (await _patient.GetByIdAsync(assignPatientDto.PatientCode) == null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "invalid Patient Code"
                };
            }

            family.PatientId = assignPatientDto.PatientCode;
            family.Relationility = assignPatientDto.relationility;
            family.DescriptionForPatient = assignPatientDto.DescriptionForPatient;
            await _family.UpdateAsync(family);

            /* var result = await RegisterFamilyToAi(family.PatientId, family.Id, family.imageUrl);
             if (!result)
             {

                 return new GlobalResponse()
                 {
                     HasError = true,
                     message = "something went wrong now get back after sometime ,Ai Service is down"
                 };
             }*/
            return new GlobalResponse
            {
                HasError = false,
                message = "Patient Assigned to this family succesfully, and you need to send a training images"
            };
        }
        public async Task<IEnumerable<GetAppointmentDto>> GetPatientAppointmentsAsync(string token)
        {
            string? FamilyId = _jwtDecode.GetUserIdFromToken(token);
            if (FamilyId == null)
            {
                return Enumerable.Empty<GetAppointmentDto>();
            }
            var family = await _family.GetByIdAsync(FamilyId);
            if (family == null || family.PatientId == null)
            {

                return Enumerable.Empty<GetAppointmentDto>();
            }
            var appointment = _Appointments.Where(p => p.PatientId == family.PatientId).ToList().Select(p => new GetAppointmentDto
            {
                AppointmentId = p.AppointmentId,
                Date = p.Date,
                Location = p.Location,
                Notes = p.Notes,
                FamilyNameWhoCreatedAppointemnt = _family.GetById(p.FamilyId).FullName,
                CanDeleted = (p.FamilyId == FamilyId) ? true : false
            });
            return appointment;
        }
        public async Task<GlobalResponse> AddAppointmentAsync(string token, AddAppointmentDto addAppointmentDto)
        {
            string? FamilyId = _jwtDecode.GetUserIdFromToken(token);
            if (FamilyId == null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "Token Not Have ID"
                };
            }
            var family = await _family.GetByIdAsync(FamilyId);
            if (family.PatientId == null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "This person doesn't have Patient yet"
                };
            }
            Appointment appointment = new Appointment
            {
                Date = addAppointmentDto.Date,
                Location = addAppointmentDto.Location,
                Notes = addAppointmentDto.Notes,
                FamilyId = family.Id,
                PatientId = family.PatientId,
            };
            var JsonAppointment = new
            {
                AppointmentId = appointment.AppointmentId,
                Date = appointment.Date,
                Location = appointment.Location,
                Notes = appointment.Notes,
                FamilyNameWhoCreatedAppointemnt = _family.GetById(appointment.FamilyId).FullName,

            };
            var Json = JsonConvert.SerializeObject(JsonAppointment);
            await _Appointments.AddAsync(appointment);
            await _appointmentHub.Clients.Group(family.PatientId).SendAsync("ReceiveAppointment", "Appointment Added", Json);
            return new GlobalResponse
            {
                HasError = false,
                message = "Appointment added successfully"
            };
        }
        public async Task<GlobalResponse> DeleteAppointmentAsync(string token, string AppointmentId)
        {
            string? FamilyId = _jwtDecode.GetUserIdFromToken(token);
            if (FamilyId == null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "Token Not Have ID"
                };
            }
            var Appointemnt = await _Appointments.GetByIdAsync(AppointmentId);
            if (Appointemnt.FamilyId != FamilyId)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "This user didn't Create this appointment so he cann't delete it"
                };

            }

            await _Appointments.DeleteAsync(Appointemnt);
            var AppointmentJson = "{\"AppointmentId\":\"" + Appointemnt.AppointmentId + "\"}";
            var Json = JsonConvert.SerializeObject(AppointmentJson);
            await _appointmentHub.Clients.Group(Appointemnt.PatientId).SendAsync("ReceiveAppointment", "Appointment deleted", Json);
            return new GlobalResponse
            {
                HasError = false,
                message = "Appointment Deleted Successfully"
            };

        }
        public async Task<IEnumerable<GetMediaDto>> GetMediaForFamilyAsync(string token)
        {

            string? FamilyId = _jwtDecode.GetUserIdFromToken(token);
            if (FamilyId == null)
            {
                return Enumerable.Empty<GetMediaDto>();
            }
            var Media = await _Media.Include(p => p.patient).Where(p => p.FamilyId == FamilyId).ToListAsync();
            var res = Media.Select(p => new GetMediaDto
            {

                Caption = p.Caption,
                MediaUrl = GetMediaUrl(p.Image_Path),
                MediaId = p.Media_Id,
                Uploaded_date = p.Upload_Date,
                MediaExtension = p.Extension
            }).ToList();
            return res;
        }

        public async Task<GlobalResponse> UploadMediaAsync(string token, AddMediaDto addMediaDto)
        {
            string? FamilyId = _jwtDecode.GetUserIdFromToken(token);
            if (FamilyId == null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "Token Not Have ID"
                };
            }
            string? PateintId = _family.GetById(FamilyId).PatientId;
            if (PateintId == null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "This Family doesn't have patient yet"

                };
            }
            string MediaId = Guid.NewGuid().ToString();
            string filePath = Path.Combine(PateintId, $"{FamilyId}_{MediaId}{Path.GetExtension(addMediaDto.MediaFile.FileName)}");
            string directoryPath = Path.Combine(_env.WebRootPath, PateintId);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            using (FileStream filestream = File.Create(Path.Combine(_env.WebRootPath, filePath)))
            {
                addMediaDto.MediaFile.CopyTo(filestream);
                filestream.Flush();
            }
            Media media = new Media
            {
                Media_Id = MediaId,
                Caption = addMediaDto.Caption,
                Image_Path = filePath,
                Upload_Date = DateTime.UtcNow,
                Extension = Path.GetExtension(addMediaDto.MediaFile.FileName),
                FamilyId = FamilyId,
                PatientId = PateintId,

            };
            await _Media.AddAsync(media);
            return new GlobalResponse
            {
                HasError = false,
                message = "Media Added Succesfully"
            };
        }



        public async Task<GlobalResponse> AssignPatientToCaregiver(string token, string CaregiverCode)
        {
            string? FamilyId = _jwtDecode.GetUserIdFromToken(token);
            if (FamilyId == null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "Token Not Have ID"
                };
            }
            var Family = _family.GetById(FamilyId);
            if (Family.PatientId == null)
            {

                return new GlobalResponse
                {
                    HasError = true,
                    message = "This Family doesn't have patient yet"

                };
            }
            if (_caregiver.GetById(CaregiverCode) == null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "Caregiver Code invaild"

                };
            }
            var patient = await _patient.GetByIdAsync(Family.PatientId);
            patient.CaregiverID = CaregiverCode;
            await _patient.UpdateAsync(patient);
            return new GlobalResponse
            {
                HasError = false,
                message = "Patient Assigned to Caregiver succesfully"
            };
        }


        public async Task<IEnumerable<LocationDto>> GetPatientLocationsTodayAsync(string token)
        {
            string? FamilyId = _jwtDecode.GetUserIdFromToken(token);
            if (FamilyId == null)
            {
                return Enumerable.Empty<LocationDto>();
            }
            var Family = _family.GetById(FamilyId);
            if (Family == null)
            {
                return Enumerable.Empty<LocationDto>();
            }
            var result = await _location.WhereAsync(p => p.PatientId == Family.PatientId && p.Timestamp.Date.Day == DateTime.Now.Date.Day && p.Timestamp.Date.Year == DateTime.Now.Date.Year && p.Timestamp.Date.Month == DateTime.Now.Date.Month);
            return result.Select(p => new LocationDto
            {
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                TimeStamp = p.Timestamp,

            }).ToList();
        }

        public async Task<IEnumerable<GetReportDto>> GetPatientReportsAsync(string token)
        {
            string? FamilyId = _jwtDecode.GetUserIdFromToken(token);
            if (FamilyId == null)
            {
                return Enumerable.Empty<GetReportDto>();
            }
            var Family = _family.GetById(FamilyId);
            if (Family == null)
            {
                return Enumerable.Empty<GetReportDto>();
            }
            var result = await _report.WhereAsync(p => p.PatientId == Family.PatientId);
            if (result == null || !result.Any())
            {
                return Enumerable.Empty<GetReportDto>();
            }
            return result.Select(p => new GetReportDto
            {
                FromDate = p.FromDate.ToString("yyyy-MM-dd"),
                ReportContent = p.ReportContent,
                ToDate = p.ToDate.ToString("yyyy-MM-dd"),
                ReportId = p.ReportId,
            }).ToList();

        }
        public async Task<NeedATrainingImageDto> FamilyNeedATraining(string token)
        {
            string? FamilyId = _jwtDecode.GetUserIdFromToken(token);
            if (FamilyId == null)
            {
                return new NeedATrainingImageDto
                {
                    GlobalResponse = new GlobalResponse
                    {
                        HasError = true,
                        message = "Token Not Have ID"
                    }
                };
            }
            var Family = _family.GetById(FamilyId);
            if (Family.PatientId == null)
            {
                return new NeedATrainingImageDto
                {
                    NeedATraining = false,
                    GlobalResponse = new GlobalResponse()
                    {
                        HasError = true,
                        message = "This Family doesn't have patient yet"
                    }
                };
            }
            if (Family.NumberOfTrainingImage >= 5)
            {
                return new NeedATrainingImageDto
                {
                    GlobalResponse = new GlobalResponse
                    {
                        HasError = false,
                        message = "Already have 5 images"
                    },
                    NeedATraining = false,

                };
            }
            List<ImageSamplesWithInstractions> ImagesSamplesUrls = new List<ImageSamplesWithInstractions>();
            for (int i = Family.NumberOfTrainingImage + 1; i < 6; i++)
            {
                var pathOfImage = Path.Combine(_env.WebRootPath, "FaceExmaple", $"{i}.jpg");
                string Instruction = "";
                if (i == 1)
                {
                    Instruction = "Front View: Directly facing the camera, eyes looking straight ahead";
                }
                else if (i == 2)
                {
                    Instruction = "3/4 Right View: The head turned slightly so the right side is more visible than the left";
                }
                else if (i == 3)
                {
                    Instruction = "Fully Right View: Turn your head to the right until your profile aligns with the camera";

                }
                else if (i == 4)
                {
                    Instruction = "3/4 Left View: The head turned slightly so the left side is more visible than the right.";
                }
                else if (i == 5)
                {
                    Instruction = "Fully Left View: Turn your head to the left until your profile aligns with the camera.";
                }
                var image = GetMediaUrl(pathOfImage);
                var imageWithInstruction = new ImageSamplesWithInstractions()
                {
                    ImageSampleUrl = image,
                    Instraction = Instruction
                };
                ImagesSamplesUrls.Add(imageWithInstruction);

            }

            return new NeedATrainingImageDto()
            {
                GlobalResponse = new GlobalResponse
                {
                    HasError = false,
                    message = "Succes"
                },
                ImagesSamplesWithInstractions = ImagesSamplesUrls,
                NeedATraining = true

            };
        }

        public async Task<GlobalResponse> TrainingImage(string token, AddTrainingImageDto addTrainingImageDto)
        {
            string? FamilyId = _jwtDecode.GetUserIdFromToken(token);
            if (FamilyId == null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "Token Not Have ID"
                };
            }
            var Family = _family.GetById(FamilyId);
            if (Family.PatientId == null)
            {

                return new GlobalResponse
                {
                    HasError = true,
                    message = "This Family doesn't have patient yet"

                };
            }
            foreach (var item in addTrainingImageDto.TrainingImages)
            {
                var result = await RegisterFamilyToAi(Family.PatientId, Family.Id, item, Family.NumberOfTrainingImage + 1);
                if (result.Item1 == "400" || result.Item1 == "500")
                {
                    return new GlobalResponse()
                    {
                        HasError = true,
                        message = result.Item2 + "\n Please send all image again after change the photo with wronged face"
                    };
                }
                Family.NumberOfTrainingImage += 1;
                await _family.UpdateAsync(Family);


            }
            return new GlobalResponse()
            {
                HasError = false,
                message = "Training image successfully"
            };
        }
        public async Task<GlobalResponse> AddPersonWithoutAccount(string token, AddPersonWithoutAccountDto addPersonWithoutAccountDto)
        {
            string? FamilyId = _jwtDecode.GetUserIdFromToken(token);
            if (FamilyId == null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "Token is not valid"
                };
            }
            var Family = _family.GetById(FamilyId);
            if (Family == null)
            {
                return new GlobalResponse()
                {
                    HasError = true,
                    message = "Family Not Found"
                };
            }
            if (Family.PatientId == null)
            {
                return new GlobalResponse()
                {
                    HasError = true,
                    message = "This Family doesn't have patient yet"
                };
            }
            if (addPersonWithoutAccountDto.TraningImage.Count < 5)
            {
                return new GlobalResponse()
                {
                    HasError = true,
                    message = "Please send at least 5 image for training with Different angles like face samples"
                };
            }
            string MediaId = Guid.NewGuid().ToString();

            string filePath = Path.Combine("User Avatar", $"{MediaId}{Path.GetExtension(addPersonWithoutAccountDto.TraningImage[0].FileName)}");
            string directoryPath = Path.Combine(_env.WebRootPath, "User Avatar");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            using (FileStream filestream = File.Create(Path.Combine(_env.WebRootPath, filePath)))
            {
                addPersonWithoutAccountDto.TraningImage[0].CopyTo(filestream);
                filestream.Flush();
            }
            var Person = new PersonWithoutAccount
            {
                PatientId = Family.PatientId,
                FullName = addPersonWithoutAccountDto.FullName,
                DescriptionForPatient = addPersonWithoutAccountDto.DescriptionForPatient,
                MainLatitude = addPersonWithoutAccountDto.MainLatitude,
                MainLongitude = addPersonWithoutAccountDto.MainLongitude,
                PhoneNumber = addPersonWithoutAccountDto.PhoneNumber,
                ImageUrl = filePath,
                Relationility = addPersonWithoutAccountDto.Relationility,

            };
            await _personWithoutAccount.AddAsync(Person);
            for (int i = 0; i < addPersonWithoutAccountDto.TraningImage.Count(); i++)
            {
                var result = await RegisterFamilyToAi(Person.PatientId, Person.Id, addPersonWithoutAccountDto.TraningImage[i], i + 1);
                if (result.Item1 == "400" || result.Item1 == "500")
                {
                    File.Delete(Path.Combine(_env.WebRootPath, filePath));
                    await _personWithoutAccount.DeleteAsync(Person);
                    return new GlobalResponse()
                    {
                        HasError = true,
                        message = result.Item2 + "\n Please add person again and send all image again after change the photo with wronged face"
                    };
                }
            }


            return new GlobalResponse
            {
                HasError = false,
                message = "Person Added Succesfully, and training image successfully"
            };
        }
        private async Task<Tuple<string, string>> RegisterFamilyToAi(string PatientId, string UserId, IFormFile Image, int idx)
        {
            string endpoint = "https://excited-hound-vastly.ngrok-free.app/register_image";

            using (HttpClient httpClient = new HttpClient())
            {
                // Read the image bytes


                // Create multipart form-data content
                var multipartContent = new MultipartFormDataContent();

                // Add patient_id and family_member_id as query parameters
                var queryParameters = new System.Collections.Generic.Dictionary<string, string>
            {
                { "patient_id", PatientId },
                { "idx", idx.ToString() },
                { "family_member_id", UserId }
            };

                // Add the image as a stream content
                multipartContent.Add(new StreamContent(Image.OpenReadStream()), "image", "image.jpg");

                // Build query string
                var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
                foreach (var param in queryParameters)
                {
                    queryString[param.Key] = param.Value;
                }

                var fullUrl = endpoint + "?" + queryString;

                // Send the request
                var response = await httpClient.PostAsync(fullUrl, multipartContent);

                // Handle the response
                if (response.IsSuccessStatusCode)
                {

                    return new Tuple<string, string>("200", "Image registered successfully");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return new Tuple<string, string>("400", $"No face detected or more than one face in the uploaded image number {idx}");
                }
                else
                {
                    return new Tuple<string, string>("500", "Something went wrong, Ai server is down Please Try Again later");
                }

            }
        }
        private string GetMediaUrl(string imagePath)
        {
            // Assuming imagePath contains the relative path to the Media within the web root
            // Construct the URL based on your application's routing configuration
            string baseUrl = _mail.ServerLink; // Replace with your actual base URL
            string relativePath = imagePath.Replace(_env.WebRootPath, "").Replace("\\", "/");

            return $"{baseUrl}/{relativePath}";
        }

        private async Task<bool> RegisterPatientToAi(string PatientId, IFormFile Image)
        {
            string endpoint = "https://excited-hound-vastly.ngrok-free.app/register_patient";

            using (HttpClient httpClient = new HttpClient())
            {


                // Create multipart form-data content
                var multipartContent = new MultipartFormDataContent();

                // Add patient_id and family_member_id as query parameters
                var queryParameters = new System.Collections.Generic.Dictionary<string, string>
                {
                    { "patient_id", PatientId },

                };

                // Add the image as a stream content
                multipartContent.Add(new StreamContent(Image.OpenReadStream()), "image", "image.jpg");

                // Build query string
                var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
                foreach (var param in queryParameters)
                {
                    queryString[param.Key] = param.Value;
                }

                var fullUrl = endpoint + "?" + queryString;

                // Send the request
                var response = await httpClient.PostAsync(fullUrl, multipartContent);

                // Handle the response
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Image registered successfully.");
                    return true;
                }
                else
                {
                    Console.WriteLine($"Failed to register image. Status code: {response.StatusCode}");
                    return false;
                }
            }
        }


    }

}
