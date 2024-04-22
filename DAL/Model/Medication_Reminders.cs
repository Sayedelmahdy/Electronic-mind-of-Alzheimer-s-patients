using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace DAL.Model
{
    public class Medication_Reminders
    {
        [Key]
        public string Reminder_ID { get; set; } = Guid.NewGuid().ToString();
        public string Medication_Name { get; set; }
        public string Dosage { get; set; }
        public MedcineType Medcine_Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public RepeatType Repeater { get; set; }
       
        public Caregiver caregiver { get; set; }
        [ForeignKey(nameof(Caregiver))]
        public string Caregiver_Id { get; set; }//اللي عملها 
        public Patient patient { get; set; }
        [ForeignKey(nameof(Patient))]
        public string Patient_Id { get; set; } // اللي معمولاله 
        [JsonIgnore]
        public ICollection<Mark_Medicine_Reminder> Mark_Medicines { get; set; }
    }
    public enum RepeatType
    {
        Once=0,
        Twice=1,
        Three_Times=2,
        Four_Times=3
    }
    public enum MedcineType
    {
        Bottle=0,
        Pill=1,
        Syringe=2,
        Tablet=3,
    }
}
