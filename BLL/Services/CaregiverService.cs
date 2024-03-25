﻿using BLL.DTOs;
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
        private readonly IBaseRepository<GameScore> _gameScoreRepository;
        private readonly IBaseRepository<Caregiver> _caregiver;
        private readonly IBaseRepository<Report> _report;
        public CaregiverService(
            IBaseRepository<Patient> patient,
            IDecodeJwt jwtDecode,
            IBaseRepository<Medication_Reminders> medication_Reminders,
            IBaseRepository<GameScore> gameScoreRepository,
            IBaseRepository<Caregiver> caregiver,
            IBaseRepository<Report> report
            )
        {
            _patient = patient;
            _jwtDecode = jwtDecode;
            _medication_Reminders = medication_Reminders;
            _gameScoreRepository = gameScoreRepository;
            _caregiver = caregiver;
            _report = report;
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
                    ReminderId=s.Reminder_ID,
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
            medicine.Time_Period = UpdateMedicationReminderDto.Time_Period;
            await _medication_Reminders.UpdateAsync(medicine);
            return new GlobalResponse
            {
                HasError=false,
                message = "Medication Reminder Updated Successfully ."
            };
        }
        // game method
        public async Task<IEnumerable<GameScoreDto>> GetGameScoresAsync(string patientId)
        {

            var patient = await _patient.FindAsync(p => p.Id == patientId);

            if (patient == null)
            { 
                return Enumerable.Empty<GameScoreDto>();
            }
            var gameScores = await _gameScoreRepository.WhereAsync(gs => gs.PatientId == patientId);

            if (gameScores == null || !gameScores.Any())
            {

                return Enumerable.Empty<GameScoreDto>();
            }
            var gameScoreDtos = gameScores.Select(gs => new GameScoreDto
            {
                GameScoreId = gs.GameScoreId,
                GameScoreName = gs.GameScoreName,
                DifficultyGame = gs.DifficultyGame,
                PatientScore = gs.PatientScore,
                MaxScore = gs.MaxScore
            });

            return gameScoreDtos;
        }
        // report card

        public async Task<GlobalResponse> CreateReportCardAsync(string token, ReportCardDto reportCardDto)
        {
            // Check if caregiverId is valid or extract it from token based on your application logic
            string? caregiverId = _jwtDecode.GetUserIdFromToken(token);
            var patient = await _patient.GetByIdAsync(reportCardDto.patientid);
            if (patient == null || patient.CaregiverID != caregiverId)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "failed to find any patients"

                };
            }


            var reportCard = new Report
            {
                FromDate = reportCardDto.FromDate.ToDateTime(TimeOnly.MinValue),
                ToDate = reportCardDto.ToDate.ToDateTime(TimeOnly.MinValue),
                ReportContent = reportCardDto.ReportContent,
                CaregiverId = caregiverId,
                PatientId = reportCardDto.patientid
            };

            // Add the report card entity to the database using Entity Framework Core
            await _report.AddAsync(reportCard);

            
            return new GlobalResponse {
                HasError =false,
                message = "Report Created Succuessfully ." 
            };
        }
        public async Task<IEnumerable<GetReportDto>> getallReport(string token, string patientid )
        {
            string? caregiverId = _jwtDecode.GetUserIdFromToken(token);
            if (string.IsNullOrEmpty(caregiverId))
            {
                return Enumerable.Empty<GetReportDto>();
            }

            var allreport = await _report.WhereAsync(s =>s.CaregiverId == caregiverId && s.PatientId == patientid);

            if (allreport.Count() ==0 || !allreport.Any())
            {
                return Enumerable.Empty<GetReportDto>();
            }

            var result = allreport.Select(d=> new GetReportDto 
            {
                FromDate = d.FromDate.Date.ToShortDateString(),
                ReportContent = d.ReportContent,
                ToDate = d.ToDate.Date.ToShortDateString(),
                ReportId = d.ReportId,
            }).ToList();
            return result;


        }
        public async Task<GlobalResponse> DeleteReport(string token, string reportId)
        {
            string? caregiverId = _jwtDecode.GetUserIdFromToken(token);

            if (string.IsNullOrEmpty(caregiverId))
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "The caregiver is not found"
                };
            }

            var reportToDelete = await _report.FindAsync(r => r.CaregiverId == caregiverId && r.ReportId == reportId);

            if (reportToDelete == null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "The report does not exist or you don't have permission to delete it"
                };
            }

            await _report.DeleteAsync(reportToDelete);

            return new GlobalResponse
            {
                HasError = false,
                message = "The report has been deleted successfully"
            };
        }






    }
}