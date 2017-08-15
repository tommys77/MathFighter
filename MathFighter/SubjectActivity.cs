using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Drm;
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
            var questionSwitch = (Switch)FindViewById(Resource.Id.sw_subject_antall_sporsmaal);
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
            var timesTable = FindViewById<Button>(Resource.Id.btn_subject_gangetabellen);
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
            var btnSqrt = FindViewById<Button>(Resource.Id.btn_subject_kvadratrot);
            btnSqrt.Click += delegate
            {
                SetSharedPreferencesAndFinish(2, "Kvadratrøtter");
            };
        }

        private void TimesTableClicked(object sender, DialogClickEventArgs e)
        {
            var rekke = factorInt.ElementAt(e.Which);
            SetSharedPreferencesAndFinish(1, factorStrings.ElementAt(e.Which), rekke);
        }

        private void SetSharedPreferencesAndFinish(int subjectId, string subject, int factor = 0)
        {
            //Toast.MakeText(this, "Du valgte: " + subject, ToastLength.Long).Show();
            editor.PutInt("subjectId", subjectId);
            editor.PutString("subject", subject);
            editor.PutInt("factor", factor);
            editor.Apply();
            Finish();
        }
        


    }
}