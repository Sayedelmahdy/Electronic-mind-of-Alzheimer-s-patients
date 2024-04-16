using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.PatientDto
{
    public class GameScoreDto
    {
        public string GameScoreId { get; set; }

        public Difficulty DifficultyGame { get; set; }
        public int PatientScore { get; set; }

    }
}
