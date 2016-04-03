namespace TestSequence
{
    internal enum TestType
    {
        /// <summary>
        /// Indicate a test type where the data should generate a '200' response.
        /// </summary>
        Positive,

        /// <summary>
        /// Indicate a test type where the data should generate error response.
        /// </summary>
        Negative,

        ////BoundaryMaximumAbove,

        ////BoundaryMaximumBelow,

        /// <summary>
        /// Indicate a boundary test type where the data is at the maximum limit.
        /// </summary>
        BoundaryMaximumAt,

        ////BoundaryMinimumAbove,

        ////BoundaryMinimumBelow,

        BoundaryMinimumAt,

        ////PositiveBoundary,

        SmartFuzz,

        Fuzz,

        ////CrossSiteScripting,

        ////Unicode,

        ////Ascii,

        LightLoad,

        ////Overflow,

        ////TimeOut,

        ////Stress,

        ////Functional,

        QaHoneypot,

        Smoke
    }
}
