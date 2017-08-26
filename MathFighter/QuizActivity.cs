using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Preferences;
using System.Diagnostics;
using Android.Media;
using MathFighter.Model;
using SQLite;
using System.Timers;

namespace MathFighter
{
    [Activity(Label = "QuizActivity")]
    public class QuizActivity : Activity
    {
        // Primitives
        private int i;
        private int xFactor;
        private int counter = 1;
        private int questionCount;
        private int subjectId;
        private int difficultyId;
        private int correctAnswersCount = 0;
        private double rightAnswer = 0;
        private string operation = "";
        private static string dbPath;
        bool hasAnswered = false;
        private long maxPointsPerQuestion = 500;
        private int spCorrect;
        private int spIncorrect;

        private List<long> timeBonuses = new List<long>();
        private List<int> spentQuestions = new List<int>();
        private List<int> intList = new List<int>();
        private Stopwatch stopWatch = new Stopwatch();
        private Timer timer;

        // Android type variables
        private TextView oppgave;
        private TextView tvOppgaveNr;
        private TextView status;
        private TextView txtTimeBonus;
        private EditText answerEdit;
        private Button answerBtn;
        private ProgressBar reverseProgressBar;
        private AlertDialog retryDialog;
        //private MediaPlayer responseSample;
        private static ISharedPreferences prefs;
        private static SoundPool soundPool;

