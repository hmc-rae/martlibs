using System;
using System.Diagnostics;

namespace martlib
{
    /// <summary>
    /// A wrapper for Stopwatch.
    /// </summary>
    public class Runtimer
    {
        public const string VERSION = "1.0.1";

        private const double CONV_VAL = 0.001;

        private Stopwatch timer;
        private double dt;
        private long targetMS;


        public Runtimer(long targetMS)
        {
            timer = new Stopwatch();
            this.targetMS = targetMS;
            dt = targetMS * CONV_VAL;
        }

        public Runtimer(float frameRate)
        {
            timer = new Stopwatch();

            this.targetMS = (long)(1000.0f / frameRate);
            dt = targetMS * CONV_VAL;
        }

        public void Wait()
        {
            if (timer.ElapsedMilliseconds < targetMS)
            {
                int time = (int)(targetMS - timer.ElapsedMilliseconds);
                Thread.Sleep(time);
            }
            Restart();
        }
        public void Restart()
        {
            dt = timer.ElapsedMilliseconds * CONV_VAL;
            timer.Restart();
        }
        public void Stop()
        {
            dt = timer.ElapsedMilliseconds * CONV_VAL;
            timer.Stop();
        }
        public void Start()
        {
            timer.Start();
        }

        public double DeltaTime
        {
            get
            {
                return dt;
            }
        }
        public double LastFrameRate
        {
            get
            {
                return 1 / dt;
            }
        }
        public float FrameRate
        {
            get
            {
                return 1000f / targetMS;
            }
            set
            {
                this.targetMS = (long)(1000.0f / value);
            }
        }
    }
}
