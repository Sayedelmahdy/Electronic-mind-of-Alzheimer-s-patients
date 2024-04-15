using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.CaregiverDto
{
    public class MedicationReminderPostDto
    {
        [Required]
        public string Medication_Name { get; set; }
        [Required]
        public string Dosage { get; set; }
        [Required]
        public MedcineType MedcineType { get; set; }
        [Required]
        public RepeatType Repeater { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }

    }
}
