using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class PostGameScoreDto
    {
     
        public Difficulty DifficultyGame { get; set; }
        public int PatientScore { get; set; }
      
        
    }
}
