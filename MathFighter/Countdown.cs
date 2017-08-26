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

namespace MathFighter


{

    public delegate void TickEvent(long millisUntilFinished);
    public delegate void FinishEvent();
    public class Countdown : CountDownTimer
    {
        public event TickEvent Tick;
        public event FinishEvent Finish;

        public Countdown(long totalTime, long interval) : base(totalTime, interval)
        {
        }

        public override void OnTick(long millisUntilFinished)
        {
            Tick?.Invoke(millisUntilFinished);
        }

        public override void OnFinish()
        {
            Finish?.Invoke();
        }
    }
}