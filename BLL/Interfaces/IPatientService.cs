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
        public Task<GlobalResponse> AddPatientToSignalRGroup(string patientId);
        Task<GetPatientProfileDto> GetPatientProfileAsync(string token);
        Task<GlobalResponse> UpdateProfileAsync(string token, UpdatePatientProfileDto updatePatientProfile);
        Task<IEnumerable<GetAppointmentDto>> GetAppointmentAsync(string token);
        Task<IEnumerable<MedicationReminderGetDto>> GetMedicationRemindersAsync(string token);
    }
}
