using BLL.DTOs;
using BLL.DTOs.AuthenticationDto;
using BLL.DTOs.FamilyDto;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IFamilyService
    {
        Task<GlobalResponse> AssignPatientToFamily(string token , AssignPatientDto assignPatientDto);
        Task<string?> GetPatientCode(string token);
        Task<GlobalResponse> AddPatientAsync(string token,AddPatientDto addPatientDto);
        Task<GetPatientProfile> GetPatientProfile(string token);
        Task<GetPatientProfile> UpdatePatientProfileAsync(string token , UpdatePatientProfileDto updatePatientProfileDto);
        Task<IEnumerable<GetPictureDto>> GetPicturesForFamilyAsync(string token);
        Task<GlobalResponse> UploadPictureAsync(string token, AddPictureDto addPictureDto) ;
        Task<GlobalResponse> AddAppointmentAsync(string familyId, string patientId, Appointment appointment);
        Task<IEnumerable<Appointment>> GetPatientAppointmentsAsync(string familyId, string patientId);
        Task<GlobalResponse> DeleteAppointmentAsync(string familyId, string patientId, int appointmentId);
        
    }
}
