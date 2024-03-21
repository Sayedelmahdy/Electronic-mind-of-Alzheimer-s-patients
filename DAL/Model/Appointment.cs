using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
       
      

        #region Navigation Prop
        [ForeignKey(nameof(Family))]
        public string FamilyId { get; set; } // الى هيعملها 
        public Family family { get; set; }
        [ForeignKey(nameof(Patient))]
        public string PatientId { get; set; } // اتعملت لمين 
        public Patient patient { get; set; }    

        #endregion
    }
}
