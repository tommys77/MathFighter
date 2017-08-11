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
using System.Diagnostics;
using SQLite;
using MathFighter.Resources.Model;

namespace MathFighter
{
    [Activity(Label = "QuizActivity")]
    public class QuizActivity : Activity
    {
        int i = 1;
        int x = 5;
        int antallSpm = 10;
        int antallRette = 0;
        private TextView oppgave;
        private TextView status;
        private Stopwatch stopWatch = new Stopwatch();
        private string dbPath;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_quiz);
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            antallSpm = prefs.GetInt("questions", 10);
            x = prefs.GetInt("factor", 1);
            status = (TextView)FindViewById(Resource.Id.status);
            dbPath = prefs.GetString("dbPath", null);
            UpdateQuiz();
            stopWatch = new Stopwatch();
            Button answerBtn = (Button)FindViewById(Resource.Id.quiz_btn_answer);
            answerBtn.Click += AnswerBtn_Click;
            EditText answerEdit = (EditText)FindViewById(Resource.Id.answer);
            answerEdit.Click += delegate
            {
                answerEdit.SelectAll();
                if (i == 1)
                {
                    stopWatch.Start();
                }

            };

        }

        private void AnswerBtn_Click(object sender, EventArgs e)
        {
            CheckAnswer();

            if (i > antallSpm)
            {
                stopWatch.Stop();
                GameOver();
            }

            UpdateQuiz();
        }

        private void UpdateQuiz()
        {
            oppgave = (TextView)FindViewById(Resource.Id.question);
            string showQuestion = $"{i} * {x}";
            oppgave.SetText(showQuestion, null);
        }

        private int Multiplication(int x, int i)
        {
            int answer = 0;
            if (i <= antallSpm)
            {
                answer = x * i;
            }
            return answer;
        }

        private void GameOver()
        {
            i = 1;
            int totalScore = CalculateScore();
            string totalTid = stopWatch.Elapsed.Minutes + "m " + stopWatch.Elapsed.Seconds + "s";
            status.SetText("Time: " + totalTid + " -- In Millis: " + stopWatch.ElapsedMilliseconds + " Score: " + CalculateScore(), null);
            antallRette = 0;
            stopWatch.Reset();
           

            AlertDialog.Builder retry = new AlertDialog.Builder(this)
                .SetTitle("En gang til?")
                .SetMessage("Tid: " + totalTid + "\nPoengsum: " + totalScore)
                .SetNegativeButton("Nei takk!", (Cancel, args) =>
                {
                    Finish();
                })
                .SetPositiveButton("OK", (OK, args) =>
                {
                    this.Recreate();
                });
            retry.Create();
            var lowestScore = LowestScore();
            if (totalScore > lowestScore.Score)
            {
                NewHighscore(totalScore, lowestScore.Id, stopWatch.ElapsedMilliseconds);
                retry.Show();
            }
        }

        private Highscore LowestScore()
        {
            var db = new SQLiteConnection(dbPath);
            var highscoreList = db.Table<Highscore>();
            var lowest = highscoreList.OrderByDescending(s => s.Score).Last();
            return lowest;

        }

        //Creates a dialog where you can enter your name and save your highscore, as well as remove the
        //lowest scoring entry from the database.
        private void NewHighscore(int totalScore, int lowestScoreId, long playtime)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            NewHighscoreDialog highscoreDialog = new NewHighscoreDialog(totalScore, lowestScoreId, playtime);
            highscoreDialog.Show(transaction, "highscore dialog");
        }

        //Score is based on time spent completing the questions and difficulty. Speedy completion, higher difficulty and more correct answers gives a higher score.
        private int CalculateScore()
        {
            double difficulty = 1.0;
            double pointsPerCorrectAnswer = 1000 * difficulty;
            double baseScore = pointsPerCorrectAnswer * antallRette;
            return Convert.ToInt32(baseScore * (1F / ((stopWatch.ElapsedMilliseconds / 1000) * (antallSpm / 10))));
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
            else
            {
                status.Append("  ---  Beklager, det er feil.");
            }

            i++;
        }

    }
}