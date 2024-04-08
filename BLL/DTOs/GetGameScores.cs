using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class GetGameScoresDto
    {
        public int RecomendationDifficulty { get; set; }
        public ICollection<GameScoreDto> GameScore { get; set; }
    }
}
