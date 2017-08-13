using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Java.Lang;
using Android.Preferences;
using System.Diagnostics;
using Android.Media;
using Android.Util;
using MathFighter.Model;
using SQLite;

namespace MathFighter
{
    [Activity(Label = "QuizActivity")]
    public class QuizActivity : Activity
    {
        int i = 1;
        int x = 5;
        int antallSpm = 10;
        int correctAnswers = 0;
        private TextView oppgave;
        private TextView status;
        private Stopwatch stopWatch = new Stopwatch();
        private string dbPath;
        private MediaPlayer responseSample;
        private ISharedPreferences prefs;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_quiz);
            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            antallSpm = prefs.GetInt("questions", 10);
            x = prefs.GetInt("factor", 1);
            dbPath = prefs.GetString("dbPath", null);
            status = (TextView)FindViewById(Resource.Id.status);
            UpdateQuiz();
            stopWatch = new Stopwatch();
            stopWatch.Reset();
            Button answerBtn = (Button)FindViewById(Resource.Id.quiz_btn_answer);
            answerBtn.SoundEffectsEnabled = false;
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
            var playtime = stopWatch.ElapsedMilliseconds;
            correctAnswers = 0;
            var lowestScore = FindLowestScore();
            if (lowestScore != null && totalScore > lowestScore.Score)
            {
                NewHighscore(totalScore, lowestScore.HighscoreId, playtime);
            }
            else OpenRetryDialog(totalScore, playtime);
        }

        //Returns the lowest current score in the database
        private Highscore FindLowestScore()
        {
            
            var db = new SQLiteConnection(dbPath);
            var subjectId = prefs.GetInt("subjectId", 0);
            Log.WriteLine(LogPriority.Debug, "MyTAG", "Current SubjectID: " + subjectId);
            var highscoreList = db.Table<Highscore>().Where(h => h.SubjectId == subjectId);
            return subjectId != 0 ? highscoreList.OrderByDescending(s => s.Score).Last() : null;
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
        private void OpenRetryDialog(int totalScore, long playtime)
        {
            var playtimeString = TimeSpan.FromMilliseconds(playtime).Minutes + "m " + TimeSpan.FromMilliseconds(playtime).Seconds + "s";
            AlertDialog.Builder retry = new AlertDialog.Builder(this)
            .SetTitle("En gang til?")
            .SetMessage("Poengsum: " + totalScore + "\nSpilletid: " + playtimeString)
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
            double baseScore = pointsPerCorrectAnswer * correctAnswers;
            return Convert.ToInt32(baseScore * (1F / ((stopWatch.ElapsedMilliseconds / 1000) * (antallSpm / 10))));
        }

        //Checks if the answer you gave is correct.
        private void CheckAnswer()
        {
            EditText answer = (EditText)FindViewById(Resource.Id.answer);
            int yourAnswer = Integer.ParseInt(answer.Text);

            int correctAnswer = Calculator(x, i);
            TextView status = (TextView)FindViewById(Resource.Id.status);
            status.SetText("Ditt svar: " + yourAnswer + " Riktig svar: " + correctAnswer, null);
            if (yourAnswer == correctAnswer)
            {
                responseSample = MediaPlayer.Create(this, Resource.Raw.correct);
                responseSample.Start();
                status.Append(" ---  Gratulerer det var rett!");
                // status.SetText("Riktig", null);
                correctAnswers++;
            }
            else
            {
                responseSample = MediaPlayer.Create(this, Resource.Raw.incorrect);
                responseSample.Start();
                status.Append("  ---  Beklager, det er feil.");
            }

            i++;
        }

        private int first;
        private int second;

        private void RandomNumbers()
        {
            var random = new Random();
            first = random.Next(10);
            second = random.Next(10);
        }
    }
}