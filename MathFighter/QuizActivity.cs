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
using Java.Lang;
using Android.Preferences;

namespace MathFighter
{
    [Activity(Label = "QuizActivity")]
    public class QuizActivity : Activity
    {
        int i = 1;
        int x = 5;
        int j = 10;
        int antallRette = 0;
        private TextView first;
        private TextView second;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Quiz);
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            j = prefs.GetInt("questions", 10);
            x = prefs.GetInt("factor", 1);
            UpdateQuiz();
            Button answerBtn = (Button)FindViewById(Resource.Id.quiz_btn_answer);
            answerBtn.Click += AnswerBtn_Click;
            EditText answerEdit = (EditText)FindViewById(Resource.Id.answer);
            answerEdit.Click += delegate
            {
                answerEdit.SelectAll();
            };

        }

        private void AnswerBtn_Click(object sender, EventArgs e)
        {
            CheckAnswer();
            if (i == j)
            {
                GameOver();
            }
            i++;
            UpdateQuiz();
        }

        private void UpdateQuiz()
        {
            first = (TextView)FindViewById(Resource.Id.first_number);
            second = (TextView)FindViewById(Resource.Id.second_number);
            first.SetText(i.ToString(), null);
            second.SetText(x.ToString(), null);
        }

        private int Multiplication(int x, int i)
        {
            int answer = 0;
            if (i <= j)
            {
                answer = x * i;
            }
            return answer;
        }

        private void GameOver()
        {
            TextView status = (TextView)FindViewById(Resource.Id.status);
            status.SetText("Gratulerer! Du fikk " + antallRette + " av " + i + " rette.", null);
            i = 0;
            antallRette = 0;
        }

        private void CheckAnswer()
        {
            EditText answer = (EditText)FindViewById(Resource.Id.answer);
            int yourAnswer = Integer.ParseInt(answer.Text);

            int correctAnswer = Multiplication(x, i);
            TextView status = (TextView)FindViewById(Resource.Id.status);
            status.SetText("Ditt svar: " + yourAnswer.ToString() + " Riktig svar: " + correctAnswer.ToString(), null);
            if (yourAnswer == correctAnswer)
            {
                status.Append(" ---  Gratulerer det var rett!");
                // status.SetText("Riktig", null);
                antallRette++;
            }
            else status.Append("  ---  Beklager, det er feil.");

        }

    }
}