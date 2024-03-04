using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class FamilyPatient
    {
        [ForeignKey(nameof(Family))]
        public string FamilyId { get; set; }
        public Family Family { get; set; }

        [ForeignKey(nameof(Patient))]
        public string PatientId { get; set; }
        public Patient Patient { get; set; }

        public string? Relationility { get; set; }
    }
}
