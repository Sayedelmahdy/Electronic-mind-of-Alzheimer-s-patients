using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class Patient : User
    {
        public DateTime? DiagnosisDate { get; set; }



        #region Navigation Prop
        [ForeignKey(nameof(Caregiver))]
        public string? CaregiverID { get; set; }
        public virtual ICollection<Family> Families { get; set; }
        public virtual Caregiver Caregiver { get; set; }
        #endregion
    }
}
