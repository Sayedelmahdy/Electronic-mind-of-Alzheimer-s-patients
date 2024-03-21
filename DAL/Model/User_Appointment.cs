using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class User_Appointment
    {
        [Key]
        public int UserAppointmentId { get; set; }
        public Appointment appointment { get; set; }
        [ForeignKey(nameof(Appointment))]
        public int AppointmentId { get; set; }
        public User user { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public Patient patient { get; set; }
        [ForeignKey(nameof(Patient))]
        public string PatientId { get; set; }
        public string State { get; set; }
    }
}
