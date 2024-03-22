using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class Family:User
    {
        public string? Relationility { get; set; }


        #region Navigation Prop
        [ForeignKey(nameof(Patient))]
        public string? PatientId { get; set; }
        public Patient patient { get; set; }
        public ICollection<Picture> Pictures { get; set; }  
        #endregion
    }
}
