using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework
{
    public static class Extension
    {
        #region Newtonsoft.Json

        private static void CheckedValue(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(propertyName);
            }

            if (propertyName.Length == 0)
            {
                throw new ArgumentException(nameof(propertyName) + ".Length == 0");
            }
        }
        public static string GetString(this JObject thisObject, string propertyName, string defaultValue)
        {
            CheckedValue(propertyName);

            JToken token = thisObject[propertyName];
            if (token != null)
            {
                return token.ToObject<string>();
            }
            else
            {
                return defaultValue;
            }
        }

        public static bool GetBoolean(this JObject thisObject, string propertyName, bool defaultValue)
        {
            CheckedValue(propertyName);

            JToken token = thisObject[propertyName];
            if (token != null)
            {
                return token.ToObject<bool>();
            }
            else
            {
                return false;
            }
        }

        public static int GetInt(this JObject thisObject, string propertyName, int defaultValue)
        {
            CheckedValue(propertyName);

            JToken token = thisObject[propertyName];
            if (token != null)
            {
                return token.ToObject<int>();
            }
            else
            {
                return defaultValue;
            }
        }

        public static float GetFloat(this JObject thisObject, string propertyName, float defaultValue)
        {
            CheckedValue(propertyName);

            JToken token = thisObject[propertyName];
            if (token != null)
            {
                return token.ToObject<float>();
            }
            else
            {
                return defaultValue;
            }
        }

        public static double GetDouble(this JObject thisObject, string propertyName, double defaultValue)
        {
            CheckedValue(propertyName);

            JToken token = thisObject[propertyName];
            if (token != null)
            {
                return token.ToObject<double>();
            }
            else
            {
                return defaultValue;
            }
        }

        public static short GetShort(this JObject thisObject, string propertyName, short defaultValue)
        {
            CheckedValue(propertyName);

            JToken token = thisObject[propertyName];
            if (token != null)
            {
                return token.ToObject<short>();
            }
            else
            {
                return defaultValue;
            }
        }

        public static JObject GetObject(this JObject thisObject, string propertyName)
        {
            CheckedValue(propertyName);

            JObject obj = thisObject[propertyName] as JObject;
            return obj;
        }

        public static JArray GetArray(this JObject thisObject, string propertyName)
        {
            CheckedValue(propertyName);

            JArray array = thisObject[propertyName] as JArray;
            return array;
        }

        public static JObject AddString(this JObject thisObject, string propertyName, string value)
        {
            CheckedValue(propertyName);

            thisObject.Add(propertyName, value);
            return thisObject;
        }

        public static JObject AddBoolean(this JObject thisObject, string propertyName, bool value)
        {
            CheckedValue(propertyName);

            thisObject.Add(propertyName, value);
            return thisObject;
        }

        public static JObject AddInt(this JObject thisObject, string propertyName, int value)
        {
            CheckedValue(propertyName);

            thisObject.Add(propertyName, value);
            return thisObject;
        }

        public static JObject AddFloat(this JObject thisObject, string propertyName, float value)
        {
            CheckedValue(propertyName);

            thisObject.Add(propertyName, value);
            return thisObject;
        }

        public static JObject AddDouble(this JObject thisObject, string propertyName, double value)
        {
            CheckedValue(propertyName);

            thisObject.Add(propertyName, value);
            return thisObject;
        }

        public static JObject AddObject(this JObject thisObject, string propertyName, JObject addObject)
        {
            CheckedValue(propertyName);

            thisObject.Add(propertyName, addObject);
            return thisObject;
        }

        public static JObject AddArray(this JObject thisObject, string propertyName, JArray addArray)
        {
            CheckedValue(propertyName);

            thisObject.Add(propertyName, addArray);
            return thisObject;
        }

        public static bool Remove(this JObject thisObject, string propertyName)
        {
            CheckedValue(propertyName);

            return thisObject.Remove(propertyName);
        }

        public static JObject GetObject(this JArray thisArray, int index)
        {
            int count = thisArray.Count;
            if (count <= index)
            {
                throw new IndexOutOfRangeException("index");
            }

            return thisArray[index] as JObject;
        }

        public static void RemoveAt(this JArray thisArray, int index)
        {
            int count = thisArray.Count;
            if (count <= index)
            {
                throw new IndexOutOfRangeException("index");
            }

            thisArray.RemoveAt(index);
        }

        #endregion
    }
}
