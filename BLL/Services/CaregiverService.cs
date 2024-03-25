using BLL.DTOs;
using BLL.DTOs.CaregiverDto;
using BLL.Helper;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BLL.Services
{
    public class CaregiverService : ICaregiverService
    {
        private readonly IBaseRepository<Patient> _patient;
        private readonly IDecodeJwt _jwtDecode;
        private readonly IBaseRepository<Medication_Reminders> _medication_Reminders;
        public CaregiverService(
            IBaseRepository<Patient> patient,
            IDecodeJwt jwtDecode,
            IBaseRepository<Medication_Reminders> medication_Reminders
            )
        {
            _patient = patient;
            _jwtDecode = jwtDecode;
            _medication_Reminders = medication_Reminders;
        }
        public async Task<string?> GetCaregiverCode(string token)
        {
            string CaregiverCode= _jwtDecode.GetUserIdFromToken(token);
            if(CaregiverCode == null )
            {
                return null;
            }
            return CaregiverCode;
        }
        public async Task<GlobalResponse> AddMedicationReminder(string token, string patientId, MedicationReminderPostDto mediceneDto)
        {
            string? caregiverId = _jwtDecode.GetUserIdFromToken(token);
            if( caregiverId == null )
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "Invalid CaregiverId"
                };
            }
            var medication = new Medication_Reminders
            {
                Medication_Name = mediceneDto.Medication_Name,
                Dosage = mediceneDto.Dosage,
                Patient_Id = patientId,
                Caregiver_Id = caregiverId,
                Repeater = mediceneDto.Repeater,
                StartDate = mediceneDto.StartDate,
                Time_Period = mediceneDto.Time_Period,
            };
           await _medication_Reminders.AddAsync(medication);
            return new GlobalResponse
            {
                HasError = false,
                message = "Medication Created Successfully ."
            };

        }

        public async Task<GlobalResponse> DeleteMedicationReminderAsync(string token, string reminderId)
        {
            string? caregiverId= _jwtDecode.GetUserIdFromToken(token);
            var reminder = await _medication_Reminders.FindAsync(i => i.Reminder_ID == reminderId);
            if (reminder == null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "Medication Reminder with This Id is Not found "
                };
            }
            await _medication_Reminders.DeleteAsync(reminder);
            return new GlobalResponse
            {
                HasError = false,
                message = "Medication Reminder Deleted Successfully ."
            };
        }

        public async Task<IEnumerable<GetPatientsDto>> GetAssignedPatientsAsync(string token)
        {
            var caregiverId  = _jwtDecode.GetUserIdFromToken(token);
            if(caregiverId == null)
            {
                return Enumerable.Empty<GetPatientsDto>();
            }
            var patients =await _patient.WhereAsync(f => f.CaregiverID == caregiverId);
            if(patients == null)
            {
                return Enumerable.Empty<GetPatientsDto>();
            }
                var res = patients.Select(s=>new GetPatientsDto
                {
                    PatientId=s.Id,
                    PatientName=s.FullName
                }).ToList();
            if(!patients.Any())
            {
                return Enumerable.Empty<GetPatientsDto>();
            }
            return res;
        }

        public async Task<IEnumerable<MedicationReminderGetDto>> GetMedicationRemindersAsync(string token, string patientId)
        {
            string? caregiverId = _jwtDecode.GetUserIdFromToken(token);
            if( caregiverId == null)
            {
                return Enumerable.Empty<MedicationReminderGetDto>();
            }
            var reminders =await _medication_Reminders.WhereAsync(s => s.Patient_Id == patientId);
            if( reminders == null)
            {
                return Enumerable.Empty<MedicationReminderGetDto> ();
            }
            var res=reminders.Select(s=> new MedicationReminderGetDto
                {
                    
                    Medication_Name=s.Medication_Name,
                    StartDate=s.StartDate,
                    Dosage=s.Dosage,
                    Repeater=s.Repeater,
                    Time_Period=s.Time_Period
                }).ToList();
            if (!res.Any())
            {
                return Enumerable.Empty<MedicationReminderGetDto>();
            }
            return res;
        }

        public async Task<GlobalResponse> UpdateMedicationReminderAsync(string token, string reminderId, MedicationReminderUpdateDto UpdateMedicationReminderDto)
        {
            string? caregiverId= _jwtDecode.GetUserIdFromToken(token);
            var medicine = await _medication_Reminders.FindAsync(r=>r.Reminder_ID == reminderId);
            if(medicine==null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "No Medication Reminder with this ID Found"
                };
            }
            medicine.Medication_Name = UpdateMedicationReminderDto.Medication_Name;
            medicine.StartDate = UpdateMedicationReminderDto.StartDate;
            medicine.Dosage= UpdateMedicationReminderDto.Dosage;
            medicine.Repeater= UpdateMedicationReminderDto.Repeater;
            medicine.Caregiver_Id=medicine.Caregiver_Id;
            await _medication_Reminders.UpdateAsync(medicine);
            return new GlobalResponse
            {
                HasError=false,
                message = "Medication Reminder Updated Successfully ."
            };
        }
    }
}
