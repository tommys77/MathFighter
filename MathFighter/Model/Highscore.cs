using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;

namespace MathFighter.Resources.Model
{
    public class Highscore
    {
        [PrimaryKey, NotNull]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public int Playtime { get; set; }
        public int TopicId { get; set; }
        public int LevelId { get; set; }

        public Highscore()
        {

        }
        public Highscore(int Id, string Name, int Score, int Playtime, int TopicId, int LevelId)
        {
            this.Id = Id;
            this.Name = Name;
            this.Score = Score;
            this.Playtime = Playtime;
            this.TopicId = TopicId;
            this.LevelId = LevelId;
        }

        public override string ToString()
        {
            return "Name: " + Name + ", Score: " + Score + ", Playtime: " + Playtime;
        }

    }


}