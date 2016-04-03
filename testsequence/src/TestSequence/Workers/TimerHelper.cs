namespace TestSequence.Workers
{
    using System;
    using System.Timers;

    /// <summary>
    /// The timer class to make it a bit quicker to do common timer 
    /// </summary>
    internal class TimerHelper : IDisposable
    {
        /// <summary>
        /// Timer to start triggering of actions.
        /// </summary>
        private readonly Timer timer = new Timer(1000);

        /// <summary>
        /// Action to execute at each interval
        /// </summary>
        private readonly Action localAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerHelper"/> class with starting parameters for the timer.
        /// </summary>
        /// <param name="action">
        /// Method name to run at each interval.
        /// </param>
        internal TimerHelper(Action action)
        {
            this.localAction = action;
            this.Start();
        }

        /// <summary>
        /// Stopping timer.
        /// </summary>
        internal void Stop()
        {
            this.timer.Stop();
            this.timer.Close();
            this.timer.Dispose();
        }

        /// <summary>
        /// Starting timer.
        /// </summary>
        private void Start()
        {
            this.timer.Elapsed += this.TimerElapsed;
            this.timer.Enabled = true;
        }

        /// <summary>
        /// Action to invoke at each timer intervals.
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">elapsed event arguments</param>
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            this.localAction.Invoke();
        }

        public void Dispose()
        {
            Console.WriteLine("Releasing Managed Resources");
            if (this.timer != null)
            {
                this.timer.Dispose();
            }
        }
    }
}
