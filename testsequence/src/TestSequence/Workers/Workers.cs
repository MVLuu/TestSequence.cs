namespace TestSequence.Workers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Launching worker (task) to execute action
    /// </summary>
    internal class Workers
    {
        /// <summary>
        /// Defining ConcurrentDictionary initial capacity for 'elapsedTime'.
        /// </summary>
        private const int InitialElapsedTimeCapacity = 101;

        /// <summary>
        /// Get the number of processor on the running systems. 
        /// For ConcurrentDictionary parameter.
        /// </summary>
        private static readonly int NumProcs = Environment.ProcessorCount;

        /// <summary>
        /// Set the concurrency level for ConcurrentDictionary.
        /// </summary>
        private static readonly int ConcurrencyLevel = NumProcs * 2;

        /// <summary>
        /// Use for performance time collection for how long it takes from start to end of Action.
        /// Note: There is a small chance that the ConcurrentDictionary key might be duplicate if two or more tasks are executed 
        /// simultaneously on multiple CPU cores.  If you hit this issue, see me and get a prize.
        /// </summary>
        private readonly ConcurrentDictionary<TimeSpan, TimeSpan> elapsedTime = new ConcurrentDictionary<TimeSpan, TimeSpan>(
            ConcurrencyLevel,
            InitialElapsedTimeCapacity);

        /// <summary>
        /// Local stopwatch for performance calculation.
        /// </summary>
        private readonly Stopwatch stopWatchLocal = new Stopwatch();

        /// <summary>
        /// Number of total customer / actions to execute.
        /// </summary>
        private int waitingCustomers;

        /// <summary>
        /// Current number of active running worker (task)
        /// </summary>
        private int numberOfActiveTask;

        /// <summary>
        /// The action (method) to execute.
        /// </summary>
        private Action action;

        /// <summary>
        /// Keep track of number of loops in the while statement. Is used to prevent high CPU waste due to rapid while loop statement.
        /// </summary>
        private int numberOfLoopsWhileWaitingForFreeWorker;

        /// <summary>
        /// Start worker(s) with parameters.
        /// </summary>
        /// <param name="numberOfConcurrentTasks">
        /// Number of expected concurrent workers (tasks).
        /// </param>
        /// <param name="numberOfActions">
        /// Total number of actions to execute.
        /// </param>
        /// <param name="workerAction">
        /// Action to execute.
        /// </param>
        /// <param name="growthType">
        /// Define the ramp-up pattern for test (default: linear) (supported types: linear, exponential, instant, cubic)
        /// </param>
        internal void Start(int numberOfActions, Action workerAction, int numberOfConcurrentTasks = 3, GrowthType growthType = null)
        {
            Logs.Log.Info(string.Format("Start Workers.Start {0}", workerAction.Method.Name));

            this.waitingCustomers = numberOfActions;

            this.action = workerAction;

            if (growthType == null)
            {
                growthType = GrowthType.Linear;
            }

            // Setting a max limit on number of concurrent workers.
            const int MaxNumberOfWorkers = 1212;

            if (numberOfConcurrentTasks > MaxNumberOfWorkers)
            {
                throw new Exception("Can not exceed maximum supported number of tasks '1212'");
            }

            // During start of test, define the ramp-up type.
            RampupHelper.Start(growthType, numberOfConcurrentTasks);

            Logs.Log.Debug(string.Format("Current waiting customers: {0} for Action: {1}", this.waitingCustomers, workerAction.Method.Name));

            this.stopWatchLocal.Start();

            // Keep trying to execute workers while there are customers in line.
            while (this.waitingCustomers > 0)
            {
                Interlocked.Increment(ref this.numberOfLoopsWhileWaitingForFreeWorker);

                this.Engine();

                // Protection against run away loops thus eating CPU.
                if (this.numberOfLoopsWhileWaitingForFreeWorker > 400000)
                {
                    Thread.Sleep(1);
                }
            }

            // Waiting for all active tasks to finish before continuing.
            while (this.numberOfActiveTask > 0)
            {
                Thread.Sleep(1);
            }

            this.stopWatchLocal.Stop();

            var avg = new List<TimeSpan>();

            // Calculate the average time for the test run.
            foreach (var timeSpan in this.elapsedTime)
            {
                var k = timeSpan.Key;
                var v = timeSpan.Value;
                avg.Add(v - k);
            }

            var average = Math.Round(avg.Average(innerList => innerList.Milliseconds), 2);

            if (average > 1000)
            {
                Logs.Log.Warn("WARNING WARNING: Average Requests took longer than 1 second.");
            }

            Logs.Log.Perf(
                string.Format(
                    "Avg (ms): {0}, MaxConcurrentWorker: {1}, #OfTests: {2} Name: {3} ",
                    average,
                    numberOfConcurrentTasks,
                    numberOfActions,
                    workerAction.Method.Name));

            Logs.Log.Info(string.Format("End Workers.Start {0}", workerAction.Method.Name));
        }

        /// <summary>
        /// Main worker code to execute action.
        /// </summary>
        private void Engine()
        {
            // Keeping actual active workers and expected workers synchronize.
            // AND only start worker if there are waiting customers.
            while (this.numberOfActiveTask < RampupHelper.SuggestedConrurrentTask && this.waitingCustomers > 0)
            {
                // Resetting run away CPU loop.
                this.numberOfLoopsWhileWaitingForFreeWorker = 0;

                // Assume worker has started and decrement number of waiting customer.
                Interlocked.Decrement(ref this.waitingCustomers);

                // Assume worker has started and increment active worker count.
                Interlocked.Increment(ref this.numberOfActiveTask);

                // Start new task and beginning of performance sensitive path.
                // Using Task.Factory.StartNew instead of Task.Run, faster at spawning new task.
                Task.Factory.StartNew(
                    () =>
                    {
                        Logs.Log.Debug(string.Format("ActiveTask(s): {0}", this.numberOfActiveTask));
                        Debug.WriteLine("ActiveTask(s): " + this.numberOfActiveTask);
                        Debug.WriteLine("ActiveTask(s): " + this.action.GetMethodInfo().Name);

                        var ts1 = this.stopWatchLocal.Elapsed;

                        try
                        {
                            this.action.Invoke();
                        }
                        catch (Exception e)
                        {
                            this.numberOfActiveTask = 0;
                            this.waitingCustomers = 0;
                            throw e;
                        }

                        var ts2 = this.stopWatchLocal.Elapsed;

                        this.elapsedTime[ts1] = ts2;

                        // Very last thing to do before finishing work. Mark worker done.
                        Interlocked.Decrement(ref this.numberOfActiveTask);
                    });

                // End performance sensitive path

                // ST - Current suggested number of active workers (tasks)
                // WC - Waiting Customers
                Logs.Log.Debug(string.Format("SuggestedTask(s): {0} WaitingCustomer(s): {1} ", RampupHelper.SuggestedConrurrentTask, this.waitingCustomers));
            }
        }
    }
}