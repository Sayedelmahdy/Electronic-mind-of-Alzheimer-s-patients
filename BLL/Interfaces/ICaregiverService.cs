using BLL.DTOs;
using BLL.DTOs.AuthenticationDto;
using BLL.DTOs.CaregiverDto;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ICaregiverService
    {
        Task<IEnumerable<GetPatientsDto>> GetAssignedPatientsAsync(string token);
        Task<string?> GetCaregiverCode(string token);
        Task<GlobalResponse> AddMedicationReminder(string token, string patientId, MedicationReminderPostDto mediceneDto);
        Task<GlobalResponse> UpdateMedicationReminderAsync(string token, string reminderId, MedicationReminderUpdateDto UpdateMedicationReminderDto);
        Task<GlobalResponse> DeleteMedicationReminderAsync(string token, string reminderId);
        Task<IEnumerable<MedicationReminderDto>> GetMedicationRemindersAsync(string token, string patientId);
        Task<IEnumerable<GameScoreDto>> GetGameScoresAsync(string patientId);
        Task<GlobalResponse> DeleteReport(string token, string reportId);
        Task<IEnumerable<GetReportDto>> getallReport(string token, string patientid);
        Task<GlobalResponse> CreateReportCardAsync(string token, ReportCardDto reportCardDto);
        Task<IEnumerable<MedicationReminderGetDto>> GetMedicationRemindersAsync(string token, string patientId);

    }
}
