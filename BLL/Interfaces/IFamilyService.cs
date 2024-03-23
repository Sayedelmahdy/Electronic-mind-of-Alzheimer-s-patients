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
        Task<GlobalResponse> AddAppointmentAsync(string token , AddAppointmentDto addAppointmentDto);
        Task<IEnumerable<GetAppointmentDto>> GetPatientAppointmentsAsync(string token);
        Task<GlobalResponse> DeleteAppointmentAsync(string token , string AppointmentId);
        
    }
}
