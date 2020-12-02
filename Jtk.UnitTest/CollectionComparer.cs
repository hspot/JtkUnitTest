using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace Jtk.UnitTest
{
    /// <summary>
    /// Comparison Helper class for determining if collections of complex objects are equal
    /// TODO: Try not to use Newtonsoft as it complicates usage in app that also use Newtonsoft
    /// </summary>
    public static class CollectionComparer
    {
        private static ITestOutputHelper _testOutputHelper;


        /// <summary>
        /// Set the helper to help with xUnit output for both CollectionComparer and ObjectComparer
        /// </summary>
        /// <param name="testOutputHelper">Test output tester</param>
        public static void SetTestOutputHelper(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            ObjectComparer.SetTestOutputHelper(testOutputHelper);
        }

        /// <summary>
        /// Determines whether two collections are equal by checking if they contain the same items, in the same order.
        /// Note that this overload determines if items are equal by calling Equals(), so two items of a reference type with the
        /// same property values will not be considered equal (unless Equals() and .GetHashCode() have been overridden for the type)
        /// </summary>
        /// <typeparam name="T">The type of items contained in the collections</typeparam>
        /// <param name="collection1">The first collection</param>
        /// <param name="collection2">The second collection</param>
        /// <returns>Whether the collections are equal</returns>
        public static bool AreCollectionsEqual<T>(IEnumerable<T> collection1, IEnumerable<T> collection2)
        {
            return AreCollectionsEqual(collection1, collection2, (x, y) => x.Equals(y));
        }

        /// <summary>
        /// Determines whether two collections are equal by checking if they contain the same items, in the same order, by
        /// using a defined comparison 
        /// </summary>
        /// <typeparam name="T">The type of items contained in the collections</typeparam>
        /// <param name="collection1">The first collection</param>
        /// <param name="collection2">The second collection</param>
        /// <param name="comparison">The comparison to perform when comparing collection items</param>
        /// <returns>Whether the collections are equal</returns>
        public static bool AreCollectionsEqual<T>(IEnumerable<T> collection1, IEnumerable<T> collection2,
            Func<T, T, bool> comparison)
        {
            if (collection1 == null && collection2 == null)
            {
                return true;
            }

            if (collection1 == null || collection2 == null)
            {
                WriteLine("CollectionComparer - Collections unequal due to one item being null.");
                return false;
            }

            var one = collection1.ToList();
            var two = collection2.ToList();

            var areEqual = true;

            if (one.Count != two.Count)
            {
                WriteLine(
                    "CollectionComparer - Collections unequal due different sizes. Collection 1 size: {0} Collection 2 size: {1}", one.Count, two.Count);
                areEqual = false;
            }

            if (areEqual)
            {
                for (var i = 0; i < one.Count; i++)
                {
                    var item1 = one[i];
                    var item2 = two[i];

                    if (!comparison(item1, item2))
                    {
                        WriteLine("CollectionComparer - Collections unequal due items at index {0} being unequal.", i);
                        areEqual = false;
                        break;
                    }
                }
            }

            if (!areEqual)
            {
                LogItemValues("Collection one", one);
                LogItemValues("Collection two", two);
            }

            return areEqual;
        }

        /// <summary>
        /// Determines whether two collections are equal by checking if they contain the same items, in the same order. 
        /// Items equality is determined by comparing each item's properties.
        /// </summary>
        /// <typeparam name="T">The type of items contained in the collections</typeparam>
        /// <param name="collection1">The first collection</param>
        /// <param name="collection2">The second collection</param>
        /// <returns>Whether the collections are equal</returns>
        public static bool AreCollectionsEqualByItemProperties<T>(IEnumerable<T> collection1, IEnumerable<T> collection2)
            where T : class
        {
            return AreCollectionsEqualByItemProperties(collection1, collection2, null);
        }

        /// <summary>
        /// Determines whether two collections are equal by checking if they contain the same items, in the same order. 
        /// Items equality is determined by comparing each item's properties.
        /// </summary>
        /// <typeparam name="T">The type of items contained in the collections</typeparam>
        /// <param name="collection1">The first collection</param>
        /// <param name="collection2">The second collection</param>
        /// <param name="compareSettings">The comparison options to use when comparing item properties</param>
        /// <returns>Whether the collections are equal</returns>
        public static bool AreCollectionsEqualByItemProperties<T>(IEnumerable<T> collection1, IEnumerable<T> collection2,
            ObjectComparer.ComparisonOptions compareSettings)
            where T : class
        {
            return AreCollectionsEqual(collection1, collection2, (x, y) => ObjectComparer.ArePropertiesEqual(x, y, compareSettings));
        }

        /// <summary>
        /// Determines whether two collections are equivalent by checking if they contain the same items, in the any order.
        /// Note that this overload determines if items are equal by calling Equals(), so two items of a reference type with the
        /// same property values will not be considered equal (unless Equals() and .GetHashCode() have been overridden for the type).
        /// </summary>
        /// <typeparam name="T">The type of items contained in the collections</typeparam>
        /// <param name="collection1">The first collection</param>
        /// <param name="collection2">The second collection</param>
        /// <returns>Whether the collections are equal</returns>
        public static bool AreCollectionsEquivalent<T>(IEnumerable<T> collection1, IEnumerable<T> collection2)
        {
            return AreCollectionsEquivalent(collection1, collection2, (x, y) => x.Equals(y));
        }

        /// <summary>
        /// Determines whether two collections are equal by checking if they contain the same items, in the any order, by
        /// using a defined comparison. Note this does not support testing scenarios where both collections contain 
        /// 'duplicate' items (as identified by the comparison).
        /// </summary>
        /// <typeparam name="T">The type of items contained in the collections</typeparam>
        /// <param name="collection1">The first collection</param>
        /// <param name="collection2">The second collection</param>
        /// <param name="comparison">The comparison to perform when comparing collection items</param>
        /// <returns>Whether the collections are equal</returns>
        public static bool AreCollectionsEquivalent<T>(IEnumerable<T> collection1, IEnumerable<T> collection2,
            Func<T, T, bool> comparison)
        {
            if (collection1 == null && collection2 == null)
            {
                return true;
            }

            if (collection1 == null || collection2 == null)
            {
                WriteLine("CollectionComparer - Collections unequal due to one item being null.");
                return false;
            }

            var one = collection1.ToList();
            var two = collection2.ToList();

            var areEqual = true;

            if (one.Count != two.Count)
            {
                WriteLine(
                    "CollectionComparer - Collections unequal due different sizes. Collection 1 size: {0} Collection 2 size: {1}",
                    one.Count, two.Count);
                areEqual = false;
            }

            if (areEqual)
            {
                foreach (var item in one)
                {
                    var matchingItems = two.FindAll(e => comparison(item, e));

                    if (matchingItems.Count == 0)
                    {
                        WriteLine("CollectionComparer - Collections unequal due to no matching item found in second collection.");

                        areEqual = false;
                        break;
                    }

                    if (matchingItems.Count > 1)
                    {
                        // need to check that we don't expect multiple matching items before erroring out
                        var expectedItems = one.FindAll(e => comparison(item, e));
                        if (expectedItems.Count != matchingItems.Count)
                        {
                            WriteLine("CollectionComparer - Collections unequal due to more than the expected number of matching " +
                                "items found in second collection. Items matching condition in first collection: {0}, Items matching " +
                                "condition in second collection: {1}", expectedItems.Count, matchingItems.Count);
                            areEqual = false;
                            break;
                        }
                    }
                }
            }

            if (!areEqual)
            {
                LogItemValues("Collection one", one);
                LogItemValues("Collection two", two);
            }

            return areEqual;
        }

        /// <summary>
        /// Determines whether two collections are equal by checking if they contain the same items, in the any order. 
        /// Items equality is determined by comparing each item's properties.
        /// </summary>
        /// <typeparam name="T">The type of items contained in the collections</typeparam>
        /// <param name="collection1">The first collection</param>
        /// <param name="collection2">The second collection</param>
        /// <returns>Whether the collections are equal</returns>
        public static bool AreCollectionsEquivalentByItemProperties<T>(IEnumerable<T> collection1, IEnumerable<T> collection2)
            where T : class
        {
            return AreCollectionsEquivalentByItemProperties(collection1, collection2, null);
        }

        /// <summary>
        /// Determines whether two collections are equal by checking if they contain the same items, in the any order. 
        /// Items equality is determined by comparing each item's properties.
        /// </summary>
        /// <typeparam name="T">The type of items contained in the collections</typeparam>
        /// <param name="collection1">The first collection</param>
        /// <param name="collection2">The second collection</param>
        /// <param name="compareSettings">The comparison options to use when comparing item properties</param>
        /// <returns>Whether the collections are equal</returns>
        public static bool AreCollectionsEquivalentByItemProperties<T>(IEnumerable<T> collection1, IEnumerable<T> collection2,
            ObjectComparer.ComparisonOptions compareSettings)
            where T : class
        {
            return AreCollectionsEqual(collection1, collection2, (x, y) => ObjectComparer.ArePropertiesEqual(x, y, compareSettings));
        }

        /// <summary>
        /// Returns a Moq Match&lt;IEnumerable&lt;<typeparam name="T"></typeparam>&gt;&gt; that can be used with .Setup, .Verify, etc 
        /// to match an 'Equal' collection.
        /// Note that this overload determines if items are equal by calling Equals(), so two items of a reference type with the
        /// same property values will not be considered equal (unless Equals() and .GetHashCode() have been overridden for the type)
        /// </summary>
        /// <typeparam name="T">The type of items contained in the collections</typeparam>
        /// <param name="expected">The collection to match</param>
        /// <returns>An IEnumerable&lt;T&gt; that can be used with Moq methods.</returns>
        public static IEnumerable<T> MatchEqualIEnumerable<T>(IEnumerable<T> expected)
        {
            return Match.Create<IEnumerable<T>>(l => AreCollectionsEqual(expected, l));
        }

        /// <summary>
        /// Returns a Moq Match&lt;IEnumerable&lt;<typeparam name="T"></typeparam>&gt;&gt; that can be used with .Setup, .Verify, etc 
        /// to match an 'Equal' collection.
        /// </summary>
        /// <typeparam name="T">The type of items contained in the collections</typeparam>
        /// <param name="expected">The collection to match</param>
        /// <param name="comparison">The comparison to use to determine if two collection items are equal.</param>
        /// <returns>An IEnumerable&lt;T&gt; that can be used with Moq methods.</returns>
        public static IEnumerable<T> MatchEqualIEnumerable<T>(IEnumerable<T> expected, Func<T, T, bool> comparison)
        {
            return Match.Create<IEnumerable<T>>(l => AreCollectionsEqual(expected, l, comparison));
        }

        /// <summary>
        /// Returns a Moq Match&lt;IEnumerable&lt;<typeparam name="T"></typeparam>&gt;&gt; that can be used with .Setup, .Verify, etc 
        /// to match an 'Equal' collection.
        /// Note that this overload determines if items are equal by calling using the ObjectComparer to match item property values.
        /// </summary>
        /// <typeparam name="T">The type of items contained in the collections</typeparam>
        /// <param name="expected">The collection to match</param>
        /// <param name="compareSettings">The comparison options to pass to the ObjectComparer. Optional.</param>
        /// <returns>An IEnumerable&lt;T&gt; that can be used with Moq methods.</returns>
        public static IEnumerable<T> MatchEqualIEnumerableByItemProperties<T>(IEnumerable<T> expected,
            ObjectComparer.ComparisonOptions compareSettings = null)
            where T : class
        {
            return Match.Create<IEnumerable<T>>(l => AreCollectionsEqual(expected, l,
                (x, y) => ObjectComparer.ArePropertiesEqual(x, y, compareSettings)));
        }

        /// <summary>
        /// Returns a Moq Match&lt;IEnumerable&lt;<typeparam name="T"></typeparam>&gt;&gt; that can be used with .Setup, .Verify, etc 
        /// to match an 'Equivalent' collection.
        /// Note that this overload determines if items are equal by calling Equals(), so two items of a reference type with the
        /// same property values will not be considered equal (unless Equals() and .GetHashCode() have been overridden for the type)
        /// </summary>
        /// <typeparam name="T">The type of items contained in the collections</typeparam>
        /// <param name="expected">The collection to match</param>
        /// <returns>An IEnumerable&lt;T&gt; that can be used with Moq methods.</returns>
        public static IEnumerable<T> MatchEquivalentIEnumerable<T>(IEnumerable<T> expected)
        {
            return Match.Create<IEnumerable<T>>(l => AreCollectionsEquivalent(expected, l));
        }

        /// <summary>
        /// Returns a Moq Match&lt;IEnumerable&lt;<typeparam name="T"></typeparam>&gt;&gt; that can be used with .Setup, .Verify, etc 
        /// to match an 'Equivalent' collection.
        /// Note that this overload determines if items are equal by calling Equals(), so two items of a reference type with the
        /// same property values will not be considered equal (unless Equals() and .GetHashCode() have been overridden for the type)
        /// </summary>
        /// <typeparam name="T">The type of items contained in the collections</typeparam>
        /// <param name="expected">The collection to match</param>
        /// <param name="comparison">The comparison to use to determine if two collection items are equal.</param>
        /// <returns>An IEnumerable&lt;T&gt; that can be used with Moq methods.</returns>
        public static IEnumerable<T> MatchEquivalentIEnumerable<T>(IEnumerable<T> expected, Func<T, T, bool> comparison)
        {
            return Match.Create<IEnumerable<T>>(l => AreCollectionsEquivalent(expected, l, comparison));
        }

        /// <summary>
        /// Returns a Moq Match&lt;IEnumerable&lt;<typeparam name="T"></typeparam>&gt;&gt; that can be used with .Setup, .Verify, etc 
        /// to match an 'Equivalent' collection.
        /// Note that this overload determines if items are equal by calling using the ObjectComparer to match item property values.
        /// </summary>
        /// <typeparam name="T">The type of items contained in the collections</typeparam>
        /// <param name="expected">The collection to match</param>
        /// <param name="compareSettings">The comparison options to pass to the ObjectComparer. Optional.</param>
        /// <returns>An IEnumerable&lt;T&gt; that can be used with Moq methods.</returns>
        public static IEnumerable<T> MatchEquivalentIEnumerableByItemProperties<T>(IEnumerable<T> expected,
            ObjectComparer.ComparisonOptions compareSettings = null)
            where T : class
        {
            return Match.Create<IEnumerable<T>>(l => AreCollectionsEquivalent(expected, l,
                (x, y) => ObjectComparer.ArePropertiesEqual(x, y, compareSettings)));
        }

        /// <summary>
        /// Returns a Moq Match&lt;List&lt;<typeparam name="T"></typeparam>&gt;&gt; that can be used with .Setup, .Verify, etc 
        /// to match an 'Equal' collection.
        /// Note that this overload determines if items are equal by calling Equals(), so two items of a reference type with the
        /// same property values will not be considered equal (unless Equals() and .GetHashCode() have been overridden for the type)
        /// </summary>
        /// <typeparam name="T">The type of items contained in the collections</typeparam>
        /// <param name="expected">The collection to match</param>
        /// <returns>An List&lt;T&gt; that can be used with Moq methods.</returns>
        public static List<T> MatchEqualList<T>(IEnumerable<T> expected)
        {
            return Match.Create<List<T>>(l => AreCollectionsEqual(expected, l));
        }

        /// <summary>
        /// Returns a Moq Match&lt;List&lt;<typeparam name="T"></typeparam>&gt;&gt; that can be used with .Setup, .Verify, etc 
        /// to match an 'Equal' collection.
        /// </summary>
        /// <typeparam name="T">The type of items contained in the collections</typeparam>
        /// <param name="expected">The collection to match</param>
        /// <param name="comparison">The comparison to use to determine if two collection items are equal.</param>
        /// <returns>An List&lt;T&gt; that can be used with Moq methods.</returns>
        public static List<T> MatchEqualList<T>(IEnumerable<T> expected, Func<T, T, bool> comparison)
        {
            return Match.Create<List<T>>(l => AreCollectionsEqual(expected, l, comparison));
        }

        /// <summary>
        /// Returns a Moq Match&lt;List&lt;<typeparam name="T"></typeparam>&gt;&gt; that can be used with .Setup, .Verify, etc 
        /// to match an 'Equivalent' collection.
        /// Note that this overload determines if items are equal by calling Equals(), so two items of a reference type with the
        /// same property values will not be considered equal (unless Equals() and .GetHashCode() have been overridden for the type)
        /// </summary>
        /// <typeparam name="T">The type of items contained in the collections</typeparam>
        /// <param name="expected">The collection to match</param>
        /// <returns>An List&lt;T&gt; that can be used with Moq methods.</returns>
        public static List<T> MatchEquivalentList<T>(IEnumerable<T> expected)
        {
            return Match.Create<List<T>>(l => AreCollectionsEquivalent(expected, l));
        }

        /// <summary>
        /// Returns a Moq Match&lt;List&lt;<typeparam name="T"></typeparam>&gt;&gt; that can be used with .Setup, .Verify, etc 
        /// to match an 'Equivalent' collection.
        /// Note that this overload determines if items are equal by calling Equals(), so two items of a reference type with the
        /// same property values will not be considered equal (unless Equals() and .GetHashCode() have been overridden for the type)
        /// </summary>
        /// <typeparam name="T">The type of items contained in the collections</typeparam>
        /// <param name="expected">The collection to match</param>
        /// /// <param name="comparison">The comparison to use to determine if two collection items are equal.</param>
        /// <returns>An List&lt;T&gt; that can be used with Moq methods.</returns>
        public static List<T> MatchEquivalentList<T>(IEnumerable<T> expected, Func<T, T, bool> comparison)
        {
            return Match.Create<List<T>>(l => AreCollectionsEquivalent(expected, l, comparison));
        }

        /// <summary>
        /// Returns a Moq Match&lt;Dictionary&lt;<typeparam name="TKey"></typeparam>, <typeparam name="TValue"></typeparam>&gt;&gt;
        /// that can be used with .Setup, .Verify, etc to match an 'Equal' collection.
        /// Note that this overload determines if items are equal by calling Equals(), so two items of a reference type with the
        /// same property values will not be considered equal (unless Equals() and .GetHashCode() have been overridden for the type)
        /// </summary>
        /// <typeparam name="TKey">The type of items that are used as keys in the dictionary</typeparam>
        /// <typeparam name="TValue">The type of items that are used as values in the dictionary</typeparam>
        /// <param name="expected">The collection to match</param>
        /// <returns>An List&lt;T&gt; that can be used with Moq methods.</returns>
        public static Dictionary<TKey, TValue> MatchEqualDictionary<TKey, TValue>(Dictionary<TKey, TValue> expected)
        {
            return Match.Create<Dictionary<TKey, TValue>>(l => AreCollectionsEqual(expected, l));
        }

        private static void LogItemValues(string label, object item)
        {
            WriteLine(label + ": ");

            var xmlFailed = false;
            try
            {
                WriteLine(JsonConvert.SerializeObject(item));
            }
            catch
            {
                xmlFailed = true;
            }

            if (xmlFailed)
            {
                try
                {
                    WriteLine(JsonConvert.SerializeObject(item, Formatting.Indented));
                }
                catch
                {
                    WriteLine("Unable to determine object values.");
                }
            }
        }

        /// <summary>
        /// Writeline to the right provider
        /// </summary>        
        private static void WriteLine(string message = "", params object[] arg)
        {
            try
            { 
                if (_testOutputHelper == null)
                {
                    Console.WriteLine(message, arg);
                }
                else
                {
                    _testOutputHelper.WriteLine(message, arg);
                }
            }
            catch(Exception ex) // Test helper error out on build server sometimes
            {
            }
        }
    }
}
