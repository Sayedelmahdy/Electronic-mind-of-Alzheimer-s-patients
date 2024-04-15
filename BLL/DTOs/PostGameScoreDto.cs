using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class PostGameScoreDto
    {

        [Required]
        public Difficulty DifficultyGame { get; set; }
        [Required]
        public int PatientScore { get; set; }
      
        
    }
}
