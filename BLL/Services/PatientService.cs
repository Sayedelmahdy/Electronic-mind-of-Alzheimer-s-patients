using BLL.DTOs;
using BLL.DTOs.FamilyDto;
using BLL.Hubs;
using BLL.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class PatientService : IPatientService
    {
        public IHubContext<MedicineReminderHub> _hubContext { get; }
        private readonly IDecodeJwt _jwtDecode;
        public PatientService
            (
            IHubContext<MedicineReminderHub> hubContext, IDecodeJwt jwtDecode
            )
        {
            _hubContext = hubContext;
            _jwtDecode = jwtDecode;
        }

        public Task<GlobalResponse> AddPatientToSignalRGroup(string patientId)
        {
            throw new NotImplementedException();
        }
    }
}
