using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.FamilyDto
{
    public class GetPictureDto
    {
        public string PictureId { get; set; }
        public DateTime Uploaded_date { get; set; }
        public string Caption { get; set; }
        public string Picture { get; set; }
    }


}
