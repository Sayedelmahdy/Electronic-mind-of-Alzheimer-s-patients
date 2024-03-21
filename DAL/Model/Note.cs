using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class Note
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Created_At { get; set; }
        #region Navigation Prop
        public Patient patient { get; set; }
        [ForeignKey(nameof(Patient))]
        public string PatientId { get; set; }
        #endregion
    }
}
