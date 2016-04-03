namespace TestSequence
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net;

    using global::TestSequence.RequestUnknown;

    using TestSequence.Workers;

    /// <summary>
    /// Generic class for building web requests and processing web responses.
    /// </summary>
    /// <typeparam name="TUriKey">The request object.</typeparam>
    /// <typeparam name="TRequestBody">The response object.</typeparam>
    /// <typeparam name="TResponseBody"></typeparam>
    internal class RequestGenerator<TUriKey, TRequestBody, TResponseBody>
        where TUriKey : new() where TRequestBody : new() where TResponseBody : new()
    {
        /// <summary>
        /// Getting the system type of TRequest.
        /// </summary>
        private readonly Type tRequest = typeof(TRequestBody);

        /// <summary>
        /// Getting the system type of TResponse.
        /// </summary>
        private readonly Type tUriKey = typeof(TUriKey);

        /// <summary>
        /// A generic storage bag for building requests and saving responses.
        /// </summary>
        private readonly ConcurrentBag<TestSequence<TUriKey, TRequestBody, TResponseBody>> sequenceBag = new ConcurrentBag<TestSequence<TUriKey, TRequestBody, TResponseBody>>();

        private TestSequence<TUriKey, TRequestBody, TResponseBody> testSequence;

        /// <summary>
        /// Main method to build test requests, run tests, execute tests and save responses.
        /// </summary>
        /// <param name="sequence">Test sessions containing web requests.</param>
        /// <param name="testType">The test type to run.</param>
        //internal ConcurrentBag<TestSequence<TUriKey, TRequest, TResponse>> CreateRequests<TUriKey, TRequest, TRespond>(TestSequence<TUriKey, TRequest, TRespond> sequence, TestType testType = TestType.Smoke)
        internal ConcurrentBag<TestSequence<TUriKey, TRequestBody, TResponseBody>> CreateRequests(TestSequence<TUriKey, TRequestBody, TResponseBody> sequence, TestType testType = TestType.Negative)
        {
            if (sequence == null)
            {
                throw new Exception(string.Format("No sequence found. {0}:{1}", this.tRequest.Name, this.tUriKey.Name));
            }

            this.testSequence = sequence;

            this.testSequence.HttpClient = sequence.HttpClient;

            this.BuildRequests(testType);

            return this.sequenceBag;
        }

        /// <summary>
        /// Building the generic type request.
        /// </summary>
        /// <param name="testType">The test type to determine the type of test request to build.</param>
        /// <returns>A concurrent bag of generated test requests.</returns>
        private void BuildRequests(TestType testType)
        {
            if (testType == TestType.Negative)
            {
                this.NegativeTest();
            }

            if (testType == TestType.Smoke)
            {
                this.SmokeTest();
            }

            if (testType == TestType.QaHoneypot)
            {
                this.QaHoneypot();
            }

            if (testType == TestType.SmartFuzz)
            {
                this.QaHoneypot();
            }

            if (testType == TestType.Fuzz)
            {
                this.QaHoneypot();
            }

            if (testType == TestType.BoundaryMinimumAt)
            {
                this.MinimumBoundaryTest();
            }

            if (testType == TestType.BoundaryMaximumAt)
            {
                this.MaximumBoundaryTest();
            }

            if (testType.Equals(TestType.LightLoad))
            {
                this.LightLoad();
            }
        }

        /// <summary>
        ///     Make multiple copies of test sequence and run the test sequences.
        /// </summary>
        private void LightLoad()
        {
            for (var i = 0; i < this.testSequence.NumberOfTestSequences; i++)
            {
                this.sequenceBag.Add(new TestSequence<TUriKey, TRequestBody, TResponseBody> { HttpClient = this.testSequence.HttpClient, Request = this.testSequence.Request, UriKey = this.testSequence.UriKey, TestSequenceGuid = Guid.NewGuid(), Description = "Light load", TestType = TestType.LightLoad, IgnoreErrorResponse = false, ExpectedStatusCode = HttpStatusCode.NoContent });
            }

            this.testSequence.NumberOfConcurrentTasks = 5;

            this.testSequence.GrowthType = GrowthType.Exponential;
        }

        /// <summary>
        ///     Build a single test request.
        /// </summary>
        private void SmokeTest()
        {
            this.sequenceBag.Add(new TestSequence<TUriKey, TRequestBody, TResponseBody> { HttpClient = this.testSequence.HttpClient, Request = this.testSequence.Request, UriKey = this.testSequence.UriKey, Description = "Smoke test", TestType = TestType.Smoke, IgnoreErrorResponse = false, ExpectedStatusCode = HttpStatusCode.NoContent});
        }

        /// <summary>
        ///     Build test sequences based on minimal value for data type.
        /// </summary>
        private void MinimumBoundaryTest()
        {
            if (this.testSequence.UriKey != null)
            {
                this.AddToUriKeySequenceBag(new RequestsUnknown<TUriKey>(this.testSequence.UriKey).GenerateBoundaryMinimum());
            }

            if (this.testSequence.Request != null)
            {
                this.AddToRequestSequenceBag(new RequestsUnknown<TRequestBody>(this.testSequence.Request).GenerateBoundaryMinimum());
            }
        }

        /// <summary>
        ///     Build test sequences based on maximum value for data type.
        /// </summary>
        private void MaximumBoundaryTest()
        {
            if (this.testSequence.UriKey != null)
            {
                this.AddToUriKeySequenceBag(new RequestsUnknown<TUriKey>(this.testSequence.UriKey).GenerateBoundaryMaximum());
            }

            if (this.testSequence.Request != null)
            {
                this.AddToRequestSequenceBag(new RequestsUnknown<TRequestBody>(this.testSequence.Request).GenerateBoundaryMaximum());
            }
        }

        /// <summary>
        ///     Method to generate all negative tests.
        /// </summary>
        private void NegativeTest()
        {
            if (this.testSequence.UriKey != null)
            {
                this.AddToUriKeySequenceBag(new RequestsUnknown<TUriKey>(this.testSequence.UriKey).GenerateAll());
            }

            if (this.testSequence.Request != null)
            {
                this.AddToRequestSequenceBag(new RequestsUnknown<TRequestBody>(this.testSequence.Request).GenerateAll());
            }
        }

        /// <summary>
        ///     Build tests bsaed on honeypot data.
        /// </summary>
        private void QaHoneypot()
        {
            if (this.testSequence.UriKey != null)
            {
                this.AddToUriKeySequenceBag(new RequestsUnknown<TUriKey>(this.testSequence.UriKey).QaHoneypot());
            }

            if (this.testSequence.Request != null)
            {
                this.AddToRequestSequenceBag(new RequestsUnknown<TRequestBody>(this.testSequence.Request).QaHoneypot());
            }
        }

        /// <summary>
        ///     Add test to seqiun
        /// </summary>
        /// <param name="uriKeyObjects"></param>
        private void AddToUriKeySequenceBag(ConcurrentBag<RequestsUnknown<TUriKey>.RequestObject> uriKeyObjects)
        {
            foreach (var uriKey in uriKeyObjects)
            {
                this.sequenceBag.Add(new TestSequence<TUriKey, TRequestBody, TResponseBody> { HttpClient = this.testSequence.HttpClient, UriKey = (TUriKey)uriKey.Object, Description = uriKey.Description, TestType = uriKey.TestType, IgnoreErrorResponse = uriKey.IgnoreErrorResponse });
            }
        }

        private void AddToRequestSequenceBag(ConcurrentBag<RequestsUnknown<TRequestBody>.RequestObject> requestObjects)
        {
            foreach (var request in requestObjects)
            {
                this.sequenceBag.Add(new TestSequence<TUriKey, TRequestBody, TResponseBody> { HttpClient = this.testSequence.HttpClient, Request = (TRequestBody)request.Object, Description = request.Description, TestType = request.TestType, IgnoreErrorResponse = request.IgnoreErrorResponse });
            }
        }

        /// <summary>
        /// Save the response to the generic storage instance.
        /// </summary>
        internal void SaveResponse(TestSequence<TUriKey, TRequestBody, TResponseBody> sequence)
        {
            Storage<TestSequence<TUriKey, TRequestBody, TResponseBody>>.Instance.Add(sequence);
        }
    }

    /// <summary>
    /// Generic storage for TestSequence.
    /// This is where the GenericData and GenericDataList is stored.
    /// </summary>
    /// <typeparam name="TSequenceData">The data type that will be stored.</typeparam>
    internal class Storage<TSequenceData>
    {
        /// <summary>
        /// Lock semaphore
        /// </summary>
        // ReSharper disable once StaticFieldInGenericType
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// A storage instance of the test session.
        /// </summary>
        private static volatile ConcurrentBag<TSequenceData> instance;

        /// <summary>
        /// Gets an instance of test data (test session).
        /// </summary>
        internal static ConcurrentBag<TSequenceData> Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new ConcurrentBag<TSequenceData>();
                        }
                    }
                }

                return instance;
            }
        }
    }

    /// <summary>
    /// Generic bag of testData for Uri parameters, requests and responses.
    /// </summary>
    internal class GenericBag<TUriKey, TRequestBody, TResponseBody>
        where TUriKey : new() where TRequestBody : new() where TResponseBody : new()
    {
        /// <summary>
        /// The request session for storage.
        /// </summary>
        private ConcurrentBag<TestSequence<TUriKey, TRequestBody, TResponseBody>> requestsBag = new ConcurrentBag<TestSequence<TUriKey, TRequestBody, TResponseBody>>();

        /// <summary>
        /// The response session for storage.
        /// </summary>
        private ConcurrentBag<TestSequence<TUriKey, TRequestBody, TResponseBody>> responseBag = new ConcurrentBag<TestSequence<TUriKey, TRequestBody, TResponseBody>>();

        /// <summary>
        /// The test session for storage.
        /// </summary>
        private ConcurrentBag<TestSequence<TUriKey, TRequestBody, TResponseBody>> testSequence = new ConcurrentBag<TestSequence<TUriKey, TRequestBody, TResponseBody>>();

        /// <summary>
        /// Gets or sets a concurrent bag for saving requests.
        /// </summary>
        internal ConcurrentBag<TestSequence<TUriKey, TRequestBody, TResponseBody>> RequestsBag
        {
            get
            {
                return this.requestsBag;
            }

            set
            {
                this.requestsBag = value;
            }
        }

        /// <summary>
        /// Gets or sets a concurrent bag for saving responses.
        /// </summary>
        internal ConcurrentBag<TestSequence<TUriKey, TRequestBody, TResponseBody>> ResponseBag
        {
            get
            {
                return this.responseBag;
            }

            set
            {
                this.responseBag = value;
            }
        }

        /// <summary>
        /// Gets or sets a concurrent bag for saving test sessions.
        /// </summary>
        internal ConcurrentBag<TestSequence<TUriKey, TRequestBody, TResponseBody>> TestSequence
        {
            get
            {
                return this.testSequence;
            }

            set
            {
                this.testSequence = value;
            }
        }
    }

    /// <summary>
    /// The generic class for web request and response containing an array of data.
    /// </summary>
    internal class GenericDataList<TUriKey, TRequestBody, TResponseBody>
        where TUriKey : new() where TRequestBody : new() where TResponseBody : new()
    {
        /// <summary>
        /// Gets or sets request being sent to the server.
        /// </summary>
        internal TestSequence<TUriKey, TRequestBody, TResponseBody> RequestBody { get; set; }

        /// <summary>
        /// Gets or sets list of responses from the server.
        /// </summary>
        internal List<TestSequence<TUriKey, TRequestBody, TResponseBody>> ResponseBody { get; set; }

        /// <summary>
        /// Gets or sets test sequence data
        /// </summary>
        internal TestSequence<TUriKey, TRequestBody, TResponseBody> SequenceData { get; set; }
    }

    /// <summary>
    /// The generic class for web request and response.
    /// </summary>
    internal class GenericData<TUriKey, TRequestBody, TResponseBody>
        where TUriKey : new()
        where TRequestBody : new()
        where TResponseBody : new()
    {
        /// <summary>
        ///  Gets or sets generic request object.
        /// </summary>
        internal TestSequence<TUriKey, TRequestBody, TResponseBody> RequestBody { get; set; }

        /// <summary>
        /// Gets or sets generic response object.
        /// </summary>
        internal TestSequence<TUriKey, TRequestBody, TResponseBody> ResponseBody { get; set; }

        /// <summary>
        /// Gets or sets the sequence data for this session.
        /// </summary>
        internal TestSequence<TUriKey, TRequestBody, TResponseBody> SequenceData { get; set; }
    }
}
