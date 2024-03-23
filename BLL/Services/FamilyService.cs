using BLL.DTOs;
using BLL.DTOs.AuthenticationDto;
using BLL.DTOs.FamilyDto;
using BLL.Helper;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class FamilyService:IFamilyService
    {
        private readonly IBaseRepository<Family> _family;
        private readonly IBaseRepository<Patient> _patient;
      
        private readonly IBaseRepository<Appointment> _Appointments;
        private readonly IDecodeJwt _jwtDecode;
        private readonly IWebHostEnvironment _env;
        private readonly JWT _jwt;
        private readonly IMailService _mailService;
        private readonly IBaseRepository<Media> _Media;
        private readonly UserManager<User> _userManager;
        private readonly Mail _mail;

        public FamilyService(
            IBaseRepository<Media> Media,
            IBaseRepository<Family>family,
            IBaseRepository<Patient>patient,     
            IBaseRepository<Appointment>appointment,
            IDecodeJwt jwtDecode,  
            IWebHostEnvironment env,
              IOptions<Mail> Mail,
              IOptions<JWT> JWT,
              IMailService mailService,
            UserManager<User> user
            )
        {
            _Media = Media;
            _family = family;
            _patient = patient;
            _mail = Mail.Value;
            _Appointments=appointment;
            _jwtDecode = jwtDecode;
            _env = env;
            _jwt = JWT.Value;
            _mailService = mailService;
            _userManager = user;
        }

        
        public async Task<string?> GetPatientCode(string token)
        {
            string? FamilyId = _jwtDecode.GetUserIdFromToken(token);
            if (FamilyId == null)
            {
              return  null;
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
            var Family = await _family.Include(p => p.patient).FirstOrDefaultAsync(p=>p.Id==FamilyId);
            if (Family.patient.FamilyCreatedId!=FamilyId)
            {
                return new GetPatientProfile
                {
                    ErrorAppear = true,
                    Message = "This user doesn't Create this Patient to update his profile",
                };
            }
            var patient = await _patient.GetByIdAsync(Family.patient.Id);
            patient.Age=updatePatientProfileDto.Age;
            patient.PhoneNumber=updatePatientProfileDto.PhoneNumber;
            patient.DiagnosisDate = updatePatientProfileDto.DiagnosisDate.ToDateTime(TimeOnly.MinValue);
            patient.MaximumDistance = updatePatientProfileDto.MaximumDistance;
            await  _patient.UpdateAsync(patient);
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
            if (_family.GetById(FamilyId).PatientId==null)
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
                FamilyCreatedId=FamilyId,
                MaximumDistance = addPatientDto.MaximumDistance
            };
            var Created = await _userManager.CreateAsync(patient, addPatientDto.Password);
            if (Created == null || !Created.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in Created.Errors)
                    errors += $"{error.Description},";
                return new GlobalResponse {
                    
                    HasError= true,
                    message = errors };
            }
            await _userManager.AddToRoleAsync(patient, "patient");
            await _userManager.UpdateAsync(patient);

            var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(patient);

            var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
            var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

            string url = $"{_mail.ServerLink}/api/Authentication/confirmemail?userid={patient.Id}&token={validEmailToken}";
            htmlContent = htmlContent.Replace("{FullName}", patient.FullName).Replace("{url}", url);
            var res=  await _mailService.SendEmailAsync(patient.Email, _mail.FromMail, _mail.Password, "Confirm your email", htmlContent);
            var family = await _family.GetByIdAsync(FamilyId);
            if (!res || family==null)
            {
             await  _userManager.DeleteAsync(patient);
                return new GlobalResponse
                {
                    HasError = true,
                    message = "something went wrong now get back after sometime"
                };
            }
            family.Relationility = addPatientDto.relationality;
            family.PatientId = patient.Id;
            await _family.UpdateAsync(family);
            return new GlobalResponse
            {
                HasError = false,
                message = "Patient Added Succsfully"
            };
        }

        public async Task<GlobalResponse> AssignPatientToFamily(string token ,AssignPatientDto assignPatientDto)
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
            if (family.PatientId!=null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "family alredy have assigned to patient"
                };

            }
            if (family==null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "invalid Family ID"
                };
            }
            if (await _patient.GetByIdAsync(assignPatientDto.PatientCode)==null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "invalid Patient Code"
                };
            }
            family.PatientId = assignPatientDto.PatientCode;
            family.Relationility = assignPatientDto.relationility;
            await  _family.UpdateAsync(family) ;
            return new GlobalResponse
            {
                HasError = false,
                message = "Patient Assigned to this family succesfully"
            };
        }
        public async Task<IEnumerable<GetAppointmentDto>> GetPatientAppointmentsAsync(string token)
        {
            string? FamilyId = _jwtDecode.GetUserIdFromToken(token);
            if (FamilyId == null)
            {
               return Enumerable.Empty<GetAppointmentDto>();
            }
            var family =await _family.GetByIdAsync(FamilyId);
            if (family==null || family.PatientId==null ) {
            
            return Enumerable.Empty<GetAppointmentDto>();
            }
            var appointment = _Appointments.Where(p => p.PatientId == family.PatientId).ToList().Select(p => new GetAppointmentDto
            {
                AppointmentId=p.AppointmentId,
                Date=p.Date,
                Location=p.Location,
                Notes=p.Notes,
                FamilyNameWhoCreatedAppointemnt=_family.GetById(p.FamilyId).FullName,
                CanDeleted = (p.FamilyId==FamilyId)?true:false
            });
            return appointment;
        }
        public async Task<GlobalResponse> AddAppointmentAsync(string token , AddAppointmentDto addAppointmentDto)
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
            if (family.PatientId== null)
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
            await _Appointments.AddAsync(appointment);
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
            if (Appointemnt.FamilyId!=FamilyId)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "This user didn't Create this appointment so he cann't delete it"
                };

            }
           
            await _Appointments.DeleteAsync(Appointemnt);
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
                MediaExtension= p.Extension
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
                Image_Path = Path.Combine(_env.WebRootPath, filePath),
                Upload_Date = DateTime.UtcNow,
                Extension = Path.GetExtension(addMediaDto.MediaFile.FileName),
                FamilyId = FamilyId,
                PatientId = PateintId,

            };
              await  _Media.AddAsync(media);
                return new GlobalResponse
                {
                    HasError = false,
                    message = "Media Added Succesfully"
                };
            }


        private string GetMediaUrl(string imagePath)
        {
            // Assuming imagePath contains the relative path to the Media within the web root
            // Construct the URL based on your application's routing configuration
            string baseUrl = _mail.ServerLink; // Replace with your actual base URL
            string relativePath = imagePath.Replace(_env.WebRootPath, "").Replace("\\", "/");

            return $"{baseUrl}/{relativePath}";
        }
    }

}
