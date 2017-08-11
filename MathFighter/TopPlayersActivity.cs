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
using MathFighter.Resources.Model;


namespace MathFighter
{
    [Activity(Label = "TopPlayersActivity")]
    public class TopPlayersActivity : Activity
    {
        private string dbPath;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_top_player);
            dbPath = Intent.GetStringExtra("path" ?? "Invalid path");
            GetHighscoreTable(dbPath);
        }

        /// <summary>
        /// Gets the highscore table.
        /// </summary>
        /// <param name="path">The path to the database file.</param>
        private void GetHighscoreTable(string path)
        {
            TextView hallOfFame = (TextView)FindViewById(Resource.Id.top_txt_highscore);
            hallOfFame.Text = "";
            var db = new SQLiteConnection(path);
            var table = db.Table<Highscore>().OrderByDescending(s => s.Score);
            foreach (var item in table)
            {
                Highscore highscore = new Highscore(item.Id, item.Name, item.Score, item.Playtime, item.TopicId, item.LevelId);
                hallOfFame.Text += highscore + "\n";
            }
        }

    }
}