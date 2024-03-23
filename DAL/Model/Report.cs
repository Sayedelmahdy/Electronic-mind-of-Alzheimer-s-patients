using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class Report
    {
        public string ReportId {  get; set; }=Guid.NewGuid().ToString();
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set;}
        public string ReportContent { get; set; }
        #region Navigation Property
        public Caregiver caregiver { get; set; }
        [ForeignKey(nameof(Caregiver))]
        public string CaregiverId { get; set; }
        public Patient patient { get; set; }
        [ForeignKey(nameof(Patient))]
        public string PatientId { get; set; }
        #endregion
    }
}
