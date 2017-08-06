﻿using System;
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
        private int defaultSelectedItem;
        private ISharedPreferences prefs;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Subject);
            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
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

        List<int> factorList = new List<int>();
        private void Listeners()
        {
            Button timesTable = FindViewById<Button>(Resource.Id.subject_gangetabellenBtn);
            for (int i = 2; i <= 20; i++)
            {
                factorList.Add(i);
            }
            var adapter = new ArrayAdapter<int>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, factorList);
            timesTable.Click += delegate
             {
                 new AlertDialog.Builder(this)
                     .SetTitle("Velg en rekke")
                     .SetSingleChoiceItems(adapter, factorList.IndexOf(prefs.GetInt("factor", -1)), TimesTableClicked)
                     .Create()
                     .Show();
             };
        }

        private void TimesTableClicked(object sender, DialogClickEventArgs e)
        {

            var toast = Toast.MakeText(this, "Du valgte: " + factorList.ElementAt(e.Which), ToastLength.Short);
            toast.Show();
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutInt("factor", Integer.ParseInt(factorList.ElementAt(e.Which).ToString()));
            editor.Apply();
            Finish();

        }


    }
}