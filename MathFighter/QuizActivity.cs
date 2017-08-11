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
            stopWatch.Reset();
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

        //When one clicks a button, this is what happens.
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

        //For showing the next question
        private void UpdateQuiz()
        {
            oppgave = (TextView)FindViewById(Resource.Id.question);
            string showQuestion = $"{i} * {x}";
            oppgave.SetText(showQuestion, null);
        }

        //Currently just a method to multiply two numbers. TODO: Expand to handle different math operations, not just multiplication.
        private int Calculator(int x, int i)
        {
            int answer = 0;
            if (i <= antallSpm)
            {
                answer = x * i;
            }
            return answer;
        }
        //Controls what happens when a game is over.
        private void GameOver()
        {
            i = 1;
            int totalScore = CalculateScore();
            long playtime = stopWatch.ElapsedMilliseconds;
            antallRette = 0;
            var lowestScore = FindLowestScore();
            if (totalScore > lowestScore.Score)
            {
                NewHighscore(totalScore, lowestScore.Id, stopWatch.ElapsedMilliseconds);
            }
            else OpenRetryDialog(totalScore, stopWatch.ElapsedMilliseconds);
        }

        //Returns the lowest current score in the database
        private Highscore FindLowestScore()
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
            highscoreDialog.DialogClosed += delegate
            {
                OpenRetryDialog(totalScore, playtime);
            };
        }

        //Asks if the player wants to have another go, or if he wants to stop.
        private void OpenRetryDialog(int totalScore, long _playtime)
        {
            var playtime = TimeSpan.FromMilliseconds(_playtime).Minutes + "m " + TimeSpan.FromMilliseconds(_playtime).Seconds + "s";
            AlertDialog.Builder retry = new AlertDialog.Builder(this)
            .SetTitle("En gang til?")
            .SetMessage("Poengsum: " + totalScore + "\nSpilletid: " + playtime)
            .SetNegativeButton("Nei takk!", (Cancel, args) =>
            {
                Finish();
            })
            .SetPositiveButton("OK", (OK, args) =>
            {
                this.Recreate();
            });
            retry.Create().Show();
        }

        //Score is based on time spent completing the questions and difficulty. Speedy completion, higher difficulty and more correct answers gives a higher score.
        private int CalculateScore()
        {
            double difficulty = 1.0;
            double pointsPerCorrectAnswer = 1000 * difficulty;
            double baseScore = pointsPerCorrectAnswer * antallRette;
            return Convert.ToInt32(baseScore * (1F / ((stopWatch.ElapsedMilliseconds / 1000) * (antallSpm / 10))));
        }

        //Checks if the answer you gave is correct.
        private void CheckAnswer()
        {
            EditText answer = (EditText)FindViewById(Resource.Id.answer);
            int yourAnswer = Integer.ParseInt(answer.Text);

            int correctAnswer = Calculator(x, i);
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