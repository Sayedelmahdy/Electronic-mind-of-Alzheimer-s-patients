using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.CaregiverDto
{
    public class MedicationReminderUpdateDto
    {
        public string Medication_Name { get; set; }

        public string Dosage { get; set; }
        public MedcineType MedcineType { get; set; }
        public RepeatType Repeater { get; set; }
        public DateTime StartDate { get; set; }



        public DateTime EndDate { get; set; }
    }
}
