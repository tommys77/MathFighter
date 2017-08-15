using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Java.Lang;
using Android.Preferences;
using System.Diagnostics;
using Android.Media;
using MathFighter.Model;
using SQLite;
using Double = System.Double;

namespace MathFighter
{
    [Activity(Label = "QuizActivity")]
    public class QuizActivity : Activity
    {
        private int i;
        private int x;
        private int count = 1;
        int correctAnswers = 0;
        private TextView oppgave;
        private TextView tvOppgaveNr;
        private TextView status;
        private Stopwatch stopWatch = new Stopwatch();
        private string dbPath;
        private MediaPlayer responseSample;
        private ISharedPreferences prefs;
        private int subjectId;
        private double rightAnswer = 0;
        private string operation = "";
        private static Calculator calculator;
        private List<int> spentQuestions = new List<int>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_quiz);
            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            calculator = new Calculator(this);
            dbPath = prefs.GetString("dbPath", null);
            status = (TextView)FindViewById(Resource.Id.status);
            UpdateQuiz();
            stopWatch = new Stopwatch();
            stopWatch.Reset();
            var answerBtn = (Button)FindViewById(Resource.Id.quiz_btn_answer);
            answerBtn.SoundEffectsEnabled = false;
            answerBtn.Click += AnswerBtn_Click;
            var answerEdit = (EditText)FindViewById(Resource.Id.answer);
            answerEdit.Click += delegate
            {

                answerEdit.SelectAll();
                if (count == 1)
                {
                    stopWatch.Start();
                }
            };
        }

        //When one clicks a button, this is what happens.
        private void AnswerBtn_Click(object sender, EventArgs e)
        {
            CheckAnswer();

            if (count > prefs.GetInt("question", 10))
            {
                stopWatch.Stop();
                GameOver();
            }

            UpdateQuiz();
        }

        //For showing the next question
        private void UpdateQuiz()
        {

            subjectId = prefs.GetInt("subjectId", 0);

            var showQuestion = "";
            var difficultyId = prefs.GetInt("difficultyId", 0);

            switch (subjectId)
            {
                case 1:
                    x = prefs.GetInt("factor", 1);
                    i = new Random().Next(1, prefs.GetInt("questions", 10));
                    showQuestion = $"{x} * {i}";
                    spentQuestions.Add(i);
                    rightAnswer = calculator.Multiply(x, i);
                    break;
                case 2:
                    var rootableInts = new List<int>();
                    switch (difficultyId)
                    {
                        case 1:
                            for (var i = 0; i <= 10; i++)
                            {
                                rootableInts.Add(i * i);
                            }
                            break;
                        case 2:
                            for (var i = 0; i <= 15; i++)
                            {
                                rootableInts.Add(i * i);
                            }
                            break;
                        case 3:
                            for (var i = 0; i <= 25; i++)
                            {
                                rootableInts.Add(i * i);
                            }
                            break;
                    }
                    var r = new Random().Next(rootableInts.Count);
                    x = rootableInts.ElementAt(r);
                    if (spentQuestions.Contains(x))
                    {
                        UpdateQuiz();
                    }
                    spentQuestions.Add(x);
                    showQuestion = "\u221a" + x;
                    rightAnswer = System.Math.Sqrt(x);
                    break;
                case 3:
                    var list = new List<string>();
                    string[] input = { "+", "-", "/", "*" };
                    list.AddRange(input);
                    operation = new Random().Next(list.IndexOf(list.First()), list.Count).ToString();
                    break;
            }
            oppgave = (TextView)FindViewById(Resource.Id.tv_quiz_show_question);
            oppgave.SetText(showQuestion, null);
            var oppgaveNr = "Spørsmål " + count;
            tvOppgaveNr = FindViewById<TextView>(Resource.Id.tv_quiz_current_question);
            tvOppgaveNr.SetText(oppgaveNr, null);
        }


        //Controls what happens when a game is over.
        private void GameOver()
        {
            var totalScore = CalculateScore();
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
            //var subjectId = prefs.GetInt("subjectId", 0);
            //Log.WriteLine(LogPriority.Debug, "MyTAG", "Current SubjectID: " + subjectId);
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
            var difficulty = 1.0;
            var pointsPerCorrectAnswer = 1000 * difficulty;
            var baseScore = pointsPerCorrectAnswer * correctAnswers;
            return Convert.ToInt32(baseScore * (1F / ((stopWatch.ElapsedMilliseconds / 1000) * (prefs.GetInt("questions", 10) / 10))));
        }

        //Checks if the answer you gave is correct.
        private void CheckAnswer()
        {
            EditText answer = (EditText)FindViewById(Resource.Id.answer);
            double yourAnswer;
            Double.TryParse(answer.Text, out yourAnswer);

            TextView status = (TextView)FindViewById(Resource.Id.status);
            status.SetText("Ditt svar: " + yourAnswer + " Riktig svar: " + rightAnswer, null);
            if (yourAnswer == rightAnswer)
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
            count++;
        }

        //private int first;
        //private int second;

        //private void RandomNumbers()
        //{
        //    var random = new Random();
        //    first = random.Next(10);
        //    second = random.Next(10);
        //}
    }
}