using Android.App;
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
        }

        int first = 0;
        int second = 0;

        private void RandomNumbers()
        {
            Random random = new Random();
            TextView firstNumber = (TextView) FindViewById(Resource.Id.first_number);
            TextView secondNumber = (TextView)FindViewById(Resource.Id.second_number);
            first = random.NextInt(100);
            second = random.NextInt(100);
            firstNumber.SetText(first);
            secondNumber.SetText(second);
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
                status.Text = "Correct!";
            }
            else status.Text = "Wrong!";
        }


    }
}

