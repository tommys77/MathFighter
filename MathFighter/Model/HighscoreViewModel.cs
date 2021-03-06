﻿using System;
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
    public class HighscoreViewModel
    {
        public int HighscoreId { get; set; }
        public string Player { get; set; }
        public string ImagePath { get; set; }
        public int Score { get; set; }
        public string Playtime { get; set; }

        public HighscoreViewModel(int highscoreId, string player, string imagePath, int score, string playTime)
        {
            HighscoreId = highscoreId;
            Player = player;
            Score = score;
            Playtime = playTime;
            ImagePath = imagePath;
        }

        //public override string ToString()
        //{
        //    return "Spiller: " + Player + ", Poeng: " + Score + ", Spilletid: " + Playtime + ", Tema: " + SubjectName;
        //}
    }



}