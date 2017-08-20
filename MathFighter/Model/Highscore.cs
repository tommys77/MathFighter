using MathFighter.Model;
using SQLite;

namespace MathFighter.Model
{
  
    public class Highscore
    {
        [PrimaryKey, NotNull]
        public int HighscoreId { get; set; }
        public string Player { get; set; }
        public string ImagePath { get; set; }
        public int Score { get; set; }
        public long Playtime { get; set; }
        public int SubjectId { get; set; }
        public int DifficultyId { get; set; }

        public Highscore()
        {

        }
        public Highscore(int highscoreId, string player, string imgPath, int score, long playtime, int subjectId, int difficultyId)
        {
            this.HighscoreId = highscoreId;
            this.Player = player;
            this.ImagePath = imgPath;
            this.Score = score;
            this.Playtime = playtime;
            this.SubjectId = subjectId;
            this.DifficultyId = difficultyId;
        }
    }


}