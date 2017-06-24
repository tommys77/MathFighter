﻿using Android.App;
using Android.Widget;
using Android.OS;
using Java.Util;
using Java.Lang;

namespace MathFighter
{
    [Activity(Label = "MathFighter", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            RandomNumbers();
            Button answerBtn = (Button)FindViewById(Resource.Id.main_btn_answer);
            Button tryAgainBtn = (Button)FindViewById(Resource.Id.main_btn_try_again);
            tryAgainBtn.Click += TryAgainBtn_Click;
            answerBtn.Click += AnswerBtn_Click;
        }

        private void TryAgainBtn_Click(object sender, System.EventArgs e)
        {
            RandomNumbers();
        }

        private void AnswerBtn_Click(object sender, System.EventArgs e)
        {

            CheckAnswer();
            //throw new System.NotImplementedException();
        }

        int first = 0;
        int second = 0;

        private void RandomNumbers()
        {
            Random random = new Random();
            TextView firstNumber = (TextView) FindViewById(Resource.Id.first_number);
            TextView secondNumber = (TextView)FindViewById(Resource.Id.second_number);
            first = random.NextInt(10);
            second = random.NextInt(10);
            firstNumber.SetText(first.ToString(), null);
            secondNumber.SetText(second.ToString(), null);
        }
                
        private int Add()
        {
            return first + second;
        }

        private void CheckAnswer()
        {
            EditText answer = (EditText)FindViewById(Resource.Id.answer);
            int yourAnswer = Integer.ParseInt(answer.Text);
            int correctAnswer = Add();
            TextView status = (TextView)FindViewById(Resource.Id.status);
            if (yourAnswer == correctAnswer)
            {
                status.SetText("Riktig!", null);
            }
            else status.SetText("Feil!", null);
        }


    }
}
