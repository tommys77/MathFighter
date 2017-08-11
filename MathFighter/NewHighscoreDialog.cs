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
using MathFighter.Resources.Model;
using SQLite;
using Android.Preferences;

namespace MathFighter
{
    public class NewHighscoreDialog : DialogFragment
    {
        private int highscore;
        private int id;
        private long playtime;
        private Button save;
        private EditText yourName;

        public NewHighscoreDialog (int highscore_in, int id_in, long playtime_in)
        {
            highscore = highscore_in;
            id = id_in;
            playtime = playtime_in;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.dialog_new_highscore, container, false);

            yourName = view.FindViewById<EditText>(Resource.Id.new_highscore_name);
            save = view.FindViewById<Button>(Resource.Id.new_highscore_ok_btn);
            save.Click += Save_Click;

            return view;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this.Activity.BaseContext);
            var dbPath = prefs.GetString("dbPath", null);
            var db = new SQLiteConnection(dbPath);
            var newHighscore = new Highscore(id, yourName.Text, highscore, playtime, 1 , 1);
            db.InsertOrReplace(newHighscore);
            this.Dismiss();
        }
        
    }
}