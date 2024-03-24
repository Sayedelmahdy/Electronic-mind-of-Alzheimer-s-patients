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
        Task<GlobalResponse> AddMedicationReminder(string token, string patientId, MedicationReminderDto mediceneDto);
        Task<GlobalResponse> UpdateMedicationReminderAsync(string token, string reminderId, MedicationReminderDto UpdateMedicationReminderDto);
        Task<GlobalResponse> DeleteMedicationReminderAsync(string token, string reminderId);
        Task<IEnumerable<MedicationReminderDto>> GetMedicationRemindersAsync(string token, string patientId);
        Task<IEnumerable<GameScoreDto>> GetGameScoresAsync(string patientId);
        Task<string?> GetCaregiverCode(string token);
    }
}
