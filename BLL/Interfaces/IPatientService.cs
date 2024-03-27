using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    internal interface IPatientService
    {
        public Task<GlobalResponse> AddPatientToSignalRGroup(string patientId);
    }
}
