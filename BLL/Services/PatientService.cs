using BLL.DTOs;
using BLL.DTOs.CaregiverDto;
using BLL.DTOs.FamilyDto;
using BLL.DTOs.PatientDto;
using BLL.Helper;
using BLL.Hubs;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class PatientService : IPatientService
    {
        public IHubContext<MedicineReminderHub> _hubContext { get; }
        private readonly IBaseRepository<Patient> _patient;
        private readonly IBaseRepository<Medication_Reminders> _medicines;
        private readonly IBaseRepository<Appointment> _appointments;
        private readonly IBaseRepository<Family> _family;
        private readonly IDecodeJwt _jwtDecode;
        private readonly IBaseRepository<Media> _media;
        private readonly Mail _mail;
        private readonly IWebHostEnvironment _env;
        private readonly IBaseRepository<SecretAndImportantFile> _secret;
        private readonly IMailService _mailService;
        public PatientService
            (
            IHubContext<MedicineReminderHub> hubContext, IDecodeJwt jwtDecode,
            IBaseRepository<Patient> patient,
            IBaseRepository<Medication_Reminders> medicines,
            IBaseRepository<Appointment> appointments,
            IBaseRepository<Family> family,
            IBaseRepository<Media> media,
             IWebHostEnvironment env,
              IOptions<Mail> Mail,
              IMailService mailService,
              IBaseRepository<SecretAndImportantFile> secret
            )
        {
            _hubContext = hubContext;
            _jwtDecode = jwtDecode;
            _patient = patient;
            _medicines = medicines;
            _appointments = appointments;
            _family = family;
            _media = media;
            _mail = Mail.Value;
            _env = env;
            _secret = secret;
            _mailService = mailService;
        }


        public async Task<GetPatientProfileDto> GetPatientProfileAsync(string token)
        {
            string? patientid = _jwtDecode.GetUserIdFromToken(token);

            if (patientid == null)
            {
                return new GetPatientProfileDto
                {
                    Message = "Invalid Patient ID",
                    HasError = true
                };
            }
            var patinet = await _patient.GetByIdAsync(patientid);
            if (patinet == null)
            {
                return new GetPatientProfileDto
                {
                    Message = "Invalid Patient ID",
                    HasError = true
                };
            }
            return new GetPatientProfileDto
            {
                PatientId = patientid,
                FullName = patinet.FullName,
                Age = patinet.Age,
                DiagnosisDate = patinet.DiagnosisDate,
                PhoneNumber = patinet.PhoneNumber,
                Message = "Welcome To Your Profile",
                HasError = false
            };
        }
        public async Task<IEnumerable<GetAppointmentDto>> GetAppointmentAsync(string token)
        {
            string PatientId = _jwtDecode.GetUserIdFromToken(token);
            if (PatientId == null)
            {
                return Enumerable.Empty<GetAppointmentDto>();
            }
            var patient = await _patient.GetByIdAsync(PatientId);
            if (patient == null)
            {
                return Enumerable.Empty<GetAppointmentDto>();
            }
            var appointments = _appointments.Include(s => s.family).Where(s => s.PatientId == PatientId);
            if (appointments == null)
            {
                return Enumerable.Empty<GetAppointmentDto>();
            }
            return appointments.Select(s => new GetAppointmentDto
            {
                AppointmentId = s.AppointmentId,
                Date = s.Date,
                Location = s.Location,
                Notes = s.Notes,
                FamilyNameWhoCreatedAppointemnt = s.family.FullName,
            }).ToList();
        }
        public async Task<IEnumerable<MedicationReminderGetDto>> GetMedicationRemindersAsync(string token)
        {
            string PatientId = _jwtDecode.GetUserIdFromToken(token);
            if (PatientId == null)
            {
                return Enumerable.Empty<MedicationReminderGetDto>();
            }
            var medicines = await _medicines.WhereAsync(s => s.Patient_Id == PatientId);
            if (medicines == null)
            {
                return Enumerable.Empty<MedicationReminderGetDto>();
            }
            return medicines.Select(s => new MedicationReminderGetDto
            {
                Medication_Name = s.Medication_Name,
                ReminderId = s.Reminder_ID,
                StartDate = s.StartDate,
                Dosage = s.Dosage,
                Repeater = s.Repeater,
                Time_Period = s.Time_Period,
            }).ToList();
        }
        public async Task<GlobalResponse> UpdateProfileAsync(string token, UpdatePatientProfileDto updatePatientProfile)
        {
            string PatientId = _jwtDecode.GetUserIdFromToken(token);
            if (PatientId == null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "Invalid patient Id !"
                };
            }
            var patient = await _patient.GetByIdAsync(PatientId);
            if (patient == null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "No Patient With this ID!"
                };
            }
            patient.Age = updatePatientProfile.Age;
            patient.PhoneNumber = updatePatientProfile.PhoneNumber;
            patient.DiagnosisDate = updatePatientProfile.DiagnosisDate.ToDateTime(TimeOnly.MinValue);
            patient.MaximumDistance = updatePatientProfile.MaximumDistance;
            await _patient.UpdateAsync(patient);
            return new GlobalResponse
            {
                HasError = false,
                message = "Profile Updated Successfully :D"
            };
        }
        public async Task<IEnumerable<GetMediaforPatientDto>> GetMediaAsync(string token)
        {
            string? PatientId = _jwtDecode.GetUserIdFromToken(token);
            if (PatientId == null)
            {
                return Enumerable.Empty<GetMediaforPatientDto>();
            }
            var patient = await _patient.GetByIdAsync(PatientId);
            if (patient == null)
            {
                return Enumerable.Empty<GetMediaforPatientDto>();
            }
            var media = await _media.Include(s => s.patient).Include(s => s.family).Where(s => s.PatientId == PatientId).ToListAsync();
            var res = media.Select(s => new GetMediaforPatientDto
            {
                Caption = s.Caption,
                MediaUrl = GetMediaUrl(s.Image_Path),
                MediaId = s.Media_Id,
                Uploaded_date = s.Upload_Date,
                MediaExtension = s.Extension,
                FamilyNameWhoUpload = s.family.FullName
            }).ToList();
            return res;
        }
        private string GetMediaUrl(string imagePath)
        {
            // Assuming imagePath contains the relative path to the Media within the web root
            // Construct the URL based on your application's routing configuration
            string baseUrl = _mail.ServerLink; // Replace with your actual base URL
            string relativePath = imagePath.Replace(_env.WebRootPath, "").Replace("\\", "/");

            return $"{baseUrl}/{relativePath}";
        }

        public Task<GlobalResponse> AddGameScoreAsync(string token, PostGameScoreDto gameScoreDto)
        {
            throw new NotImplementedException();
        }

        public Task<GetGameScoresDto> GetGameScoresDto(string token)
        {
            throw new NotImplementedException();
        }

        public async Task<GlobalResponse> AddSecretFileAsync(string token, PostSecretFileDto secretFileDto)
        {
            try
            {
                string patientId = _jwtDecode.GetUserIdFromToken(token);
                if (patientId == null)
                {
                    return new GlobalResponse
                    {
                        HasError = true,
                        message = "Invalid patient ID"
                    };
                }           
                string fileID = Guid.NewGuid().ToString();
                string filepath = Path.Combine( patientId, $"{patientId }_{fileID}{Path.GetExtension(secretFileDto.FileName)}");
                string directorypath = Path.Combine(_env.WebRootPath, patientId,"SecretFiles");
                if (!Directory.Exists(directorypath))
                {
                    Directory.CreateDirectory(directorypath);
                }
                using (FileStream filestream = File.Create(Path.Combine(_env.WebRootPath, filepath)))
                {
                    await secretFileDto.File.CopyToAsync(filestream);
                    filestream.Flush();
                }
                
                
                var secretFile = new SecretAndImportantFile
                {
                    File_Id = fileID,
                    FileName = secretFileDto.FileName,
                    File_Description = secretFileDto.File_Description,
                    DocumentPath = Path.Combine(_env.WebRootPath, filepath),
                    permissionEndDate = DateTime.Now.AddDays(1),
                    hasPermission = true
                   
                };

                
                await _secret.AddAsync(secretFile);

                return new GlobalResponse
                {
                    HasError = false,
                    message = "Secret file added successfully"
                };
            }
            catch
            {
              
                return new GlobalResponse
                {
                    HasError = true,
                    message = "An error occurred while adding the secret file",
                  
                };
            }
        }



        public async Task<GlobalResponse> AskToViewSecretFileAsync(string token, IFormFile videoFile)
        {
            try
            {
                string patientId = _jwtDecode.GetUserIdFromToken(token);
                
                if (patientId == null)
                {
                    return new GlobalResponse
                    {
                        HasError = true,
                        message = "Invalid patient ID"
                    };
                }

                // Check if the video file is null or empty
                if (videoFile == null || videoFile.Length == 0)
                {
                    return new GlobalResponse
                    {
                        HasError = true,
                        message = "Video file is empty"
                    };
                }

                // Save the video file and get the File_Id
                string filepath = await SaveVideoAsync(patientId, videoFile);
                var patient = await _patient.GetByIdAsync(patientId);
                if (filepath != null)
                {
                    // Video saved successfully, return the File_Id for further processing
                  var result =   await _mailService.SendEmailAsync("hazemzizo@gmail.com", _mail.FromMail, _mail.Password, $"Patient{patient.FullName} has sent a request to view his secret files", $"Video uploaded successfully for review. video Link: {GetMediaUrl(filepath)}");
                    if (result == true)
                    {
                        return new GlobalResponse
                        {
                            HasError = false,
                            message = $"Video uploaded successfully for review. File ID: {filepath}"
                        };
                    }
                    else
                    {
                        return new GlobalResponse
                        {
                            HasError = true,
                            message = "Failed to save the video, Try again later"
                        };
                    }
                   
                }
                else
                {
                    return new GlobalResponse
                    {
                        HasError = true,
                        message = "Failed to save the video"
                    };
                }
            }
            catch 
            {
                // Handle exceptions
                return new GlobalResponse
                {
                    HasError = true,
                    message = "An error occurred " 
                };
            }
        }


        private async Task<string> SaveVideoAsync(string patientId, IFormFile videoFile)
        {
            try
            {
                string fileID = Guid.NewGuid().ToString();
                string fileName = $"{patientId}_{fileID}{Path.GetExtension(videoFile.FileName)}";

                string directoryPath = Path.Combine(_env.WebRootPath, patientId,"VideoForReview");

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string filePath = Path.Combine(directoryPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await videoFile.CopyToAsync(stream);
                    stream.Flush(); 
                }
                return filePath;
            }
            catch 
            {
                return "Error saving video file";
            }
        }
        public async Task<GlobalResponse> ApproveVideoAsync(string fileId)
        {
            try
            {
                var secretFile = await _secret.FindAsync(s => s.File_Id == fileId);
                if (secretFile == null)
                {
                    return new GlobalResponse
                    {
                        HasError = true,
                        message = "Secret file not found"
                    };
                }
                secretFile.hasPermission = true;
                secretFile.permissionEndDate = DateTime.Now.AddDays(1);
                
                await _secret.UpdateAsync(secretFile);

                return new GlobalResponse
                {
                    HasError = false,
                    message = "Secret file permission updated successfully"
                };
            }
            catch 
            {
                
                return new GlobalResponse
                {
                    HasError = true,
                    message = "there is an error in approving the secret file"
                };
            }
        }
        public async Task<IEnumerable<GetSecretFIleDTO>> GetSecretFilesAsync(string token)
        {
            try
            {
                
                if (string.IsNullOrEmpty(token))
                {
                    return Enumerable.Empty<GetSecretFIleDTO>();
                }

                string patientId = _jwtDecode.GetUserIdFromToken(token);

                if (string.IsNullOrEmpty(patientId))
                {
                    return Enumerable.Empty<GetSecretFIleDTO>();
                }

                var patient = await _patient.GetByIdAsync(patientId);

                if (patient == null)
                {
                    return Enumerable.Empty<GetSecretFIleDTO>();
                }

                var secretFiles = await _secret.WhereAsync(s => s.PatientId == patientId && s.hasPermission);

                if (secretFiles == null || !secretFiles.Any())
                {
                    return Enumerable.Empty<GetSecretFIleDTO>();
                }

                var result = secretFiles.Select(s => new GetSecretFIleDTO
                {
                    SecretId = s.File_Id,
                    FileName = s.FileName,
                    File_Description = s.File_Description,
                    DocumentUrl = GetMediaUrl(s.DocumentPath),
                    HasError = false
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in GetSecretFilesAsync: {ex.Message}");
                return Enumerable.Empty<GetSecretFIleDTO>();
            }
        }



    }
}