        // Custom type variables
        private static Calculator calculator;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_quiz);
            SetVariables();
            SetQuizViewElements();
            SetListeners();
            UpdateQuiz();
        }

        private void SetListeners()
        {
            answerBtn.SoundEffectsEnabled = false;
            answerBtn.Click += AnswerBtn_Click;

            answerEdit.EditorAction += AnswerEdit_EditorAction;
            answerEdit.Click += delegate
            {

                answerEdit.Text = "";
                answerEdit.SelectAll();
                if (counter == 1)
                {
                    stopWatch.Start();
                }
            };
        }

        private void SetVariables()
        {
            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            calculator = new Calculator(this);
            soundPool = new SoundPool(1, Stream.Music, 0);
            spCorrect = soundPool.Load(this, Resource.Raw.correct, 1);
            spIncorrect = soundPool.Load(this, Resource.Raw.incorrect, 1);

            // From shared preferences
            dbPath = prefs.GetString("dbPath", null);
            difficultyId = prefs.GetInt("difficultyId", 1);
            questionCount = prefs.GetInt("question", 10);
            subjectId = prefs.GetInt("subjectId", 0);
        }

        private void SetQuizViewElements()
        {
            status = FindViewById<TextView>(Resource.Id.status);
            txtTimeBonus = FindViewById<TextView>(Resource.Id.tv_quiz_timebonus);
            answerEdit = FindViewById<EditText>(Resource.Id.answer);
            reverseProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            reverseProgressBar.Max = (int)maxPointsPerQuestion;
            answerBtn = FindViewById<Button>(Resource.Id.quiz_btn_answer);
            stopWatch = new Stopwatch();

        }

        // Reusable method to set up timer for each question.
        private void BeginTimer()
        {
            reverseProgressBar.Progress = (int)maxPointsPerQuestion;

            timer = new Timer();
            timer.Interval = 10;
            timer.Elapsed += TimerElapsed;
            hasAnswered = false;
            timer.Start();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!hasAnswered)
            {
                reverseProgressBar.Progress--;
            }

            if (reverseProgressBar.Progress == 0)
            {
                timer.Stop();
                RunOnUiThread(() =>
                {
                    answerBtn.PerformClick();
                });

                //reverseProgressBar.Progress = (int)maxPointsPerQuestion;
                //hasAnswered = true;
                //answerBtn.PerformClick();
            }
        }

        private void AnswerEdit_EditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == Android.Views.InputMethods.ImeAction.Done)
            {
                answerBtn.PerformClick();
            }
        }

        //When one clicks the answer button, this is what happens.
        private void AnswerBtn_Click(object sender, EventArgs e)
        {
            hasAnswered = true;
            timeBonuses.Add(reverseProgressBar.Progress);
            txtTimeBonus.Text = "Time bonus: " + timeBonuses.Last().ToString();
            timer.Dispose();
            CheckAnswer();
            if (counter > questionCount)
            {
                stopWatch.Stop();
                GameOver();
            }
            else UpdateQuiz();
        }

        //For showing the next question
        private void UpdateQuiz()
        {
            var showQuestion = "";


            switch (subjectId)
            {
                case 1:
                    xFactor = prefs.GetInt("factor", 1);
                    if (counter == 1)
                    {
                        for (int k = 1; k <= questionCount; k++)
                        {
                            intList.Add(k);
                        }
                    }
                    if (intList.Count() != 0)
                    {
                        i = intList.ElementAt(new Random().Next(intList.IndexOf(intList.Last())));
                    }
                    intList.Remove(i);
                    showQuestion = $"{xFactor} * {i}";
                    rightAnswer = calculator.Multiply(xFactor, i);
                    break;
                case 2:
                    xFactor = GenerateRootQuestion();
                    spentQuestions.Add(xFactor);
                    showQuestion = "\u221a" + xFactor;
                    rightAnswer = Math.Sqrt(xFactor);
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
                    if (operation == "/" && (a == 0 || b == 0 || a % b != 0))
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
            var oppgaveNr = "Spørsmål " + counter;
            tvOppgaveNr = FindViewById<TextView>(Resource.Id.tv_quiz_current_question);
            tvOppgaveNr.SetText(oppgaveNr, null);

            BeginTimer();
        }

        private int GenerateRootQuestion()
        {
            var random = 0;
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
            random = new Random().Next(rootableInts.Count);
            xFactor = rootableInts.ElementAt(random);
            if (spentQuestions.Contains(xFactor))
            {
                GenerateRootQuestion();
            }
            return xFactor;
        }

        // Generates numbers to use in creating a question
        private void GenerateNumbers(int max, List<int> list, bool sqrt = false)
        {
            for (var k = 0; k <= max; k++)
            {
                if (sqrt == true) list.Add(k * k);
                else list.Add(k);
            }
        }

        // When the game is over...
        private void GameOver()
        {
            intList.Clear();
            counter = 10;
            var totalScore = CalculateScore();
            var playtime = stopWatch.ElapsedMilliseconds;
            correctAnswersCount = 0;
            var lowestScore = FindLowestScore();
            if (lowestScore != null && totalScore > lowestScore.Score)
            {
                NewHighscore(totalScore, lowestScore.HighscoreId, playtime);
            }
            else OpenRetryDialog(totalScore, playtime);
        }

        // Returns the lowest current score in the database
        private Highscore FindLowestScore()
        {
            var db = new SQLiteConnection(dbPath);
            var highscoreList = db.Table<Highscore>().Where(h => h.SubjectId == subjectId && h.DifficultyId == difficultyId);
            return subjectId != 0 ? highscoreList.OrderByDescending(s => s.Score).Last() : null;
        }

        // Opens a dialog fragment where you can enter your name and save your highscore, as well as remove the
        // lowest scoring entry from the database.
        private void NewHighscore(int totalScore, int lowestScoreId, long playtime)
        {
            var transaction = FragmentManager.BeginTransaction();
            var highscoreDialog = new NewHighscoreDialog(totalScore, lowestScoreId, playtime, difficultyId);
            highscoreDialog.Show(transaction, "highscore dialog");
            highscoreDialog.DialogClosed += delegate
            {
                OpenRetryDialog(totalScore, playtime);
            };
        }

        //Asks if the player wants to have another go. Goes back to main if not.
        private void OpenRetryDialog(int totalScore, long playtime)
        {
            var playtimeString = TimeSpan.FromMilliseconds(playtime).Minutes + "m " + TimeSpan.FromMilliseconds(playtime).Seconds + "s";

            var builder = new AlertDialog.Builder(this)
            .SetTitle("En gang til?")
            .SetMessage("Poengsum: " + totalScore + "\nSpilletid: " + playtimeString)
            .SetNegativeButton("Nei takk!", StopPlaying)
            .SetPositiveButton("OK", PlayAgain);

            retryDialog = builder.Create();
            retryDialog.Show();
        }

        private void PlayAgain(object sender, DialogClickEventArgs e)
        {
            retryDialog.Dismiss();
            Recreate();
        }

        private void StopPlaying(object sender, DialogClickEventArgs e)
        {
            retryDialog.Dismiss();
            Finish();
        }


        // For now, score is calculated depending on time spent (a small bonus for everything below 1 min per 10 questions)
        // number or correct questions (biggest bonus). The amount of points per question depends on difficulty.
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
            var baseScore = pointsPerCorrectAnswer * correctAnswersCount;
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
            else { timeBonus = ((maxTime / numberOfQUestions) - (stopWatch.ElapsedMilliseconds / numberOfQUestions)) * correctAnswersCount / 10; }
            var totalScore = baseScore + timeBonus;
            return (int)totalScore;
        }

        // Checks if the answer you gave is correct and takes the proper actions.
        private void CheckAnswer()
        {
            //if (responseSample != null)
            //{
            //    responseSample.Release();
            //}

            double.TryParse(answerEdit.Text, out double yourAnswer);

            status.SetText("Ditt svar: " + yourAnswer + " Riktig svar: " + rightAnswer, null);

            if (yourAnswer.ToString().Equals(rightAnswer.ToString()))
            {
                correctAnswersCount++;
                //responseSample = MediaPlayer.Create(this, Resource.Raw.correct);
                //responseSample.Start();
                soundPool.Play(spCorrect, 1, 1, 0, 0, 1);
                status.Append(" ---  Gratulerer du har nå " + correctAnswersCount + "av " + counter + " mulige rette!");
                //if (!responseSample.IsPlaying)
                //{
                //    responseSample.Release();
                //}
            }
            else
            {
                //responseSample = MediaPlayer.Create(this, Resource.Raw.incorrect);
                //responseSample.Start();
                soundPool.Play(spIncorrect, 1, 1, 0, 0, 1);
                status.Append("  ---  Beklager, det er feil.");
                //if (!responseSample.IsPlaying)
                //{
                //    responseSample.Release();
                //}
            }
            counter++;
            answerEdit.Text = "";
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            stopWatch.Reset();
            timer.Dispose();
        }

        protected override void OnPause()
        {
            base.OnPause();
            timer.Dispose();
        }
    }
}