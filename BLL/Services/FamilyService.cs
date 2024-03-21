using BLL.DTOs;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class FamilyService:IFamilyService
    {
        private readonly IBaseRepository<Family> _family;
        private readonly IBaseRepository<Patient> _patient;
      
        private readonly IBaseRepository<Appointment> _Appointments;
      
        private readonly IBaseRepository<Picture> _pictures;
        private readonly UserManager<User> _User;

        public FamilyService(
            IBaseRepository<Picture> pictures,
            IBaseRepository<Family>family,
            IBaseRepository<Patient>patient,
            
            IBaseRepository<Appointment>appointment,
         
            UserManager<User> user
            )
        {
            _pictures = pictures;
            _family = family;
            _patient = patient;
           
            _Appointments=appointment;
           
            _User = user;
        }

        public Task<GlobalResponse> AddAppointmentAsync(string familyId, string patientId, Appointment appointment)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse> AddPatientAsync(string familyId, RegisterDto model, string relationality)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse> AssignFamilyToPatient(string familyId, string patientId, string relationility)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse> DeleteAppointmentAsync(string familyId, string patientId, int appointmentId)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse> DeletePictureAsync(string familyId, string patientId, int pictureId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Appointment>> GetPatientAppointmentsAsync(string familyId, string patientId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PatientDto>> GetPatientsforFamilyAsync(string familyId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Picture>> GetPicturesForFamilyAsync(string familyId, string patientId)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse> UpdateAppointmentAsync(string familyId, string patientId, int appointmentId, Appointment appointment)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse> UpdatePatientProfileAsync(string familyId, string patientId, Patient patient)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse> UploadPictureAsync(string familyId, string patientId, Picture picture)
        {
            throw new NotImplementedException();
        }
    }
}
