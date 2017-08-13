using System.Collections.Generic;
using Android.App;
using Android.Widget;
using Android.OS;
using Java.Util;
using Java.Lang;
using System.IO;
using SQLite;
using Android.Content;
using Android.Preferences;
using MathFighter.Model;

namespace MathFighter
{
    [Activity(Label = "MathFighter", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "highscore.db3");
        private TextView topicsTV;
        private DatabaseManager dbManager;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_main);

            var prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            var editor = prefs.Edit();
            editor.PutString("dbPath", dbPath);
            editor.Apply();
            RefreshScreen();
            dbManager = new DatabaseManager(dbPath);
            dbManager.CreateTable();
            var highscoreBtn = FindViewById<Button>(Resource.Id.main_btn_highscore);
            highscoreBtn.Click += HighscoreBtn_Click;
            var startBtn = FindViewById<Button>(Resource.Id.main_btn_start);
            startBtn.Click += StartBtn_Click;
            var subjectsBtn = FindViewById<Button>(Resource.Id.main_btn_topics);
            subjectsBtn.Click += SubjectsBtn_Click;
        }

        private void SubjectsBtn_Click(object sender, System.EventArgs e)
        {
            var topics = new Intent(this, typeof(SubjectActivity));
            StartActivity(topics);
        }

        private void RefreshScreen()
        {
            topicsTV = (TextView)FindViewById(Resource.Id.main_txt_topics);
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            string subject = prefs.GetString("subject", null);
            topicsTV.SetText(subject ?? "Ikke valgt", null);
        }

        private void StartBtn_Click(object sender, System.EventArgs e)
        {
            Intent quiz = new Intent(this, typeof(QuizActivity));
            StartActivity(quiz);
        }

        private void HighscoreBtn_Click(object sender, System.EventArgs e)
        {
            var intent = new Intent(this, typeof(TopPlayersActivity));
            StartActivity(intent);
        }

        
        protected override void OnResume()
        {
            base.OnResume();
            RefreshScreen();
        }
    }
}

