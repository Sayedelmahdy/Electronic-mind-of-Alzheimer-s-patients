using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class GameScore
    {
        public string GameScoreId { get; set; }=Guid.NewGuid().ToString();
        public Difficulty DifficultyGame { get; set; }
        public int PatientScore { get; set; }
        public DateTime GameDate { get; set; }
       
        #region Navigation Property
        public Patient patient { get; set; }
        [ForeignKey(nameof(Patient))]
        public string PatientId { get; set;}
        #endregion
    }
    public enum Difficulty
    {
        Easy = 0,
        Meduim =1,
        Hard =2
    }
}
