using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MathFighter
{
    public class Calculator
    {
        private static ISharedPreferences prefs;


        public Calculator(Context context)
        {
            prefs = PreferenceManager.GetDefaultSharedPreferences(context);
        }

        //Currently just a method to multiply two numbers. TODO: Expand to handle different math operations, not just multiplication.
        public int Multiply(int x, int i)
        {
            var antallSpm = prefs.GetInt("questions", 10);
            int answer = 0;
            if (i <= antallSpm)
            {
                answer = x * i;
            }
            return answer;
        }

        public double Divide(int x, int y)
        {
            return (double)x / y;
        }

        public int Add(params int[] numbers)
        {
            int answer = 0;
            for (int i = 0; i < numbers.Length; i++)
            {
                answer += numbers.ElementAt(i);
            }
            return answer;
        }

        public int Subtract(params int[] numbers)
        {
            int answer = 0;
            for (int i = 0; i < numbers.Length; i++)
            {
                answer -= numbers.ElementAt(i);
            }
            return answer;
        }

        public void MixedSubjects(string operation, int x = 0, int y = 0, params int[] integers)
        {
            switch (operation)
            {
                case "*":
                    Multiply(x, y);
                    break;
                case "/":
                    Divide(x, y);
                    break;
                case "+":
                    Add(integers);
                    break;
                case "-":
                    Subtract(integers);
                    break;
            }
        }
    }
}
