using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Preferences;
using Android.Graphics;

namespace MathFighter
{
    [Activity(Label = "MathFighter", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private readonly string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "highscore.db3");
        private string imgPath;

        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;
        private TextView tvTema;
        private TextView tvVanskelighetsgrad;
        private TextView tvPlayer;
        private ImageView ivPlayer;

        private DatabaseManager dbManager;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ActionBar.SetDisplayShowHomeEnabled(false);
            ActionBar.SetDisplayShowTitleEnabled(false);

            ActionBar.SetCustomView(Resource.Layout.actionbar_main);
            ActionBar.SetDisplayShowCustomEnabled(true);

            SetContentView(Resource.Layout.activity_main);

            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            editor = prefs.Edit();
            editor.PutString("dbPath", dbPath);
            editor.Apply();

            dbManager = new DatabaseManager(dbPath);
            dbManager.CreateTable();

            var highscoreBtn = FindViewById<Button>(Resource.Id.main_btn_highscore);
            var startBtn = FindViewById<Button>(Resource.Id.btn_main_start);
            var subjectsBtn = FindViewById<Button>(Resource.Id.btn_main_tema);
            var btnVanskelighetsgrad = FindViewById<Button>(Resource.Id.btn_main_vanskelighetsgrad);

            highscoreBtn.Click += HighscoreBtn_Click;
            startBtn.Click += StartBtn_Click;
            subjectsBtn.Click += SubjectsBtn_Click;
            btnVanskelighetsgrad.Click += BtnVanskelighetsgrad_Click;

            tvPlayer = FindViewById<TextView>(Resource.Id.tv_actionbar_player_name);
            tvPlayer.Text = prefs.GetString("player", null);
            ivPlayer = FindViewById<ImageView>(Resource.Id.iv_actionbar_player);
            ivPlayer.Click += IvPlayer_Click;

            RefreshScreen();
        }

        private void IvPlayer_Click(object sender, System.EventArgs e)
        {
            var transaction = FragmentManager.BeginTransaction();
            var playerSettingsDialog = new PlayerSettingsDialog(this);
            playerSettingsDialog.Show(transaction, "player_settings");
            playerSettingsDialog.DialogClosed += delegate
            {
                tvPlayer.Text = prefs.GetString("player", null);
                RefreshScreen();
            };
        }



        private void BtnVanskelighetsgrad_Click(object sender, System.EventArgs e)
        {
            var difficultyId = prefs.GetInt("difficultyId", 0);
            if (difficultyId >= 3)
            {
                editor.PutInt("difficultyId", 1);
            }
            else editor.PutInt("difficultyId", difficultyId + 1);
            editor.Apply();
            SetDifficulty();
        }

        private void SetDifficulty()
        {
            string difficulty;
            switch (prefs.GetInt("difficultyId", 0))
            {
                case 1:
                    difficulty = "Lett";
                    break;
                case 2:
                    difficulty = "Middels";
                    break;
                case 3:
                    difficulty = "Vanskelig";
                    break;
                default:
                    difficulty = "Lett";
                    editor.PutInt("difficultyId", 1);
                    break;
            }
            tvVanskelighetsgrad.SetText(difficulty, null);
        }

        private void SubjectsBtn_Click(object sender, System.EventArgs e)
        {
            var subjects = new Intent(this, typeof(SubjectActivity));
            StartActivity(subjects);
        }

        private void RefreshScreen()
        {
            Java.IO.File file = null;
            imgPath = prefs.GetString("imgPath", null);

            if (imgPath != null)
            {
                file = new Java.IO.File(imgPath);
            }

            var playerImg = BitmapFactory.DecodeResource(Resources, Resource.Drawable.Adrian);
            int width = ivPlayer.Height;
            int height = Resources.DisplayMetrics.HeightPixels;
            if (file != null)
            {
                playerImg = playerImg.PreparePlayerImage(width, height, file.Path);
            }
            else playerImg.PreparePlayerImage(width, height);

            ivPlayer.SetImageBitmap(playerImg);

            tvTema = FindViewById<TextView>(Resource.Id.tv_main_tema);
            tvTema.SetText(prefs.GetString("subject", null) ?? "Ikke valgt", null);

            tvVanskelighetsgrad = FindViewById<TextView>(Resource.Id.tv_main_vanskelighetsgrad);
            SetDifficulty();
        }

        private void StartBtn_Click(object sender, System.EventArgs e)
        {
            if (prefs.GetInt("subjectId", 0) != 0)
            {
                Intent quiz = new Intent(this, typeof(QuizActivity));
                StartActivity(quiz);
            }
            else
            {
                var chooseSubjectAlert = new AlertDialog.Builder(this)
                    .SetTitle("Vent litt!")
                    .SetMessage("Du må velge et tema før du kan starte et spill :)")
                    .SetPositiveButton("OK", (ok, args) =>
                    {
                    })
                    .Show();
            }

        }

        private void HighscoreBtn_Click(object sender, System.EventArgs e)
        {
            var intent = new Intent(this, typeof(TopPlayersActivity));
            StartActivity(intent);
        }


        protected override void OnResume()
        {
            base.OnResume();
            RefreshScreen();
        }
    }
}

