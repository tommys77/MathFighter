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

namespace MathFighter.Model
{
    public class HighscoreViewHelper
    {
        private int HighscoreId { get; set; }
        private string Player { get; set; }
        private int Score { get; set; }
        private string Playtime { get; set; }
        private string SubjectName { get; set; }

        public HighscoreViewHelper() { }

        public HighscoreViewHelper(int highscoreId, string player, int score, string playTime, string subjectName)
        {
            this.HighscoreId = highscoreId;
            this.Player = player;
            this.Score = score;
            this.Playtime = playTime;
            this.SubjectName = subjectName;
        }

        public override string ToString()
        {
            return "Spiller: " + Player + ", Poeng: " + Score + ", Spilletid: " + Playtime + ", Tema: " + SubjectName;
        }
    }



}