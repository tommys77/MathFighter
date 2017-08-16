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
using System.Globalization;
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

        List<int> intList = new List<int>();
        //For showing the next question
        private void UpdateQuiz()
        {

            subjectId = prefs.GetInt("subjectId", 0);

            var showQuestion = "";
            var difficultyId = prefs.GetInt("difficultyId", 0);
            var r = 0;
            switch (subjectId)
            {
                case 1:
                    x = prefs.GetInt("factor", 1);
                    int numberOfQuestions = prefs.GetInt("questions", 10);
                    if (count == 1)
                    {
                        for (int k = 1; k <= numberOfQuestions; k++)
                        {
                            intList.Add(k);
                        }
                    }
                    if (intList.Count() != 0)
                    {
                        i = intList.ElementAt(new Random().Next(intList.IndexOf(intList.Last())));
                    }
                    intList.Remove(i);
                    //if (!spentQuestions.Co(ntains(i)) { UpdateQuiz(); }
                    //spentQuestions.Add(i);
                    showQuestion = $"{x} * {i}";
                    rightAnswer = calculator.Multiply(x, i);
                    break;
                case 2:
                    var rootableInts = new List<int>();
                    switch (difficultyId)
                    {
                        case 1:

                            GenerateNumbers(10, rootableInts, true);
                            break;
                        case 2:
                            GenerateNumbers(15, rootableInts);
                            break;
                        case 3:
                            GenerateNumbers(25, rootableInts);
                            break;
                    }
                    r = new Random().Next(rootableInts.Count);
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
                    string[] input = { "+", "-", "/", "*" };
                    operation = input.ElementAt(new Random().Next(input.Count() - 1));
                    if (intList.Count == 0)
                    {
                        GenerateNumbers(10, intList);
                        if (difficultyId == 2)
                        {
                            GenerateNumbers(15, intList);
                        }
                        else if (difficultyId == 3) GenerateNumbers(25, intList);
                    }
                    var rnd = new Random();
                    var a = rnd.Next(intList.Count - 1);
                    var b = rnd.Next(intList.Count - 1);
                    if (operation == "/" && (a == 0 || b == 0 || a % b != 0 ))
                    {
                        UpdateQuiz();
                    }
                    showQuestion = $"{a} {operation} {b}";
                    rightAnswer = calculator.MixedSubjects(operation, a, b);
                    intList.Remove(b);
                    break;
            }
            oppgave = (TextView)FindViewById(Resource.Id.tv_quiz_show_question);
            oppgave.SetText(showQuestion, null);
            var oppgaveNr = "Spørsmål " + count;
            tvOppgaveNr = FindViewById<TextView>(Resource.Id.tv_quiz_current_question);
            tvOppgaveNr.SetText(oppgaveNr, null);
        }

        private void GenerateNumbers(int max, List<int> list, bool sqrt = false)
        {
            for (var k = 0; k <= max; k++)
            {
                if (sqrt == true) list.Add(k * k);
                else list.Add(k);
            }
        }


        //Controls what happens when a game is over.
        private void GameOver()
        {
            intList.Clear();
            count = 10;
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
            var transaction = FragmentManager.BeginTransaction();
            var highscoreDialog = new NewHighscoreDialog(totalScore, lowestScoreId, playtime);
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
            var retry = new AlertDialog.Builder(this)
            .SetTitle("En gang til?")
            .SetMessage("Poengsum: " + totalScore + "\nSpilletid: " + playtimeString)
            .SetNegativeButton("Nei takk!", (cancel, args) =>
            {
                Finish();
            })
            .SetPositiveButton("OK", (ok, args) =>
            {
                Recreate();
            });
            retry.Create().Show();
        }

        //Score is based on time spent completing the questions and difficulty. Speedy completion, higher difficulty and more correct answers gives a higher score.
        private int CalculateScore()
        {
            var numberOfQUestions = prefs.GetInt("questions", 10);
            double difficulty;
            switch (prefs.GetInt("difficultyId", 1))
            {
                case 2:
                    difficulty = 1.5;
                    break;
                case 3:
                    difficulty = 2.0;
                    break;
                default:
                    difficulty = 1.0;
                    break;
            }

            var pointsPerCorrectAnswer = 1000 * difficulty;
            var baseScore = pointsPerCorrectAnswer * correctAnswers;
            long timeBonus;
            long maxTime = 60000;
            if (numberOfQUestions == 20)
            {
                maxTime *= 2;
            }
            if (stopWatch.ElapsedMilliseconds > maxTime)
            {
                timeBonus = 0;
            }
            else { timeBonus = ((maxTime / numberOfQUestions) - (stopWatch.ElapsedMilliseconds / numberOfQUestions)) * correctAnswers / 10; }
            var totalScore = baseScore + timeBonus;
            return (int)totalScore;
            //return Convert.ToInt32(baseScore * (1F / ((stopWatch.ElapsedMilliseconds / 1000) * (prefs.GetInt("questions", 10) / 10))));
        }

        //Checks if the answer you gave is correct.
        private void CheckAnswer()
        {
            var answer = (EditText)FindViewById(Resource.Id.answer);
            double.TryParse(answer.Text, out double yourAnswer);

            status = (TextView)FindViewById(Resource.Id.status);
            status.SetText("Ditt svar: " + yourAnswer + " Riktig svar: " + rightAnswer, null);
            if (yourAnswer.ToString().Equals(rightAnswer.ToString()))
            {
                correctAnswers++;
                responseSample = MediaPlayer.Create(this, Resource.Raw.correct);
                responseSample.Start();
                status.Append(" ---  Gratulerer du har nå " + correctAnswers + "av " + count + " mulige rette!");
                // status.SetText("Riktig", null);

            }
            else
            {
                responseSample = MediaPlayer.Create(this, Resource.Raw.incorrect);
                responseSample.Start();
                status.Append("  ---  Beklager, det er feil.");
            }
            count++;
        }
    }
}