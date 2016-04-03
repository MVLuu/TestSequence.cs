namespace TestSequence.Workers
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Class to manage the ramp-up of action for worker process.
    /// </summary>
    internal static class RampupHelper
    {
        /// <summary>
        /// Counter for tracking growth intervals
        /// </summary>
        private static int growthCounter = 1;

        /// <summary>
        /// Stop the timer when reaching the max value.
        /// </summary>
        private static int stopThreadWhenValueExceeds;

        /// <summary>
        /// Timer helper object.
        /// </summary>
        private static TimerHelper timerHelper;

        /// <summary>
        /// The current suggested concurrent task value (number of tasks that should be running).
        /// </summary>
        private static int suggestedConcurrentTask;

        /// <summary>
        /// Gets the suggested concurrent task value.
        /// </summary>
        internal static int SuggestedConrurrentTask
        {
            get
            {
                return suggestedConcurrentTask;
            }
        }

        /// <summary>
        /// Start ramp-up helper.
        /// </summary>
        /// <param name="growthType">The ramp-up growth pattern.</param>
        /// <param name="stopValue">If current SuggestedConcurrentTaskValue equal or exceed
        /// stopThreadWhenValueExceeds, stop timer and use stopThreadWhenValueExceeds.</param>
        internal static void Start(GrowthType growthType, int stopValue)
        {
            stopThreadWhenValueExceeds = stopValue;

            if (growthType == GrowthType.Instant)
            {
                suggestedConcurrentTask = stopThreadWhenValueExceeds;
            }

            if (growthType == GrowthType.Linear)
            {
                Task.Factory.StartNew(() => { timerHelper = new TimerHelper(LinearGrowth); }).Wait();
            }

            if (growthType == GrowthType.Cubic)
            {
                Task.Factory.StartNew(() => { timerHelper = new TimerHelper(CubicGrowth); }).Wait();
            }

            if (growthType == GrowthType.Exponential)
            {
                Task.Factory.StartNew(() => { timerHelper = new TimerHelper(ExponentialGrowth); }).Wait();
            }
        }

        /// <summary>
        /// Running ramp-up using linear growth pattern.
        /// </summary>
        private static void LinearGrowth()
        {
            suggestedConcurrentTask = growthCounter++;
            CheckStopTimer();
        }

        /// <summary>
        /// Running ramp-up using exponential growth pattern.
        /// </summary>
        private static void ExponentialGrowth()
        {
            suggestedConcurrentTask = Convert.ToInt32(Math.Pow(2, growthCounter++));

            CheckStopTimer();
        }

        /// <summary>
        /// Cubic growth.
        /// </summary>
        private static void CubicGrowth()
        {
            var z = growthCounter++;
            suggestedConcurrentTask = z * z * z;
            CheckStopTimer();
        }

        /// <summary>
        /// If current ramp-up value equal or exceed expected value then stop timer.
        /// </summary>
        private static void CheckStopTimer()
        {
            if (SuggestedConrurrentTask >= stopThreadWhenValueExceeds)
            {
                timerHelper.Stop();

                suggestedConcurrentTask = stopThreadWhenValueExceeds;
            }
        }
    }

}
