namespace TestSequence.RequestUnknown
{
    using System;

    using Newtonsoft.Json;

    internal class Utilities
    {
        [ThreadStatic]
        private static Random random;

        internal static Random Random
        {
            get
            {
                return random ?? (random = new Random());
            }
        }

        internal static string SerializeObject<T>(T source)
        {
            return JsonConvert.SerializeObject(source);
        }

        internal static T DeserializeObject<T>(string source)
        {
            return JsonConvert.DeserializeObject<T>(source);
        }
    }
}
