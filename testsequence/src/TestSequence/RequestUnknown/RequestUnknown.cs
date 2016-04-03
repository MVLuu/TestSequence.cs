namespace TestSequence.RequestUnknown
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using QaHoneypot;

    internal class RequestsUnknown<T>
        where T : new()
    {
        private readonly ConcurrentBag<RequestObject> requestObjectBag = new ConcurrentBag<RequestObject>();

        private readonly string serializedWrite;

        private readonly string newSerialized;

        private string Serialized
        {
            get
            {
                return this.serializedWrite;
            }
        }

        public RequestsUnknown(T requestObject)
        {
            this.serializedWrite = JsonConvert.SerializeObject(requestObject);

            this.newSerialized = string.Copy(this.Serialized).Replace("\"0\"", "\"" + Utilities.Random.Next(1000, 100000000) + "\"");
        }

        public ConcurrentBag<RequestObject> QaHoneypot()
        {
            this.GenerateHoneypotObjects();

            return this.requestObjectBag;
        }

        public ConcurrentBag<RequestObject> GenerateAll()
        {
            this.GenerateBoundaryMinimumAtObjects();
            this.GenerateBoundaryMaximumAtObjects();
            this.GenerateHoneypotObjects();

            return this.requestObjectBag;
        }

        public ConcurrentBag<RequestObject> GenerateBoundaryMinimum()
        {
            this.GenerateBoundaryMinimumAtObjects();

            return this.requestObjectBag;
        }

        public ConcurrentBag<RequestObject> GenerateFuzz()
        {
            this.GenerateFuzzObjects();

            return this.requestObjectBag;
        }

        public ConcurrentBag<RequestObject> GenerateBoundaryMaximum()
        {
            this.GenerateBoundaryMaximumAtObjects();

            return this.requestObjectBag;
        }

        /// <summary>
        /// Populate each property within object with minimum value.
        /// </summary>
        /// <returns>A new object with properties set to minimal value.</returns>
        private void GenerateBoundaryMinimumAtObjects()
        {
            var foundDouble = this.FindFloatInJson(this.Serialized);

            if (foundDouble.Count > 0)
            {
                foreach (var item in foundDouble)
                {
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(Regex.Replace(string.Copy(this.Serialized), item, string.Empty)), Description = "Generate boundary test at 'int' minimum value. " + item.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMinimumAt });
                }

                return;
            }

            var foundGuids = this.FindGuidInJson(this.Serialized);

            if (foundGuids.Count > 0)
            {
                foreach (var foundGuid in foundGuids)
                {
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(string.Copy(this.Serialized).Replace(foundGuid.Split('|')[1], Guid.Empty.ToString())), Description = "Generate boundary test at 'minimum' GUID value. " + foundGuid.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMinimumAt });
                }

                return;
            }

            var foundDateTimes = this.FindDateTimeInJson(this.Serialized);

            if (foundDateTimes.Count > 0)
            {
                foreach (var foundDateTime in foundDateTimes)
                {
                    var serializedCopy = string.Copy(this.Serialized);
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(serializedCopy.Replace(foundDateTime.Split('|')[1], DateTime.MinValue.ToString(CultureInfo.InvariantCulture))), Description = "Generate boundary test at 'minimum' DateTime value" + foundDateTime.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMinimumAt });
                }

                return;
            }

            var foundBooleans = this.FindBoolInJson(this.Serialized);

            if (foundBooleans.Count > 0)
            {
                foreach (var foundBoolean in foundBooleans)
                {
                    var serializedCopy = string.Copy(this.Serialized);
                    var z = serializedCopy.Replace(foundBoolean.Split('|')[1], bool.FalseString);
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(z), Description = "Generate boundary test at 'minimum' bool value. " + foundBoolean.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMinimumAt });
                }

                return;
            }

            var foundIdStrings = this.FindStringContainInJson(this.Serialized, "id", true);

            if (foundIdStrings.Count > 0)
            {
                foreach (var foundIdString in foundIdStrings)
                {
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(string.Copy(this.Serialized).Replace(foundIdString.Split('|')[1], "0")), Description = "Generate boundary test at 'minimum' string containing 'id' value. " + foundIdString.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMinimumAt });
                }

                return;
            }

            var foundGuidStrings = this.FindStringContainInJson(this.Serialized, "guid");

            if (foundGuidStrings.Count > 0)
            {
                foreach (var foundStringString in foundGuidStrings)
                {
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(string.Copy(this.Serialized).Replace(foundStringString.Split('|')[1], Guid.Empty.ToString())), Description = "Generate boundary test at 'minimum' string containing 'guid' value. " + foundStringString.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMinimumAt });
                }

                return;
            }

            var foundDateStrings = this.FindStringContainInJson(this.Serialized, "date");

            if (foundDateStrings.Count > 0)
            {
                foreach (var foundDateString in foundDateStrings)
                {
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(string.Copy(this.Serialized).Replace(foundDateString.Split('|')[1], DateTime.MinValue.ToString(CultureInfo.InvariantCulture))), Description = "Generate boundary test at 'minimum' string containing 'date' value. " + foundDateString.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMinimumAt });
                }

                return;
            }

            var foundTimeStrings = this.FindStringContainInJson(this.Serialized, "time");

            if (foundTimeStrings.Count > 0)
            {
                foreach (var foundTimeString in foundTimeStrings)
                {
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(string.Copy(this.Serialized).Replace(foundTimeString.Split('|')[1], DateTime.MinValue.ToString("t"))), Description = "Generate boundary test at 'minimum' string containing 'time' value. " + foundTimeString.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMinimumAt });
                }

                return;
            }
        }

        /// <summary>
        /// Populate each property within object with fuzz data.
        /// </summary>
        /// <returns>A new object with properties set to fuzz data.</returns>
        internal void GenerateFuzzObjects()
        {
            var foundGuids = this.FindGuidInJson(this.Serialized);

            if (foundGuids.Count > 0)
            {
                foreach (var foundGuid in foundGuids)
                {
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(string.Copy(this.Serialized).Replace(foundGuid.Split('|')[1], Guid.NewGuid().ToString())), Description = "Generate fuzz test with random GUID value " + foundGuid.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMaximumAt });
                }

                return;
            }

            var foundDateTimes = this.FindDateTimeInJson(this.Serialized);

            if (foundDateTimes.Count > 0)
            {
                foreach (var foundDateTime in foundDateTimes)
                {
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(string.Copy(this.Serialized).Replace(foundDateTime.Split('|')[1], this.RandomDay().ToString(CultureInfo.InvariantCulture))), Description = "Generate fuzz test with random day time value: " + foundDateTime.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMaximumAt });
                }

                return;
            }

            var foundBooleans = this.FindBoolInJson(this.Serialized);

            if (foundBooleans.Count > 0)
            {
                foreach (var foundBoolean in foundBooleans)
                {
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(string.Copy(this.Serialized).Replace(foundBoolean.Split('|')[1], this.RandomBool().ToString())), Description = "Generate fuzz test with random bool value " + foundBoolean.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMaximumAt });
                }

                return;
            }

            var foundIntegers = this.FindFloatInJson(this.Serialized);

            if (foundIntegers.Count > 0)
            {
                foreach (var foundInt in foundIntegers)
                {
                    var serializedCopy = string.Copy(this.Serialized);
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(serializedCopy.Replace(foundInt.Split('|')[1], RandomFloat().ToString(CultureInfo.InvariantCulture))), Description = "Generate boundary test at 'maximum' int value " + foundInt.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMaximumAt });
                }

                return;
            }

            var foundIdStrings = this.FindStringContainInJson(this.Serialized, "id", true);

            if (foundIdStrings.Count > 0)
            {
                foreach (var foundIdString in foundIdStrings)
                {
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(string.Copy(this.Serialized).Replace(foundIdString.Split('|')[1], CreateInt(10))), Description = "Generate boundary test at 'maximum' string containing 'id' value." + foundIdString.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMaximumAt });
                }

                return;
            }

            var foundTimeStrings = this.FindStringContainInJson(this.Serialized, "time");

            if (foundTimeStrings.Count > 0)
            {
                foreach (var foundTimeString in foundTimeStrings)
                {
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(string.Copy(this.Serialized).Replace(foundTimeString.Split('|')[1], DateTime.MaxValue.ToString("t"))), Description = "Generate boundary test at 'maximum' string containing 'time' value." + foundTimeString.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMaximumAt });
                }

                return;
            }

            var foundDateStrings = this.FindStringContainInJson(this.Serialized, "date");

            if (foundTimeStrings.Count > 0)
            {
                foreach (var foundDateString in foundDateStrings)
                {
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(string.Copy(this.Serialized).Replace(foundDateString.Split('|')[1], DateTime.MaxValue.ToString("d"))), Description = "Generate boundary test at 'maximum' string containing 'date' value." + foundDateString.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMaximumAt });
                }

                return;
            }

            var foundStrings = this.FindStringInJson(this.newSerialized);

            if (foundStrings.Count > 0)
            {
                foreach (var foundString in foundStrings)
                {
                    var serializedCopy = string.Copy(this.newSerialized);
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(serializedCopy.Replace(foundString.Split('|')[1], CreateString(1))), Description = "Generate boundary test at 'string' maximum value. " + foundString.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMaximumAt });
                }

                return;
            }
        }

        /// <summary>
        /// Populate each property within object with maximum value.
        /// </summary>
        /// <returns>A new object with properties set to maximum value.</returns>
        internal void GenerateBoundaryMaximumAtObjects()
        {
            var foundGuids = this.FindGuidInJson(this.Serialized);

            if (foundGuids.Count > 0)
            {
                foreach (var foundGuid in foundGuids)
                {
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(string.Copy(this.Serialized).Replace(foundGuid.Split('|')[1], "ffffffff-ffff-ffff-ffff-ffffffffffff")), Description = "Generate boundary test at 'maximum' GUID value " + foundGuid.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMaximumAt });
                }

                return;
            }

            var foundDateTimes = this.FindDateTimeInJson(this.Serialized);

            if (foundDateTimes.Count > 0)
            {
                foreach (var foundDateTime in foundDateTimes)
                {
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(string.Copy(this.Serialized).Replace(foundDateTime.Split('|')[1], DateTime.MaxValue.ToString(CultureInfo.InvariantCulture))), Description = "Generate boundary test at 'maximum' DateTime value" + foundDateTime.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMaximumAt });
                }

                return;
            }

            var foundBooleans = this.FindBoolInJson(this.Serialized);

            if (foundBooleans.Count > 0)
            {
                foreach (var foundBoolean in foundBooleans)
                {
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(string.Copy(this.Serialized).Replace(foundBoolean.Split('|')[1], bool.TrueString)), Description = "Generate boundary test at 'maximum' bool value." + foundBoolean.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMaximumAt });
                }

                return;
            }

            var foundIntegers = this.FindFloatInJson(this.Serialized);

            if (foundIntegers.Count > 0)
            {
                foreach (var foundInt in foundIntegers)
                {
                    var serializedCopy = string.Copy(this.Serialized);
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(serializedCopy.Replace(foundInt.Split('|')[1], int.MaxValue.ToString(CultureInfo.InvariantCulture))), Description = "Generate boundary test at 'maximum' int value " + foundInt.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMaximumAt });
                }

                return;
            }

            var foundIdStrings = this.FindStringContainInJson(this.Serialized, "id", true);

            if (foundIdStrings.Count > 0)
            {
                foreach (var foundIdString in foundIdStrings)
                {
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(string.Copy(this.Serialized).Replace(foundIdString.Split('|')[1], CreateInt(10))), Description = "Generate boundary test at 'maximum' string containing 'id' value." + foundIdString.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMaximumAt });
                }

                return;
            }
            
            var foundTimeStrings = this.FindStringContainInJson(this.Serialized, "time");

            if (foundTimeStrings.Count > 0)
            {
                foreach (var foundTimeString in foundTimeStrings)
                {
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(string.Copy(this.Serialized).Replace(foundTimeString.Split('|')[1], DateTime.MaxValue.ToString("t"))), Description = "Generate boundary test at 'maximum' string containing 'time' value." + foundTimeString.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMaximumAt });
                }

                return;
            }

            var foundDateStrings = this.FindStringContainInJson(this.Serialized, "date");

            if (foundTimeStrings.Count > 0)
            {
                foreach (var foundDateString in foundDateStrings)
                {
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(string.Copy(this.Serialized).Replace(foundDateString.Split('|')[1], DateTime.MaxValue.ToString("d"))), Description = "Generate boundary test at 'maximum' string containing 'date' value." + foundDateString.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMaximumAt });
                }

                return;
            }

            var foundStrings = this.FindStringInJson(this.newSerialized);

            if (foundStrings.Count > 0)
            {
                foreach (var foundString in foundStrings)
                {
                    var serializedCopy = string.Copy(this.newSerialized);
                    this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(serializedCopy.Replace(foundString.Split('|')[1], CreateString(1))), Description = "Generate boundary test at 'string' maximum value. " + foundString.Split('|')[0], IgnoreErrorResponse = true, TestType = TestType.BoundaryMaximumAt });
                }

                return;
            }
        }



        public void GenerateHoneypotObjects()
        {
            //var nullRepresentation = Utilities.Random.Next(1000, 100000000);
            //var newSerialized = string.Copy(this.Serialized).Replace("\"0\"", "\"" + nullRepresentation + "\"");
            var arrayOfStringContainingGuid = this.FindStringContainInJson(string.Copy(this.newSerialized).ToLower(), "guid");

            if (arrayOfStringContainingGuid.Count > 0)
            {
                foreach (var stringContainingGuid in arrayOfStringContainingGuid)
                {
                    foreach (var honeypotRecord in Honeypot.AllGuids())
                    {
                        try
                        {
                            this.AddToRequestBag(honeypotRecord, stringContainingGuid, "all GUIDs");
                        }
                        catch (JsonReaderException jsonReaderException)
                        {
                            Debug.WriteLine(jsonReaderException);
                        }
                        catch (JsonSerializationException jsonSerializationException)
                        {
                            Debug.WriteLine(jsonSerializationException);
                        }
                    }
                }
            }

            var arrayOfStringContainingId = this.FindStringContainInJson(string.Copy(this.newSerialized).ToLower(), "id", true);

            if (arrayOfStringContainingId.Count > 0)
            {
                foreach (var stringContainingId in arrayOfStringContainingId)
                {
                    foreach (var honeypotRecord in Honeypot.AllId())
                    {
                        try
                        {
                            this.AddToRequestBag(honeypotRecord, stringContainingId, "Id");
                        }
                        catch (JsonReaderException jsonReaderException)
                        {
                            Debug.WriteLine(jsonReaderException);
                        }
                        catch (JsonSerializationException jsonSerializationException)
                        {
                            Debug.WriteLine(jsonSerializationException);
                        }
                    }
                }
            }
        }

        private void AddToRequestBag(string honeypotRecord, string stringContainingData, string identifier)
        {
            var honeypotKeyValue = honeypotRecord.Split('|');
            var honeypotKey = honeypotKeyValue[0];
            var honeypotValue = honeypotKeyValue[1];

            var guidKeyValue = stringContainingData.Split('|');
            var guidKey = guidKeyValue[0];
            var guidValue = guidKeyValue[1];

            if (guidKey.ToLower().Equals(honeypotKey.ToLower()))
            {
                this.requestObjectBag.Add(new RequestObject { Object = Utilities.DeserializeObject<T>(string.Copy(this.newSerialized).Replace(guidValue, honeypotValue)), Description = string.Format("Honeypot test, property containing {0} name, insert a honeypot test type '{1}'.", honeypotKey, identifier), IgnoreErrorResponse = true, TestType = TestType.QaHoneypot });
            }
        }

        private List<string> FindStringContainInJson(string jsonString, string searchKey, bool exactMatch = false)
        {
            var jTokenId = new List<string>();
            this.FindStringContainInJson(jsonString, searchKey, jTokenId, exactMatch);
            return jTokenId;
        }

        private void FindStringContainInJson(string jsonString, string searchKey, ICollection<string> jTokenId, bool exactMatch = false)
        {
            foreach (var jsonObject in JObject.Parse(jsonString))
            {
                var jsonObjectLocal = jsonObject;
                var value = jsonObjectLocal.Value;
                var key = jsonObjectLocal.Key;

                if (exactMatch)
                {
                    if (key.ToLower().Equals(searchKey))
                    {
                        jTokenId.Add(string.Concat(key, "|", value));
                        continue;
                    }
                }
                else
                {
                    if (key.ToLower().Contains(searchKey))
                    {
                        jTokenId.Add(string.Concat(key, "|", value));
                        continue;
                    }    
                }

                if (value.Type == JTokenType.Object)
                {
                    this.FindStringContainInJson(value.ToString(), searchKey, jTokenId);
                }
            }
        }

        private List<string> FindGuidInJson(string jsonString)
        {
            var guidList = new List<string>();
            this.FindGuidInJson(jsonString, guidList);
            return guidList;
        }

        private void FindGuidInJson(string jsonString, ICollection<string> guidList)
        {
            foreach (var jsonObject in JObject.Parse(jsonString))
            {
                var value = jsonObject.Value;
                var key = jsonObject.Key;

                Guid guidValue;

                if (Guid.TryParse(value.ToString(), out guidValue))
                {
                    guidList.Add(string.Concat(key, "|", value.ToString()));
                    continue;
                }

                if (value.Type == JTokenType.Object)
                {
                    this.FindGuidInJson(value.ToString());
                }
            }
        }

        private List<string> FindDateTimeInJson(string jsonString)
        {
            var dateTimeList = new List<string>();
            this.FindDateTimeInJson(jsonString, dateTimeList);
            return dateTimeList;
        }

        private void FindDateTimeInJson(string jsonString, List<string> dateTimeList)
        {
            foreach (var jsonObject in JObject.Parse(jsonString))
            {
                var value = jsonObject.Value;
                var key = jsonObject.Key;

                DateTime dateTimeValue;

                if (DateTime.TryParse(value.ToString(), out dateTimeValue))
                {
                    dateTimeList.Add(string.Concat(key, "|", value.ToString()));
                    continue;
                }

                if (value.Type == JTokenType.Object)
                {
                    this.FindDateTimeInJson(value.ToString());
                }
            }
        }

        private List<string> FindBoolInJson(string jsonString)
        {
            var booleanList = new List<string>();
            this.FindBoolInJson(jsonString, booleanList);
            return booleanList;
        }

        private void FindBoolInJson(string jsonString, List<string> booleanList)
        {
            foreach (var jsonObject in JObject.Parse(jsonString))
            {
                var value = jsonObject.Value;
                var key = jsonObject.Key;

                bool parseBool;

                if (bool.TryParse(value.ToString(), out parseBool))
                {
                    booleanList.Add(string.Concat(key, "|", value.ToString()));
                    continue;
                }

                if (value.Type == JTokenType.Object)
                {
                    this.FindBoolInJson(value.ToString());
                }
            }
        }

        private List<string> FindArrayInJson(string jsonString)
        {
            var arrayList = new List<string>();
            this.FindArrayInJson(jsonString, arrayList);
            return arrayList;
        }

        private void FindArrayInJson(string jsonString, List<string> arrayList)
        {
            foreach (var jsonObject in JObject.Parse(jsonString))
            {
                var value = jsonObject.Value;
                var key = jsonObject.Key;

                if (value.Type == JTokenType.Array)
                {
                    arrayList.Add(string.Concat(key, "|", value));
                    continue;
                }

                if (value.Type == JTokenType.Object)
                {
                    this.FindBoolInJson(value.ToString());
                }
            }
        }

        private List<string> FindStringInJson(string jsonString)
        {
            var guidList = new List<string>();
            this.FindGuidInJson(jsonString, guidList);
            return guidList;
        }

        private List<string> FindFloatInJson(string jsonString)
        {
            var floatList = new List<string>();
            this.FindFloatInJson(jsonString, floatList);
            return floatList;
        }

        private DateTime RandomDay()
        {
            var start = DateTime.MinValue;
            var random = Utilities.Random;
            var range = (DateTime.MaxValue - start).Days;
            return start.AddDays(random.Next(range));
        }

        static float RandomFloat()
        {
            var buffer = new byte[4];
            Utilities.Random.NextBytes(buffer);
            return BitConverter.ToSingle(buffer, 0);
        }

        private bool RandomBool()
        {
            return !(Utilities.Random.Next(0, 1) < 0.5);
        }

        private void FindFloatInJson(string jsonString, ICollection<string> floatList)
        {
            foreach (var jsonObject in JObject.Parse(jsonString))
            {
                var value = jsonObject.Value;
                var key = jsonObject.Key;

                if (value.Type == JTokenType.Float)
                {
                    floatList.Add(string.Concat(key, "|", value));
                    continue;
                }

                if (value.Type == JTokenType.Object)
                {
                    this.FindFloatInJson(value.ToString());
                }
            }
        }

        private void FindStringInJson(string jsonString, ICollection<string> floatList)
        {
            foreach (var jsonObject in JObject.Parse(jsonString))
            {
                var value = jsonObject.Value;
                var key = jsonObject.Key;

                if (value.Type == JTokenType.String)
                {
                    floatList.Add(string.Concat(key, "|", value));
                    continue;
                }

                if (value.Type == JTokenType.Object)
                {
                    this.FindFloatInJson(value.ToString());
                }
            }
        }

        private string CreateArray(int numberOfArray, string arrayValue)
        {
            var array = "\"" + arrayValue + "\", ";
            var sb = new StringBuilder(numberOfArray);
            for (int i = 0; i < numberOfArray; i++)
            {
                sb.Append(array);
            }

            var z = "[" + sb.ToString() + "]";
            return z;
        }

        private static string CreateString(int numberOfChar = 1)
        {
            var sb = new StringBuilder(numberOfChar);
            for (var i = 0; i < numberOfChar; i++)
            {
                sb.Append("RequestUnknown ");
            }

            return sb.ToString();
        }

        private static string CreateInt(int numberOfDigits = 1)
        {
            var sb = new StringBuilder(numberOfDigits);
            for (var i = 0; i < numberOfDigits; i++)
            {
                sb.Append("8");
            }

            return sb.ToString();
        }

        public class RequestObject
        {
            internal object Object { get; set; }

            internal TestType TestType { get; set; }

            internal string Description { get; set; }

            internal bool IgnoreErrorResponse { get; set; }
        }
    }
}
