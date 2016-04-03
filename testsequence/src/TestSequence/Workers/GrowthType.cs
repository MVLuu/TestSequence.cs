namespace TestSequence.Workers
{
    /// <summary>
    /// Determine the way actions are executed at startup of TestRunner library.
    /// </summary>
    internal class GrowthType
    {
        /// <summary>
        /// Instantly load the max actions.
        /// </summary>
        private static readonly GrowthType InstantGrowthType = new GrowthType();

        /// <summary>
        /// Linear growth of actions per second.
        /// </summary>
        private static readonly GrowthType LinearGrowthType = new GrowthType();

        /// <summary>
        /// Exponential growth of actions per second.
        /// </summary>
        private static readonly GrowthType ExponentialGrowthType = new GrowthType();

        /// <summary>
        /// Cubic growth of actions per second.
        /// </summary>
        private static readonly GrowthType CubicgGrowthType = new GrowthType();

        /// <summary>
        /// Gets growth type representing instant
        /// </summary>
        internal static GrowthType Instant
        {
            get
            {
                return InstantGrowthType;
            }
        }

        /// <summary>
        /// Gets growth type representing linear
        /// </summary>
        internal static GrowthType Linear
        {
            get
            {
                return LinearGrowthType;
            }
        }

        /// <summary>
        /// Gets growth type representing exponential
        /// </summary>
        internal static GrowthType Exponential
        {
            get
            {
                return ExponentialGrowthType;
            }
        }

        /// <summary>
        /// Gets growth type representing cubic
        /// </summary>
        internal static GrowthType Cubic
        {
            get
            {
                return CubicgGrowthType;
            }
        }
    }
}
