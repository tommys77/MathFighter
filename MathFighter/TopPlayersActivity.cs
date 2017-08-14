using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Renderscripts;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MathFighter.Adapters;
using MathFighter.Model;
using SQLite;


namespace MathFighter
{
    [Activity(Label = "TopPlayersActivity")]
    public class TopPlayersActivity : Activity
    {
        private string dbPath;
        private ISharedPreferences prefs;
        private int subjectId;
        private RadioButton rb_gangetabellen;
        private RadioButton rb_kvadratrot;
        private RadioButton rb_lett_blanding;
        private RadioGroup subjectRadioGroup;

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
            rb_gangetabellen = FindViewById<RadioButton>(Resource.Id.rb_gangetabellen);
            rb_kvadratrot = FindViewById<RadioButton>(Resource.Id.rb_kvadratrot);
            rb_lett_blanding = FindViewById<RadioButton>(Resource.Id.rb_lett_blanding);
            subjectRadioGroup = FindViewById<RadioGroup>(Resource.Id.rbg_tema);
            SetCurrentSubjectRadio(subjectRadioGroup);
            subjectRadioGroup.CheckedChange += SubjectRadioGroup_CheckedChange; ;
            GetHighscoreTable();
        }

        private void SubjectRadioGroup_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            if (e.CheckedId == rb_gangetabellen.Id)
            {
                subjectId = 1;
            }
            else if (e.CheckedId == rb_lett_blanding.Id)
            {
                subjectId = 2;
            }
            else if (e.CheckedId == rb_kvadratrot.Id)
            {
                subjectId = 3;
            }
            GetHighscoreTable();
        }

        public void SetCurrentSubjectRadio(RadioGroup subjectGroup)
        {
            switch (subjectId)
            {
                case 1:
                    subjectGroup.Check(Resource.Id.rb_gangetabellen);
                    break;
                case 2:
                    subjectGroup.Check(Resource.Id.rb_kvadratrot);
                    break;
                case 3:
                    subjectGroup.Check(Resource.Id.rb_lett_blanding);
                    break;
                default:
                    subjectGroup.Check(Resource.Id.rb_lett_blanding);
                    break;
            }
        }

        private void GetHighscoreTable()
        {
            //var hallOfFame = FindViewById<TextView>(Resource.Id.top_txt_highscore);
            //hallOfFame.Text = "";
            if (dbPath == null) return;
            var dbManager = new DatabaseManager(dbPath);
            var highscores = dbManager.GetHighscores(subjectId);
            var mList = new List<HighscoreViewHelper>();
            var adapter = new HighscoreListAdapter(this, highscores);
            var lvHighscores = FindViewById<ListView>(Resource.Id.listView_top_player_highscores);
            lvHighscores.Adapter = adapter;
        }



    }
}