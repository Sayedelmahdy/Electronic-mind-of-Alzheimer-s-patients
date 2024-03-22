using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.FamilyDto
{
    public class AddPictureDto
    {
        [Required]
        [FileExtensions(Extensions ="png,jpg" , ErrorMessage ="Valid File Extensions is ` png , jpg `") ]
       public IFormFile Picture {  get; set; }
        [Required]
       public string Caption { get; set; }  


    }
}
