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
        private string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "highscore.db3");
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;
        private TextView tvTema;
        private TextView tvVanskelighetsgrad;
        private DatabaseManager dbManager;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ActionBar.SetDisplayShowHomeEnabled(false);
            ActionBar.SetDisplayShowTitleEnabled(false);
            ActionBar.SetCustomView(Resource.Layout.actionbar_main);
            ActionBar.SetDisplayShowCustomEnabled(true);
            SetContentView(Resource.Layout.activity_main);
            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            editor = prefs.Edit();
            editor.PutString("dbPath", dbPath);
            editor.Apply();
            RefreshScreen();
            dbManager = new DatabaseManager(dbPath);
            dbManager.CreateTable();
            var highscoreBtn = FindViewById<Button>(Resource.Id.main_btn_highscore);
            highscoreBtn.Click += HighscoreBtn_Click;
            var startBtn = FindViewById<Button>(Resource.Id.btn_main_start);
            startBtn.Click += StartBtn_Click;
            var subjectsBtn = FindViewById<Button>(Resource.Id.btn_main_tema);
            subjectsBtn.Click += SubjectsBtn_Click;
            var btnVanskelighetsgrad = FindViewById(Resource.Id.btn_main_vanskelighetsgrad);
            btnVanskelighetsgrad.Click += BtnVanskelighetsgrad_Click;
        }

        private void BtnVanskelighetsgrad_Click(object sender, System.EventArgs e)
        {
            var difficultyId = prefs.GetInt("difficultyId", 0);
            if (difficultyId >= 3)
            {
                editor.PutInt("difficultyId", 1);
            }
            else editor.PutInt("difficultyId", difficultyId+1);
            editor.Apply();
            SetDifficulty();
        }

        private void SetDifficulty()
        {
            string difficulty;
            switch (prefs.GetInt("difficultyId", 0))
            {
                case 1:
                    difficulty = "Lett";
                    break;
                case 2:
                    difficulty = "Middels";
                    break;
                case 3:
                    difficulty = "Vanskelig";
                    break;
                default:
                    difficulty = "Lett";
                    editor.PutInt("difficultyId", 1);
                    break;
            }
            tvVanskelighetsgrad.SetText(difficulty, null);
        }

        private void SubjectsBtn_Click(object sender, System.EventArgs e)
        {
            var topics = new Intent(this, typeof(SubjectActivity));
            StartActivity(topics);
        }

        private void RefreshScreen()
        {
            tvTema = FindViewById<TextView>(Resource.Id.tv_main_tema);
            tvVanskelighetsgrad = FindViewById<TextView>(Resource.Id.tv_main_vanskelighetsgrad);
            tvTema.SetText(prefs.GetString("subject", null) ?? "Ikke valgt", null);
            SetDifficulty();
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

