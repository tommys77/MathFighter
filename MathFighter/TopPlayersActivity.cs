using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using MathFighter.Adapters;
using MathFighter.Model;


namespace MathFighter
{
    [Activity(Label = "TopPlayersActivity")]
    public class TopPlayersActivity : Activity
    {
        private string dbPath;
        private ISharedPreferences prefs;
        private int subjectId;
        private Button btnDifficulty;
        private Button btnSubject;
        private int difficultyId;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ActionBar.SetDisplayShowHomeEnabled(false);
            ActionBar.SetDisplayShowTitleEnabled(false);
            ActionBar.SetCustomView(Resource.Layout.actionbar_top_player);
            ActionBar.SetDisplayShowCustomEnabled(true);

            SetContentView(Resource.Layout.activity_top_player);

            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            dbPath = prefs.GetString("dbPath", null);
            subjectId = prefs.GetInt("subjectId", 1);
            difficultyId = prefs.GetInt("difficultyId", 1);
            //rb_gangetabellen = FindViewById<RadioButton>(Resource.Id.rb_gangetabellen);
            //rb_kvadratrot = FindViewById<RadioButton>(Resource.Id.rb_kvadratrot);
            //rb_lett_blanding = FindViewById<RadioButton>(Resource.Id.rb_lett_blanding);
            //subjectRadioGroup = FindViewById<RadioGroup>(Resource.Id.rbg_tema);

            btnDifficulty = FindViewById<Button>(Resource.Id.btn_top_player_difficulty);
            btnDifficulty.Click += BtnDifficulty_Click;

            btnSubject = FindViewById<Button>(Resource.Id.btn_top_player_subject);
            btnSubject.Text = prefs.GetString("subject", null);
            btnSubject.Click += BtnSubject_Click;

            SetCurrentSubjectAndDifficulty();

            //SetCurrentSubjectRadio(subjectRadioGroup);


            //subjectRadioGroup.CheckedChange += SubjectRadioGroup_CheckedChange; ;
            GetHighscoreTable();
        }

        private void BtnDifficulty_Click(object sender, EventArgs e)
        {
            if (difficultyId >= 3)
            {
                difficultyId = 1;
            }
            else difficultyId++;
            SetCurrentSubjectAndDifficulty();
        }

        private void BtnSubject_Click(object sender, EventArgs e)
        {
            if (subjectId >= 3)
            {
                subjectId = 1;
            }
            else subjectId++;
            SetCurrentSubjectAndDifficulty();
        }

        private void SetCurrentSubjectAndDifficulty()
        {
            switch (difficultyId)
            {
                case 1:
                    btnDifficulty.Text = "Lett";
                    break;
                case 2:
                    btnDifficulty.Text = "Middels";
                    break;
                case 3:
                    btnDifficulty.Text = "Vanskelig";
                    break;
            }

            switch (subjectId)
            {
                case 1:
                    btnSubject.Text = "Gangetabellen";
                    break;
                case 2:
                    btnSubject.Text = "Kvadratrøtter";
                    break;
                case 3:
                    btnSubject.Text = "Lett blanding";
                    break;
            }
            GetHighscoreTable();
        }

        //private void SubjectRadioGroup_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        //{
        //    if (e.CheckedId == rb_gangetabellen.Id)
        //    {
        //        subjectId = 1;
        //    }
        //    else if (e.CheckedId == rb_kvadratrot.Id)
        //    {
        //        subjectId = 2;
        //    }
        //    else if (e.CheckedId == rb_lett_blanding.Id)
        //    {
        //        subjectId = 3;
        //    }
        //    GetHighscoreTable();
        //}

        //        public void SetCurrentSubjectRadio(RadioGroup subjectGroup)
        //{
        //            switch (subjectId)
        //            {
        //                case 1:
        //                    subjectGroup.Check(Resource.Id.rb_gangetabellen);
        //                    break;
        //                case 2:
        //                    subjectGroup.Check(Resource.Id.rb_kvadratrot);
        //                    break;
        //                case 3:
        //                    subjectGroup.Check(Resource.Id.rb_lett_blanding);
        //                    break;
        //                default:
        //                    subjectGroup.Check(Resource.Id.rb_lett_blanding);
        //                    break;
        //            }
        //        }

        private void GetHighscoreTable()
        {
            if (dbPath == null) return;
            var dbManager = new DatabaseManager(dbPath);
            var highscores = dbManager.GetHighscores(subjectId, difficultyId);
            var mList = new List<HighscoreViewModel>();
            var adapter = new HighscoreListAdapter(this, highscores);
            var lvHighscores = FindViewById<ListView>(Resource.Id.listView_top_player_highscores);
            lvHighscores.Adapter = adapter;
        }

    }
}