using BLL.DTOs;
using BLL.DTOs.CaregiverDto;
using BLL.DTOs.FamilyDto;
using BLL.DTOs.PatientDto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IPatientService
    {
       
        Task<GetPatientProfileDto> GetPatientProfileAsync(string token);
        Task<IEnumerable<GetFamiliesDto>> GetFamiliesAsync(string token);
        Task<GetFamilyLocationDto?> GetFamilyLocation(string token, string familyId);
        Task<GlobalResponse> UpdateProfileAsync(string token, UpdateMyProfileDto updatePatientProfile);
        Task<IEnumerable<GetAppointmentDto>> GetAppointmentAsync(string token);
        Task<IEnumerable<MedicationReminderGetDto>> GetMedicationRemindersAsync(string token);
        Task<GlobalResponse> MarkMedicationReminderAsync(string token, MarkMedictaionDto markMedictaionDto);
        Task<IEnumerable<GetMediaforPatientDto>> GetMediaAsync(string token);
        Task<GlobalResponse> AddGameScoreAsync(string token, PostGameScoreDto gameScoreDto);
        Task<GetGameScoresDto?> GetGameScoresAsync(string token);
        Task<GlobalResponse> AddSecretFileAsync(string token, PostSecretFileDto secretFileDto);
        Task<CurrentAndMaxScoreDto?> GetRecommendedScoreAsync(string token);
        Task<GlobalResponse> AskToViewSecretFileAsync(string token, IFormFile videoFile);
        Task<GetAllSecretFileDto?> GetSecretFilesAsync(string token);
        Task<GlobalResponse> ApproveVideoAsync(string fileId);
        
    }
}
