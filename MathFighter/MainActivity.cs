using Android.App;
using Android.Widget;
using Android.OS;
using Java.Util;
using Java.Lang;
using System.IO;
using SQLite;
using MathFighter.Resources.Model;
using Android.Content;
using Android.Preferences;

namespace MathFighter
{
    [Activity(Label = "MathFighter", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {

        private string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "highscore.db3");
        private TextView topicsTV;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            //prefs.GetInt("questions", 10);
            //prefs.GetInt("factor", 1);
            createTable(dbPath);
            RefreshScreen();
            //RandomNumbers();
            //Button answerBtn = (Button)FindViewById(Resource.Id.main_btn_answer);
            //Button tryAgainBtn = (Button)FindViewById(Resource.Id.main_btn_try_again);
            Button highscoreBtn = (Button)FindViewById(Resource.Id.main_btn_highscore);
            //Button addScoreBtn = (Button)FindViewById(Resource.Id.main_btn_add_score);
            //tryAgainBtn.Click += TryAgainBtn_Click;
            //answerBtn.Click += AnswerBtn_Click;
            highscoreBtn.Click += HighscoreBtn_Click;
            Button startBtn = (Button)FindViewById(Resource.Id.main_btn_start);
            startBtn.Click += StartBtn_Click;
            var topicsBtn = (Button)FindViewById(Resource.Id.main_btn_topics);
            topicsBtn.Click += TopicsBtn_Click;
            //addScoreBtn.Click += AddScoreBtn_Click;
        }

        private void TopicsBtn_Click(object sender, System.EventArgs e)
        {
            var topics = new Intent(this, typeof(SubjectActivity));
            StartActivity(topics);
        }

        private void RefreshScreen()
        {
            topicsTV = (TextView)FindViewById(Resource.Id.main_txt_topics);
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            string showTopic = prefs.GetString("tema", null);
            topicsTV.SetText(showTopic, null);
        }

        private void StartBtn_Click(object sender, System.EventArgs e)
        {
            Intent quiz = new Intent(this, typeof(QuizActivity));
            StartActivity(quiz);
        }

        //private void AddScoreBtn_Click(object sender, System.EventArgs e)
        //{
        //    var db = new SQLiteConnection(dbPath);
        //    db.InsertOrReplace(new Highscore(3, "Tommy", 100, 13, 2, 2));
        //}

        private void HighscoreBtn_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(TopPlayersActivity));
            intent.PutExtra("path", dbPath);
            StartActivity(intent);
        }


        //Method to create table if not already exists.
        private void createTable(string path)
        {

            var db = new SQLiteConnection(path);
            // db.DropTable<Highscore>();
            db.CreateTable<Highscore>();
            for (int i = 1; i <= 10; i++)
            {
                if (db.Find<Highscore>(i) == null)
                {
                    db.Insert(new Highscore(i, "Unregistered", 0, 0, 1, 1));
                }
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            RefreshScreen();
        }

        //private void TryAgainBtn_Click(object sender, System.EventArgs e)
        //{
        //    RandomNumbers();
        //}

        //private void AnswerBtn_Click(object sender, System.EventArgs e)
        //{

        //    CheckAnswer();
        //    //throw new System.NotImplementedException();
        //}

        //int first = 0;
        //int second = 0;

        //private void RandomNumbers()
        //{
        //    Random random = new Random();
        //    TextView firstNumber = (TextView)FindViewById(Resource.Id.first_number);
        //    TextView secondNumber = (TextView)FindViewById(Resource.Id.second_number);
        //    first = random.NextInt(10);
        //    second = random.NextInt(10);
        //    firstNumber.SetText(first.ToString(), null);
        //    secondNumber.SetText(second.ToString(), null);
        //}






    }
}

