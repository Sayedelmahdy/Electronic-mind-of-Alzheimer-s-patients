using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class Location
    {
        [Key]
        public string LocationId { get; set; } = Guid.NewGuid().ToString();
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow.AddHours(2);

        public Patient patient { get; set; }
        [ForeignKey(nameof(Patient))]
        public string PatientId { get; set; }
    }
}
