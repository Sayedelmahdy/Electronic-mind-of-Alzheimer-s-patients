using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class Picture
    {
        [Key]
        public string Picture_Id { get; set; } = Guid.NewGuid().ToString();
        public string Image_Path { get; set; }
        public DateTime Upload_Date { get; set; }
        public string Caption { get; set; }
        #region Navigation Prop

        [ForeignKey(nameof(Patient))]
        public string PatientId { get; set; }
        [ForeignKey(nameof(Family))]
        public string FamilyId { get; set; }


        public Patient patient { get; set; }
        
        public Family family { get; set; }
       
        #endregion
    }
}
