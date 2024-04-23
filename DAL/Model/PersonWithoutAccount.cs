using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class PersonWithoutAccount
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string FullName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string imageUrl { get; set; }
        [Required]
        public double MainLongitude { get; set; } = 0;
        [Required]
        public double MainLatitude { get; set; } = 0;
        [Required]
        public string Relationility { get; set; }
        public string? DescriptionForPatient { get; set; }

        public string PatientId { get; set; }
        public Patient Patient { get; set; }
    }
}
