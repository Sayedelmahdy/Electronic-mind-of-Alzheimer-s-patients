using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class Family:User
    {
        public string? Relationility { get; set; }


        #region Navigation Prop
        public string? PatientId { get; set; }
        public Patient Patient { get; set; }
        public ICollection<Picture> Pictures { get; set; }  
        #endregion
    }
}
