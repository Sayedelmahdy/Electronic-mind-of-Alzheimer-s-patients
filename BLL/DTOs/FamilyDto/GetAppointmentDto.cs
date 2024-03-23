using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.FamilyDto
{
    public class GetAppointmentDto
    {
        public string AppointmentId { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
        public string FamilyNameWhoCreatedAppointemnt { get; set; }
        public bool CanDeleted { get; set; }
    }
}
