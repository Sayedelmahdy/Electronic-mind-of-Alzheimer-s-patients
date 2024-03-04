using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class Family:User
    {

        public virtual ICollection<FamilyPatient> FamilyPatients { get; set; }
    }
}
