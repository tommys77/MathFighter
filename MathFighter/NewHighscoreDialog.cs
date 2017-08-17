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
using Android.Preferences;
using MathFighter.Model;

namespace MathFighter
{
    public class NewHighscoreDialog : DialogFragment
    {
        private int highscore;
        private int id;
        private long playtime;
        private int difficultyId;
        private Button save;
        private EditText yourName;
        public event EventHandler DialogClosed;
        private ISharedPreferences prefs;

        public NewHighscoreDialog (int highscore_in, int id_in, long playtime_in, int difficultyId_in)
        {
            highscore = highscore_in;
            id = id_in;
            playtime = playtime_in;
            difficultyId = difficultyId_in;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.dialog_new_highscore, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(this.Activity.BaseContext);
            yourName = view.FindViewById<EditText>(Resource.Id.new_highscore_name);
            save = view.FindViewById<Button>(Resource.Id.new_highscore_ok_btn);
            save.Click += Save_Click;

            return view;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            var dbPath = prefs.GetString("dbPath", null);
            var dbManager = new DatabaseManager(dbPath);
            //var db = new SQLiteConnection(dbPath);
            var subjectId = prefs.GetInt("subjectId", 0);
            if (subjectId != 0)
            {
                var newHighscore = new Highscore(id, yourName.Text, highscore, playtime, subjectId, difficultyId);
                dbManager.InsertHighscore(newHighscore);
            }
            else
            {
                Toast.MakeText(this.Activity.BaseContext, "Registrering feilet", ToastLength.Long).Show();
            }
            this.Dismiss();
            
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            DialogClosed?.Invoke(this, null);
        }


    }
}