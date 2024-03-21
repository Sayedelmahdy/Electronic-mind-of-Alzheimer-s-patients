using BLL.DTOs;
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
        Task<GlobalResponse> AssignFamilyToPatient(string familyId, string patientId, string relationility);
        Task<GlobalResponse> AddPatientAsync(string familyId, RegisterDto model, string relationality);
        Task<IEnumerable<PatientDto>> GetPatientsforFamilyAsync(string familyId); 
        Task<GlobalResponse> UpdatePatientProfileAsync(string familyId,string patientId, Patient patient);
        Task<GlobalResponse> UploadPictureAsync(string familyId, string patientId, Picture picture);
        Task<GlobalResponse> DeletePictureAsync(string familyId, string patientId, int pictureId);
        Task<IEnumerable<Picture>> GetPicturesForFamilyAsync(string familyId, string patientId);
        Task<GlobalResponse> AddAppointmentAsync(string familyId, string patientId, Appointment appointment);
        Task<IEnumerable<Appointment>> GetPatientAppointmentsAsync(string familyId, string patientId);
        Task<GlobalResponse> UpdateAppointmentAsync(string familyId, string patientId, int appointmentId, Appointment appointment);
        Task<GlobalResponse> DeleteAppointmentAsync(string familyId, string patientId, int appointmentId);
        
    }
}
