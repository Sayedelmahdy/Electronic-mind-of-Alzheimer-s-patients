using BLL.DTOs;
using BLL.DTOs.CaregiverDto;
using BLL.DTOs.FamilyDto;
using BLL.DTOs.PatientDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IPatientService
    {
       
        Task<GetPatientProfileDto> GetPatientProfileAsync(string token);
        Task<GlobalResponse> UpdateProfileAsync(string token, UpdatePatientProfileDto updatePatientProfile);
        Task<IEnumerable<GetAppointmentDto>> GetAppointmentAsync(string token);
        Task<IEnumerable<MedicationReminderGetDto>> GetMedicationRemindersAsync(string token);
        Task<IEnumerable<GetMediaforPatientDto>> GetMediaAsync(string token);
        Task<GlobalResponse> AddGameScoreAsync(string token, PostGameScoreDto gameScoreDto);
        Task<GetGameScoresDto> GetGameScoresAsync(string token);
        Task<GlobalResponse> AddSecretFileAsync(string token, PostSecretFileDto secretFileDto);
        Task<GlobalResponse> AskToViewSecretFileAsync(string token);
        Task<GetSecretFileDto> GetSecretFileAsync(string token);
        //Task<GetGameScoresDto> GetGameScoresAsync2(string token);
    }
}
