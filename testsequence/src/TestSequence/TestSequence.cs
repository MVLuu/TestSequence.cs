namespace TestSequence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections.Concurrent;
    using System.Net;

    using TestSequence.Workers;
    using System.Net.Http;
    public class TestSequence<TRequestUriKeys, TRequestBody, TResponseBody>
        where TRequestUriKeys : new() 
        where TRequestBody : new() 
        where TResponseBody : new()
    {
        private static volatile ConcurrentBag<TestSequence<TRequestUriKeys, TRequestBody, TResponseBody>> instance;

        private static readonly object SyncRoot = new object();

        // Using a singletone of concurrent bag to store the test sequences.
        internal static ConcurrentBag<TestSequence<TRequestUriKeys, TRequestBody, TResponseBody>> Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (instance == null) instance = new ConcurrentBag<TestSequence<TRequestUriKeys, TRequestBody, TResponseBody>>();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        ///     The request URI keys
        /// </summary>
        public TRequestUriKeys UriKey { get; set; }

        /// <summary>
        ///     The request body
        /// </summary>
        public TRequestBody Request { get; set; }

        /// <summary>
        ///     The deserialized response body
        /// </summary>
        private TResponseBody Response;

        /// <summary>
        ///     The accumulated list of responses.
        /// </summary>
        public List<TResponseBody> ResponseList { get; set; }

        /// <summary>
        ///     A test sequence GUID to identify each test sequence.
        /// </summary>
        internal Guid TestSequenceGuid;

        /// <summary>
        ///     The ExtendedHttpClient that is passed on to each http client request.
        /// </summary>
        public HttpClient HttpClient { get; set; }

        /// <summary>
        ///     Gets or sets the test type to run.
        /// </summary>
        internal TestType TestType { get; set; }

        /// <summary>
        ///     Gets or sets the number of concurrent actions to execute.
        /// </summary>
        public int NumberOfConcurrentTasks { get; set; }

        /// <summary>
        ///     Get or sets the number of copy of test sequence to make for light load test.
        /// </summary>
        public int NumberOfTestSequences { get; set; }

        /// <summary>
        ///     Get or sets the ramp-up type for workers during execution.
        /// </summary>
        internal GrowthType GrowthType { get; set; }

        /// <summary>
        ///     Gets or sets the property to store description of test session.
        /// </summary>
        internal string Description { get; set; }

        /// <summary>
        ///     Should errors from the http response be ignored?
        /// </summary>
        public bool IgnoreErrorResponse { get; set; }

        /// <summary>
        ///     The expected HTTP response status code from the HTTP client request.
        /// </summary>
        public HttpStatusCode ExpectedStatusCode { get; set; }

        /// <summary>
        ///     Test sequence bag for saving all the requests.
        /// </summary>
        public ConcurrentBag<TestSequence<TRequestUriKeys, TRequestBody, TResponseBody>> TestSequenceBagRequest { get; set; }

        /// <summary>
        ///     Test sequence bag for saving all the responses.
        /// </summary>
        public ConcurrentBag<TestSequence<TRequestUriKeys, TRequestBody, TResponseBody>> TestSequenceBagResponse { get; set; }

        /// <summary>
        ///     The action method to execute.
        /// </summary>
        public Action Action { get; set; }

        /// <summary>
        ///     Instantiating the test sequence with new objects and generating a new TestSequence GUID for identifying this test.
        /// </summary>
        public TestSequence()
        {
            // Initialize concurrent bag to store requests.
            this.TestSequenceBagRequest = new ConcurrentBag<TestSequence<TRequestUriKeys, TRequestBody, TResponseBody>>();

            // Initialize concurrent bag to store responses.
            this.TestSequenceBagResponse = new ConcurrentBag<TestSequence<TRequestUriKeys, TRequestBody, TResponseBody>>();
            
            // A unique GUID for identifying test sequence.
            this.TestSequenceGuid = Guid.NewGuid();

            // Setting default as 65 for number of test sequences to generate.
            this.NumberOfTestSequences= 65;

            // Default number of concurrent requests made at one time.
            this.NumberOfConcurrentTasks = 3;

            // Default growth type for test workers.
            this.GrowthType = GrowthType.Linear;
        }

        /// <summary>
        ///     Test to build and execute QA Honeypot test sequence.
        /// </summary>
        /// <returns>A list of responses from each of the QA Honeypot test.</returns>
        public List<TResponseBody> QaHoneypot()
        {
            return this.BuildTestSequence(TestType.QaHoneypot);
        }

        /// <summary>
        ///     A basic test to make a request and confirm expected succesful response.
        /// </summary>
        /// <returns>A list of responses from the smoke test, in this case it is just one item in the list.</returns>
        public List<TResponseBody> Smoke()
        {
            return this.BuildTestSequence(TestType.Smoke);
        }

        /// <summary>
        ///     Test to build smart fuzz data based on data type and execute the requests.
        /// </summary>
        /// <returns>A list of responses from the requests.</returns>
        public List<TResponseBody> SmartFuzz()
        {
            return this.BuildTestSequence(TestType.SmartFuzz);
        }

        /// <summary>
        ///     Test to build boundary test based on data type and execute the requests.
        /// </summary>
        /// <returns>A list of responses from the requests.</returns>
        public List<TResponseBody> BoundaryMax()
        {
            return this.BuildTestSequence(TestType.BoundaryMaximumAt);
        }

        /// <summary>
        ///     Build a test sequence for lightly load test the endpoint.
        ///     Specifications:   
        ///     Ramp-up: exponential grow to maximum concurrent connection.
        ///     Number of requests: 30
        ///     Maximum number of concurrent requests: 3
        /// </summary>
        /// <returns>A list of responses from the requests.</returns>
        public List<TResponseBody> LightLoad()
        {
            return this.BuildTestSequence(TestType.LightLoad);
        }

        /// <summary>
        ///     Build a test sequence for minimum boundary testing of all properties.
        /// </summary>
        /// <returns>A list of responses from the requests.</returns>
        public List<TResponseBody> BoundaryMin()
        {
            return this.BuildTestSequence(TestType.BoundaryMinimumAt);
        }

        /// <summary>
        ///     Build the test sequence and start worker processes to execute Action based on generated requests.
        /// </summary>
        /// <param name="testType">The type of tests to generate.</param>
        /// <returns>The list of responses from the request action.</returns>
        private List<TResponseBody> BuildTestSequence(TestType testType)
        {
            foreach (var testSequence in new RequestGenerator<TRequestUriKeys, TRequestBody, TResponseBody>().CreateRequests(this, testType))
            {
                this.TestSequenceBagRequest.Add(testSequence);
            }

            if (!this.TestSequenceBagRequest.IsEmpty)
            {
                new Workers.Workers().Start(this.TestSequenceBagRequest.Count, this.Action, this.NumberOfConcurrentTasks, this.GrowthType);
            }

            return this.TestSequenceBagResponse.Select(testSequence => testSequence.Response).ToList();
        }
    }
}