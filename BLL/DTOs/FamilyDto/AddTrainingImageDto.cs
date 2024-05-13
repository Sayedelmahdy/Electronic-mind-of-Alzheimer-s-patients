using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.FamilyDto
{
    public class AddTrainingImageDto
    {
        public List< IFormFile> TrainingImages { get; set; }
    }
}
