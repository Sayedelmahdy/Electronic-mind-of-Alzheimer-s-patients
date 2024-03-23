using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.CaregiverDto
{
    public class MedicationReminderDto
    {
        public string Reminder_ID { get; set; } 
        
        public string Medication_Name { get; set; }
        
        public string Dosage { get; set; }
        public DateTime StartDate { get; set; }
        
        public RepeatType Repeater { get; set; }
        
        public string Time_Period { get; set; }
    }
}
