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
using Android.Preferences;
using Java.Lang;

namespace MathFighter
{
    [Activity(Label = "SubjectActivity")]
    public class SubjectActivity : Activity
    {
        private int questions = 10;
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_subject);
            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            editor = prefs.Edit();
            Listeners();
            var questionSwitch = (Switch)FindViewById(Resource.Id.subject_questionsToggle);
            questionSwitch.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                questions = e.IsChecked ? 20 : 10;
                var toast = Toast.MakeText(this, questions + " spørsmål", ToastLength.Short);
                toast.Show();
                editor.PutInt("questions", questions);
                editor.Apply();
            };
        }

        List<int> factorInt = new List<int>();
        private List<string> factorStrings = new List<string>();
        private void Listeners()
        {
            var timesTable = FindViewById<Button>(Resource.Id.subject_gangetabellenBtn);
            for (int i = 2; i <= 20; i++)
            {
                factorInt.Add(i);
            }
            foreach (var i in factorInt)
            {
                factorStrings.Add(i + " - gangen");
            }

            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, factorStrings);
            timesTable.Click += delegate
            {
                 new AlertDialog.Builder(this)
                     .SetTitle("Velg en rekke")
                     .SetSingleChoiceItems(adapter,factorStrings.IndexOf(prefs.GetInt("factor", -1).ToString()), TimesTableClicked)
                     .Create()
                     .Show();
             };
        }

        private void TimesTableClicked(object sender, DialogClickEventArgs e)
        {
            //Toast.MakeText(this, "Du valgte: " + factorInt.ElementAt(e.Which), ToastLength.Short).Show();
            var rekke = factorInt.ElementAt(e.Which);
            editor.PutInt("factor", rekke);
            editor.PutInt("subjectId", 1);
            editor.PutString("subject", factorStrings.ElementAt(e.Which));
            editor.Apply();
            Finish();
        }
        


    }
}