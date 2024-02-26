using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class Caregiver:User
    {
        public virtual ICollection<Patient> Patients { get; set; }
    }
}
