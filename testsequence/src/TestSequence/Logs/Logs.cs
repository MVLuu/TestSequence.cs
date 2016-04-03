namespace TestSequence.Logs
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Linq;

    internal class Log
    {
        /// <summary>
        /// Setting initial dictionary capacity to help speed things up.
        /// </summary>
        private const int InitialCapacity = 10000;

        /// <summary>
        /// Using stopwatch as dictionary key.
        /// </summary>
        private static readonly Stopwatch StopWatch = new Stopwatch();

        /// <summary>
        /// Determine number of processor is in the running environment.
        /// </summary>
        private static readonly int NumProcs = Environment.ProcessorCount;

        /// <summary>
        /// Set concurrency level of dictionary by multiplying number of CPU times two.
        /// </summary>
        private static readonly int ConcurrencyLevel = NumProcs * 2;

        /// <summary>
        /// Initialize currency library.
        /// </summary>
        private static readonly ConcurrentDictionary<TimeSpan, string> ConcurrentDictionary = new ConcurrentDictionary<TimeSpan, string>(ConcurrencyLevel, InitialCapacity);

        /// <summary>
        /// A private instance of the log.
        /// </summary>
        private static readonly Log PrivateInstance = new Log();

        /// <summary>
        /// Prevents a default instance of the <see cref="Log"/> class from being created.
        /// Reset and start stopwatch.
        /// </summary>
        private Log()
        {
            StopWatch.Reset();
            StopWatch.Start();
        }

        /// <summary>
        /// Gets or sets a value indicating whether logs should be written to Console.
        /// </summary>
        internal static bool WriteToConsole { get; set; }

        /// <summary>
        /// Gets an instance of the charting log.
        /// </summary>
        internal static Log Instance
        {
            get
            {
                return PrivateInstance;
            }
        }

        /// <summary>
        /// Warn: an unexpected technical or business event happened, customers may be affected, but probably no immediate human intervention is required. On call people won't be called immediately, but support personnel will want to review these issues asap to understand what the impact is. Basically any issue that needs to be tracked but may not require immediate intervention.
        /// </summary>
        /// <param name="message">The message to append to log.</param>
        internal static void Warn(string message)
        {
            ConcurrentDictionary[StopWatch.Elapsed] = "|Warn: " + message;
        }

        /// <summary>
        /// Error: The system is in distress, customers are probably being affected (or will soon be) and the fix probably requires human intervention. The "2AM rule" applies here- if you're on call, do you want to be woken up at 2AM if this condition happens? If yes, then log it as "error".
        /// </summary>
        /// <param name="message">The message to append to log.</param>
        internal static void Error(string message)
        {
            ConcurrentDictionary[StopWatch.Elapsed] = "|Error: " + message;
        }

        /// <summary>
        /// Fail: The running code failed but handled by code, most likely a failure to complete a process.
        /// </summary>
        /// <param name="message">The message to append to log.</param>
        internal static void Fail(string message)
        {
            ConcurrentDictionary[StopWatch.Elapsed] = "|Fail: " + message;
        }

        /// <summary>
        /// Debug: just about everything that doesn't make the "info" cut... any message that is helpful in tracking the flow through the system and isolating issues, especially during the development and QA phases. We use "debug" level logs for entry/exit of most non-trivial methods and marking interesting events and decision points inside methods.
        /// </summary>
        /// <param name="message">The message to append to log.</param>
        internal static void Debug(string message)
        {
            ConcurrentDictionary[StopWatch.Elapsed] = "|Debug: " + message;
        }

        /// <summary>
        /// Info: things we want to see at high volume in case we need to forensically analyze an issue. System lifecycle events (system start, stop) go here. "Session" lifecycle events (login, logout, etc.) go here. Significant boundary events should be considered as well (e.g. database calls, remote API calls). Typical business exceptions can go here (e.g. login failed due to bad credentials). Any other event you think you'll need to see in production at high volume goes here.
        /// </summary>
        /// <param name="message">The message to append to log.</param>
        internal static void Info(string message)
        {
            ConcurrentDictionary[StopWatch.Elapsed] = "|Info: " + message;
        }

        /// <summary>
        /// Trace: we don't use this often, but this would be for extremely detailed and potentially high volume logs that you don't typically want enabled even during normal development. Examples include dumping a full object hierarchy, logging some state during every iteration of a large loop, etc.
        /// </summary>
        /// <param name="message">The message to append to log.</param>
        internal static void Trace(string message)
        {
            ConcurrentDictionary[StopWatch.Elapsed] = "|Trace: " + message;
        }

        /// <summary>
        /// Performance: Bare minimum to record performance data.
        /// </summary>
        /// <param name="message">The message to append to log.</param>
        internal static void Perf(string message)
        {
            ConcurrentDictionary[StopWatch.Elapsed] = "|Perf: " + message;
        }

        /// <summary>
        /// Writing logs to Console and system trace.
        /// </summary>
        internal static void WriteLog()
        {
            var concurrentDictionaryList = ConcurrentDictionary.Keys.ToList();

            concurrentDictionaryList.Sort();

            //// A bit faster loop by using a 'for'.
            //// TODO: Try it out.
            ////for (int i = 0; i < cDictList.Count; i++)
            ////{
            ////    var logMessage = cDictList[i] + ConcurrentDictionary[cDictList[i]];
            ////    if (WriteToConsole)
            ////    {
            ////        Console.WriteLine("Console: " + logMessage);
            ////    }
            ////    System.Diagnostics.Trace.WriteLine("Trace: " + logMessage);
            ////    string z;
            ////    ConcurrentDictionary.TryRemove(cDictList[i], out z);
            ////}
            foreach (var key in concurrentDictionaryList)
            {
                string value;
                ConcurrentDictionary.TryRemove(key, out value);

                var logMessage = key + value;

                if (WriteToConsole)
                {
                    Console.WriteLine("Console: " + logMessage);
                }

                System.Diagnostics.Trace.WriteLine("|Trace: " + logMessage);
            }
        }
    }
}
