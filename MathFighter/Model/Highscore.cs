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

namespace MathFighter.Resources.Model
{
    public class Highscore
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public int Playtime { get; set; }
    }
}