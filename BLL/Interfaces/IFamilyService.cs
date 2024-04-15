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
      
        Task<IEnumerable<LocationDto>> GetPatientLocationsTodayAsync(string token);
        Task<GlobalResponse> AssignPatientToCaregiver (string token,string CaregiverCode);
        Task<GlobalResponse> AddPatientAsync(string token,AddPatientDto addPatientDto);
        Task<GetPatientProfile> GetPatientProfile(string token);
        Task<GetPatientProfile?> UpdatePatientProfileAsync(string token , UpdatePatientProfileDto updatePatientProfileDto);
        Task<IEnumerable<GetMediaDto>> GetMediaForFamilyAsync(string token);
        Task<GlobalResponse> UploadMediaAsync(string token, AddMediaDto addPictureDto) ;
        Task<GlobalResponse> AddAppointmentAsync(string token , AddAppointmentDto addAppointmentDto);
        Task<IEnumerable<GetAppointmentDto>> GetPatientAppointmentsAsync(string token);
        Task<GlobalResponse> DeleteAppointmentAsync(string token , string AppointmentId);
        Task<IEnumerable<GetReportDto>> GetPatientReportsAsync(string token);
        
    }
}
