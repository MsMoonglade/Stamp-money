using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal static class HashSetExtensions
    {
        #region --- Private Methods ---

        internal static void Add<TValue>(this HashSet<TValue> self, TValue addition)
        {
            var ignoreThis = self.Add(addition);
        }
        
        internal static void ForEach<TValue>(this HashSet<TValue> self, Action<TValue> action)
        {
            foreach (var item in self)
            {
                action(item);
            }
        }

        #endregion
    }

    internal static class SwExtensionMethods
    {
        #region --- Public Methods ---

        public static bool SwIsEmpty(this ICollection self)
        {
            return self.Count == 0;
        }
        
        public static void DontDestroyOnLoad(this UnityEngine.Object gameObject)
        {
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
        }

        public static void RenderLast(this Canvas canvas)
        {
            canvas.transform.SetAsLastSibling();
            canvas.sortingOrder = short.MaxValue;
        }

        #endregion


        #region --- Private Methods ---

        /// <summary>
        /// This utility method helps to use split a string with string as token (instead of char / chars array) in Unity 2020.
        /// </summary>
        /// <param name="self">The String instance to split</param>
        /// <param name="token">The part that will be extracted while all other parts will be in the result components array</param>
        /// <returns></returns>
        internal static string[] SwSplit(this string self, string token)
        {
            if (string.IsNullOrEmpty(token)) return new[] {self};

#if UNITY_2021_3_OR_NEWER
                return self.Split(token);
#else
            string copy = self.Clone() as string ?? "";
            List<string> components = new List<string>();

            int tokenIndex;

            do
            {
                tokenIndex = copy.IndexOf(token, StringComparison.Ordinal);

                if (tokenIndex < 0) continue;

                var component = copy.Substring(0, tokenIndex);
                components.Add(component);
                copy = copy.Remove(0, tokenIndex + token.Length);
            }
            while (tokenIndex >= 0);

            components.Add(copy);
            
            return components.ToArray();
#endif
        }
        
        internal static Color ExtractColorFromHex(this string colorHex, Color defaultColor)
        {
            if (colorHex == null || colorHex.Equals(string.Empty)) return defaultColor;

            return ColorUtility.TryParseHtmlString(colorHex, out var color) ? color : defaultColor;
        }

        internal static TSource FirstOr<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue)
        {
            var result = defaultValue;

            try
            {
                result = source.First(predicate);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                // Ignore the exception and return the default value...
            }

            return result;
        }

        /// Returns the `defaultValue` in case: the key doesn't exists / it holds to a null value.
        internal static bool SwAddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> self, TKey key, TValue value)
        {
            if (key == null) return false;

            if (self.ContainsKey(key))
            {
                self.Remove(key);
            }

            self.Add(key, value);

            return true;
        }

        /// <summary>
        ///     Merge one dictionary into another.
        ///     The last source keys will override the first source keys, in case of conflicts.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="sources"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        internal static Dictionary<TKey, TValue> SwClone<TKey, TValue>(this Dictionary<TKey, TValue> self)
        {
            var copied = new Dictionary<TKey, TValue>();
            copied.SwMerge(self);

            return copied;
        }

        /// <summary>
        ///     Merge one dictionary into another.
        ///     The last source keys will override the first source keys, in case of conflicts.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="sources"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        internal static Dictionary<TKey, TValue> SwCloneMerged<TKey, TValue>(this Dictionary<TKey, TValue> self, params Dictionary<TKey, TValue>[] sources)
        {
            return self.SwClone().SwMerge(sources);
        }
        
        internal static bool SwIsValidEmailAddress(this string self)
        {
            var emailAddressRegex = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");

            return emailAddressRegex.IsMatch(self);
        }

        /// <summary>
        ///     Merge dictionaries extension
        ///     The last source keys overrides the first source keys
        /// </summary>
        /// <param name="self"></param>
        /// <param name="sources"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        internal static Dictionary<TKey, TValue> SwMerge<TKey, TValue>(this Dictionary<TKey, TValue> self, params Dictionary<TKey, TValue>[] sources)
        {
            foreach (var source in sources)
            {
                if (source != null)
                {
                    foreach (var keyValuePair in source)
                    {
                        self[keyValuePair.Key] = keyValuePair.Value;
                    }
                }
            }

            return self;
        }

        internal static string SwRemoveSpaces(this string self)
        {
            return self?.Replace(" ", "");
        }

        /// Returns the `defaultValue` in case: the key doesn't exists / it holds to a null value.
        internal static TValue SwSafelyGet<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, [CanBeNull] TValue defaultValue)
        {
            if (self == null || key == null) return defaultValue;
            if (!self.ContainsKey(key)) return defaultValue;
            self.TryGetValue(key, out var value);

            return value == null ? defaultValue : value;
        }

        internal static string SwToString<TSource>(this IEnumerable<TSource> source, string delimiter = ", ")
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("[");
            
            foreach (var element in source)
            {
                stringBuilder.Append(element?.ToString() ?? "null");
                stringBuilder.Append(delimiter);
            }
            
            stringBuilder = stringBuilder.Append("]").Replace(delimiter + "]", "]");
            
            return stringBuilder.ToString();
        }
        
        internal static HashSet<TSource> SwToHashSet<TSource>(this IEnumerable<TSource> source)
        {
            return new HashSet<TSource>(source);
        }

        internal static bool SwIsNullOrEmpty(this string self)
        {
            return string.IsNullOrEmpty(self);
        }

        internal static TSource SwSafelyGet<TSource>(this IEnumerable<TSource> source, int index, TSource defaultValue)
        {
            if (source == null) return defaultValue;
            var arrOrNull = source as TSource[];
            var arr = arrOrNull ?? source.ToArray();

            return arr.Length > index ? arr[index] : defaultValue;
        }

        internal static long SwTimestampMilliseconds(this DateTime self)
        {
            return self.Ticks / TimeSpan.TicksPerMillisecond;
        }

        internal static long SwTimestampSeconds(this DateTime self)
        {
            return self.Ticks / TimeSpan.TicksPerSecond;
        }

        internal static Dictionary<string, object> SwToJsonDictionary(this string self)
        {
            return SwJsonParser.DeserializeToDictionary(self);
        }

        internal static string SwToJsonString(this Dictionary<string, object> self)
        {
            return SwJsonParser.Serialize(self);
        }

        /// <summary>
        ///     <para>Use this method instead of `dateTime.ToString()` to avoid exceptions that occur in Unity 2020 and above.</para>
        /// </summary>
        internal static string SwToString(this DateTime self)
        {
            // Formatting a date with CultureInfo.InvariantCulture as second parameter prevents exceptions in case the users' default culture is Arabic or Thai
            return self.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     <para>Use this method instead of `dateTime.ToString(...)` to avoid exceptions that occur in Unity 2020 and above.</para>
        /// </summary>
        internal static string SwToString(this DateTime self, string format)
        {
            // Formatting a date with CultureInfo.InvariantCulture as second parameter prevents exceptions in case the users' default culture is Arabic or Thai
            return self.ToString(format, CultureInfo.InvariantCulture);
        }

        #endregion
    }
}