using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.PatientDto
{
    public class CurrentAndMaxScoreDto
    {
        public ScoreDto Score { get; set; }
        public int RecommendedGameDifficulty { get; set; }
        
    }
}
