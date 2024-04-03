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
        public PatientService
            (
            IHubContext<MedicineReminderHub> hubContext, IDecodeJwt jwtDecode,
            IBaseRepository<Patient>patient,
            IBaseRepository<Medication_Reminders>medicines ,
            IBaseRepository<Appointment>appointments,
            IBaseRepository<Family>family,
            IBaseRepository<Media>media,
             IWebHostEnvironment env,
              IOptions<Mail> Mail
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
        }

        public Task<GlobalResponse> AddPatientToSignalRGroup(string patientId)
        {
            throw new NotImplementedException();
        }
        public async Task<GetPatientProfileDto> GetPatientProfileAsync(string token)
        {
            string patientid = _jwtDecode.GetUserIdFromToken(token);

            if(patientid == null)
            {
                return new GetPatientProfileDto
                {
                    Message = "Invalid Patient ID",
                    HasError = true
                };
            }
            var patinet = await _patient.GetByIdAsync(patientid);
            if( patinet == null)
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
            if(PatientId == null)
            {
                return Enumerable.Empty<GetAppointmentDto>();
            }
            var patient = await _patient.GetByIdAsync(PatientId);
            if(patient == null)
            {
                return Enumerable.Empty<GetAppointmentDto>();
            }
          var appointments =  _appointments.Include(s=>s.family).Where(s=>s.PatientId== PatientId);
            if(appointments == null)
            {
                return Enumerable.Empty<GetAppointmentDto>();
            }
            return appointments.Select(s=> new GetAppointmentDto
            {
                AppointmentId=s.AppointmentId,
                Date=s.Date,
                Location= s.Location,
                Notes=s.Notes,
                FamilyNameWhoCreatedAppointemnt=s.family.FullName,
            }).ToList();
        }
        public async Task<IEnumerable<MedicationReminderGetDto>> GetMedicationRemindersAsync(string token)
        {
            string PatientId = _jwtDecode.GetUserIdFromToken(token);
            if (PatientId == null)
            {
                return Enumerable.Empty<MedicationReminderGetDto>();
            }
            var medicines = await _medicines.WhereAsync(s=>s.Patient_Id== PatientId);
            if(medicines == null)
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
            string PatientId = _jwtDecode.GetUserIdFromToken(token);
            if (PatientId == null)
            {
                return Enumerable.Empty<GetMediaforPatientDto>();
            }
            var patient = await _patient.GetByIdAsync(PatientId);
            if (patient == null)
            {
                return Enumerable.Empty<GetMediaforPatientDto>();
            }
            var media =await _media.Include(s => s.patient).Include(s=>s.family).Where(s => s.PatientId == PatientId).ToListAsync();
            var res = media.Select(s => new GetMediaforPatientDto
            {
                Caption = s.Caption,
                MediaUrl = GetMediaUrl(s.Image_Path),
                MediaId = s.Media_Id,
                Uploaded_date = s.Upload_Date,
                MediaExtension = s.Extension,
                FamilyNameWhoUpload=s.family.FullName
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
    }
}
