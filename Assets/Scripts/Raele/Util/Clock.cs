using System;
using System.Diagnostics;
using System.Timers;

namespace Raele.Util
{
    /**
     * The Clock is a timer-like class that will execute given callback every once in a predetermined interval of time;
     * the only difference here is that this class allows the client to pause the clock and resume again at any time and
     * it will continue at the same point in the timer where it paused.
     */
    public class Clock
    {
        public long Interval
        {
            get => this.m_interval;
            set
            {
                bool wasRunning = !this.IsPaused;
                this.Pause(); // We pause to avoid any scenario on which it could concurrently invoke the interval
                              // callback here and by the Timer at the same time
                this.m_interval = value;
                
                if (this.m_partial >= this.m_interval)
                {
                    this.m_partial = 0;
                    this.OnInterval();
                }

                wasRunning.Then(this.Start);
            }
        }

        public Action OnInterval { get; set; }
        public bool IsPaused => !this.m_watch.IsRunning;

        public long ProgressMs => this.IsPaused.Then(this.m_partial).Otherwise(this.m_watch.ElapsedMilliseconds);

        private Stopwatch m_watch = null;
        private Timer m_timer = null;
        private long m_interval = 0;
        private long m_partial = 0;

        public Clock(long intervalMs, Action onInterval)
        {
            this.m_interval = intervalMs;
            this.m_watch = new Stopwatch();
            this.m_timer = new Timer();
            this.m_timer.Elapsed += (dummy0, dummy1) => this.OnElapsed();
            this.OnInterval = onInterval;
        }

        /**
         * Starts the timer or resumes it if paused.
         */
        public void Start()
        {
            this.m_watch.Reset();
            this.m_watch.Start();
            this.m_timer.Interval = this.m_interval - this.m_partial;
            this.m_timer.Start();
        }

        /**
         * Stops the timer. Calling Start then will continue the timer at the same point in time where it were when this
         * method was called.
         */
        public void Pause()
        {
            this.m_timer.Stop();
            this.m_watch.Stop();
            this.m_partial = this.m_watch.ElapsedMilliseconds;
        }

        /**
         * Stops the timer and return it to its initial state.
         */
        public void Reset(bool andRestart = false)
        {
            this.Pause();
            this.m_partial = 0;

            andRestart.Then(this.Start);
        }

        /**
         * This will change the 
         */
        public void SetIntervalProportionally(long interval)
        {
            bool wasRunning = !this.IsPaused;
            this.Pause();
            double proportionalProgress = this.m_partial.AsDouble() / this.m_interval;
            this.m_partial = (long) ((this.m_interval = interval) * proportionalProgress);
            this.m_partial = Math.Min(this.m_partial, this.m_interval - 1);
            wasRunning.Then(this.Start);
        }

        private void OnElapsed()
        {
            this.m_partial = 0;
            this.Start();
            this.OnInterval();
        }
    }
}
