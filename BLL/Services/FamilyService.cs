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
        private readonly IBaseRepository<FamilyPatient> _familyPatient;
        private readonly IBaseRepository<Appointment> _Appointments;
        private readonly IBaseRepository<User_Appointment> _User_Appointments;
        private readonly IBaseRepository<Picture> _pictures;
        private readonly UserManager<User> _User;

        public FamilyService(
            IBaseRepository<Picture> pictures,
            IBaseRepository<Family>family,
            IBaseRepository<Patient>patient,
            IBaseRepository<FamilyPatient>familypatient,
            IBaseRepository<Appointment>appointment,
            IBaseRepository<User_Appointment>user_appointment,
            UserManager<User> user
            )
        {
            _pictures = pictures;
            _family = family;
            _patient = patient;
            _familyPatient = familypatient;
            _Appointments=appointment;
            _User_Appointments=user_appointment;
            _User = user;
        }

       
    }
}
