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
        private readonly IMailService _mailService;
        private readonly IBaseRepository<Picture> _pictures;
        private readonly UserManager<User> _userManager;
        private readonly Mail _mail;

        public FamilyService(
            IBaseRepository<Picture> pictures,
            IBaseRepository<Family>family,
            IBaseRepository<Patient>patient,     
            IBaseRepository<Appointment>appointment,
            IDecodeJwt jwtDecode,  
            IWebHostEnvironment env,
              IOptions<Mail> Mail,
              IMailService mailService,
            UserManager<User> user
            )
        {
            _pictures = pictures;
            _family = family;
            _patient = patient;
            _mail = Mail.Value;
            _Appointments=appointment;
            _jwtDecode = jwtDecode;
            _env = env;
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
                DiagnosisDate = addPatientDto.DiagnosisDate.ToDateTime(TimeOnly.MinValue),
                PhoneNumber = addPatientDto.PhoneNumber,
                FamilyCreatedId=FamilyId,
                MaximumDistance = addPatientDto.MaximumDistance
            };
           await _userManager.CreateAsync(patient, addPatientDto.Password);
            await _userManager.AddToRoleAsync(patient, "Patient");
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
        public Task<IEnumerable<Appointment>> GetPatientAppointmentsAsync(string familyId, string patientId)
        {
            throw new NotImplementedException();
        }
        public Task<GlobalResponse> AddAppointmentAsync(string familyId, string patientId, Appointment appointment)
        {
            throw new NotImplementedException();
        }
        public Task<GlobalResponse> DeleteAppointmentAsync(string familyId, string patientId, int appointmentId)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<GetPictureDto>> GetPicturesForFamilyAsync(string token)
        {

            string? FamilyId = _jwtDecode.GetUserIdFromToken(token);
            if (FamilyId == null)
            {
               return Enumerable.Empty<GetPictureDto>();
            }
            var pictures = await _pictures.Include(p => p.patient).Where(p => p.FamilyId == FamilyId).ToListAsync();
            var res = pictures.Select(p => new GetPictureDto
            {

                Caption = p.Caption,
                Picture = System.IO.File.ReadAllBytes(p.Image_Path),
                PictureId = p.Picture_Id,
                Uploaded_date = p.Upload_Date,
            }).ToList();
            return res;
        }

     
      

        public async Task<GlobalResponse> UploadPictureAsync(string token, AddPictureDto addPictureDto)
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
            string PictureId = Guid.NewGuid().ToString();

            string FilePath = @$"{PateintId}\{FamilyId}_{PictureId}";
                if (!Directory.Exists(_env.WebRootPath + PateintId))
                {
                    Directory.CreateDirectory(_env.WebRootPath + PateintId);

                }
                using (FileStream filestream = System.IO.File.Create(_env.WebRootPath + FilePath))
                {
                     addPictureDto.Picture.CopyTo(filestream);
                    filestream.Flush();
                }
                Picture picture = new Picture
                {
                    Picture_Id=PictureId,
                    Caption = addPictureDto.Caption,
                    Image_Path = _env.WebRootPath + FilePath,
                    Upload_Date = DateTime.UtcNow,
                    FamilyId = FamilyId,
                    PatientId = PateintId,

                };
              await  _pictures.AddAsync(picture);
                return new GlobalResponse
                {
                    HasError = false,
                    message = "Picture Added Succesfully"
                };
            }

      
    }
}
