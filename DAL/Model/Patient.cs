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
        public int MaximumDistance { get; set; }
        public string? FamilyCreatedId { get; set; }

        public int CurrentScore { get; set; }
        public int MaximumScore { get; set; }
        #region Navigation Prop
        [ForeignKey(nameof(Caregiver))]
        public string? CaregiverID { get; set; }
        public virtual ICollection<Family> families { get; set; }
        public virtual Caregiver caregiver { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<GameScore> GameScores { get; set; }
        public virtual ICollection<SecretAndImportantFile> SecretAndImportantFiles { get; set; }
        public virtual ICollection<Location> Locations { get; set; }
        #endregion
    }
}
