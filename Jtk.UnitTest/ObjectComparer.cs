using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Xunit;
using Match = Moq.Match;
using Xunit.Abstractions;

namespace Jtk.UnitTest
{
    /// <summary>
    /// Helper class for comparing objects in unit tests.
    /// TODO: Try not to use Newtonsoft as it complicates usage in app that also use Newtonsoft
    /// </summary>
    public static class ObjectComparer
    {
        private static ITestOutputHelper _testOutputHelper;

        private const int _maxRecursionLevel = 15;

        private static readonly ComparisonOptions _defaultSettings = new ComparisonOptions
        {
            ComparisonType = ComparisonType.Inclusive,
            ShouldRecursivelyCompareSubProperties = false,
        };

        private static readonly Regex _collectionIndexPattern = new Regex(@"\[\d+\]");

        /// <summary>
        /// Set the helper to help with xUnit output
        /// </summary>
        /// <param name="testOutputHelper">Test output tester</param>
        public static void SetTestOutputHelper(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        /// <summary>
        /// Asserts that the properties of two objects are equal.  Will pass if both objects are null, and fail if the 
        /// expected object is null while the actual object isn't. 
        /// Note: Properties that implement IComparable are compared  with .CompareTo(), and properties that do not 
        /// are compared with .Equals(). This means that reference types that don't implement Equals() will always
        /// be consider unequal, even if their properties are identical, unless a ComparisonOptions is passed with
        /// ShouldRecursivelyCompareSubProperties set to true.
        /// </summary>
        /// <typeparam name="T">The type of objects being compared</typeparam>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        /// <param name="settings">The settings to use when comparing property values</param>
        public static void AssertPropertiesAreEqual<T>(T expected, T actual, ComparisonOptions settings = null)
            where T : class
        {            
            if (expected == null)
            {
                Assert.Null(actual);
            }
            else
            {
                Assert.NotNull(actual);

                var comparisonResults = CompareProperties(expected, actual, settings);

                if (!comparisonResults.Item1)
                {
                    LogInequalProperties(expected, actual, comparisonResults);
                }

                Assert.True(comparisonResults.Item1, "Properties of items are not equal. See test output for details.");
            }
        }

        /// <summary>
        /// Compares the properties of two objects to determine if they are equal.  Will pass if both objects are null, and fail if the 
        /// expected object is null while the actual object isn't. 
        /// Note: Properties that implement IComparable are compared  with .CompareTo(), and properties that do not 
        /// are compared with .Equals(). This means that reference types that don't implement Equals() will always
        /// be consider unequal, even if their properties are identical, unless a ComparisonOptions is passed with
        /// ShouldRecursivelyCompareSubProperties set to true.
        /// </summary>
        /// <typeparam name="T">The type of objects being compared</typeparam>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        /// <param name="settings">The settings to use when comparing property values</param>
        /// <returns>
        ///		<c>true</c> if the properties are equal; otherwise, <c>false</c>
        /// </returns>
        public static bool ArePropertiesEqual<T>(T expected, T actual, ComparisonOptions settings = null)
            where T : class
        {            
            var result = CompareProperties(expected, actual, settings);

            if (!result.Item1)
            {
                LogInequalProperties(expected, actual, result);
            }

            return result.Item1;
        }

        /// <summary>
        /// Returns a Moq Match&lt;T&gt; that will match an object with the same property values as a given object.
        /// Note: Properties that implement IComparable are compared  with .CompareTo(), and properties that do not 
        /// are compared with .Equals(). This means that reference types that don't implement Equals() will always
        /// be consider unequal, even if their properties are identical.
        /// </summary>
        /// <typeparam name="T">The type of objects being compared</typeparam>
        /// <param name="expected">The expected object</param>
        /// <returns>A Moq Match&lt;T&gt; to be used in a Verify or Setup statement</returns>
        public static T MatchPropertyValues<T>(T expected) where T : class
        {
            return MatchPropertyValues(expected, null);
        }

        /// <summary>
        /// Returns a Moq Match&lt;T&gt; that will match an object with the same property values as a given object.
        /// Note: Properties that implement IComparable are compared  with .CompareTo(), and properties that do not 
        /// are compared with .Equals(). This means that reference types that don't implement Equals() will always
        /// be consider unequal, even if their properties are identical, unless a ComparisonOptions is passed with
        /// ShouldRecursivelyCompareSubProperties set to true.
        /// </summary>
        /// <typeparam name="T">The type of objects being compared</typeparam>
        /// <param name="expected">The expected object</param>
        /// <param name="settings">The settings to use when comparing property values</param>
        /// <returns>A Moq Match&lt;T&gt; to be used in a Verify or Setup statement</returns>
        public static T MatchPropertyValues<T>(T expected, ComparisonOptions settings) where T : class
        {
            return Match.Create<T>(a =>
            {
                var results = CompareProperties(expected, a, settings);
                var isEqual = results.Item1;

                if (!isEqual)
                {
                    LogInequalProperties(expected, a, results);
                }

                return isEqual;
            });
        }

        // not worried about comparing value types with nulls here, it won't cause any problems
        // ReSharper disable CompareNonConstrainedGenericWithNull
        private static Tuple<bool, List<InequalProperty>, List<string>> CompareProperties<T>(T expected, T actual,
            ComparisonOptions compareSettings = null, int previousRecursionLevel = 0, string propertyNamespace = "", bool isEnumItemRecursion = false)
        {
            var currentRecursionLevel = previousRecursionLevel + 1;
            var settings = compareSettings ?? _defaultSettings;

            // the generic parameters will ensure that the expected and actual types are the same, but the reflection that the recursion does 
            // will pass the values as object, so to correctly determine the properties we need to determine the actual type of the params 
            // instead of just doing typeof(T)
            var expectedType = expected != null ? expected.GetType() : typeof(T);

            var actualType = actual != null ? actual.GetType() : typeof(T);

            if (expected == null && actual == null)
            {
                return Tuple.Create(true, new List<InequalProperty>(), new List<string>());
            }

            if (expected == null || actual == null)
            {
                return Tuple.Create(false, new List<InequalProperty>(), new List<string>());
            }

            // sanity check, shouldn't actually be possible
            if (expectedType != actualType)
            {
                return Tuple.Create(false, new List<InequalProperty>(), new List<string>());
            }

            if (ShouldCompareAsValueType(expectedType))
            {
                return Tuple.Create(expected.Equals(actual), new List<InequalProperty>(), new List<string>());
            }

            // if we are recursing through the items of an Enumeration we need to check to see if we should
            // do a direct comparison of each item before looking at their properties
            if (isEnumItemRecursion)
            {
                // remove . from the end of the property namespace to get an accurate 'name' for the item
                var itemNamespace = propertyNamespace.TrimEnd('.');
                var itemType = expected.GetType();
                var itemAsInequal = new InequalProperty(itemNamespace, expected, actual);

                var itemNamespaceWithoutIndex = _collectionIndexPattern.Replace(itemNamespace, "[]");

                if (settings.PropertiesToCompareByReference.NullSafe().Any(r => itemNamespaceWithoutIndex == r))
                {
                    itemAsInequal.InequalityReason = "Comparison of objects with .ReferenceEquals() returned false";

                    return ReferenceEquals(expected, actual)
                        ? Tuple.Create(true, new List<InequalProperty>(), new List<string>())
                        : Tuple.Create(false, new List<InequalProperty> { itemAsInequal }, new List<string>());
                }
                if (IsComparable(itemType))
                {
                    var comparibleItem = (IComparable)expected;

                    itemAsInequal.InequalityReason = "Comparison of ICompariable objects with .CompareTo() did not return zero";

                    return comparibleItem.CompareTo(actual) == 0
                        ? Tuple.Create(true, new List<InequalProperty>(), new List<string>())
                        : Tuple.Create(false, new List<InequalProperty> { itemAsInequal }, new List<string>());
                }
                if (HasOverriddenEquals(itemType) && !IsKeyValuePair(itemType))
                {
                    itemAsInequal.InequalityReason = "Comparison of objects with overridden .Equals() returned false";

                    return expected.Equals(actual)
                        ? Tuple.Create(true, new List<InequalProperty>(), new List<string>())
                        : Tuple.Create(false, new List<InequalProperty> { itemAsInequal }, new List<string>());
                }
            }

            var inequalProperties = new List<InequalProperty>();

            var properties = GetProperties(expectedType).ToList();

            // if the type doesn't have any properties, just compare with .Equals()
            if (!properties.Any())
            {
                return Tuple.Create(expected.Equals(actual), new List<InequalProperty>(), new List<string>());
            }

            var filteredProperties = FilterProperties(properties, propertyNamespace, settings);
            var ignoredProperties = filteredProperties.IgnoredPropertyNames;

            foreach (var property in filteredProperties.PropertiesToCompare)
            {
                var propertyType = property.PropertyType;

                var expectedValue = property.GetValue(expected, new object[0]);
                var actualValue = property.GetValue(actual, new object[0]);

                var propertyAsInequal = new InequalProperty(propertyNamespace + property.Name, expectedValue, actualValue);

                if (expectedValue == null && actualValue == null) // both null
                {
                    // values are equal, this case prevents null exceptions
                }
                else if (ShouldCompareReferences(property, propertyNamespace, settings)) // setting override - compare via .ReferenceEquals
                {
                    if (!ReferenceEquals(expectedValue, actualValue))
                    {
                        propertyAsInequal.InequalityReason = "Comparison of objects with .ReferenceEquals() returned false";
                        inequalProperties.Add(propertyAsInequal);
                    }
                }
                else if ((expectedValue == null) != (actualValue == null)) // null / not null mismatch
                {
                    propertyAsInequal.InequalityReason = "Null / not null mismatch";
                    inequalProperties.Add(propertyAsInequal);
                }
                else if (ShouldCompareAsValueType(propertyType)) // struct - compare directly with .Equals()
                {
                    if (!expectedValue.Equals(actualValue))
                    {
                        propertyAsInequal.InequalityReason = "Comparison of Value Type objects with .Equals() returned false";
                        inequalProperties.Add(propertyAsInequal);
                    }
                }
                else if (IsComparable(propertyType)) // IComparable - compare with .Compare()
                {
                    var comparibleValue = (IComparable)expectedValue;

                    if (comparibleValue.CompareTo(actualValue) != 0)
                    {
                        propertyAsInequal.InequalityReason = "Comparison of ICompariable objects with .CompareTo() did not return zero";
                        inequalProperties.Add(propertyAsInequal);
                    }
                }
                else if (HasOverriddenEquals(propertyType) && !IsKeyValuePair(propertyType)) // type overrides .Equals() - compare with .Equals() (not KVP's Equals() as that won't take properties into account)
                {
                    if (!expectedValue.Equals(actualValue))
                    {
                        propertyAsInequal.InequalityReason = "Comparison of objects with overridden .Equals() returned false";
                        inequalProperties.Add(propertyAsInequal);
                    }
                }
                else // no obvious correct way to compare - either recurse object properties or use .Equals()
                {
                    if (settings.ShouldRecursivelyCompareSubProperties && currentRecursionLevel <= _maxRecursionLevel)
                    {
                        if (IsEnumerable(propertyType))
                        {
                            var expectedCollection = ((IEnumerable)expectedValue).Cast<object>().ToList();
                            var actualCollection = ((IEnumerable)actualValue).Cast<object>().ToList();

                            if (expectedCollection.Count != actualCollection.Count)
                            {
                                propertyAsInequal.InequalityReason = string.Format("IEnumerable Count Mismatch - Expected Count: {0} Actual Count: {1}",
                                    expectedCollection.Count, actualCollection.Count);
                                inequalProperties.Add(propertyAsInequal);
                            }
                            else
                            {
                                for (var i = 0; i < expectedCollection.Count; i++)
                                {
                                    var itemNamespace = propertyNamespace + property.Name + string.Format("[{0}].", i);
                                    var itemResult = CompareProperties(expectedCollection[i], actualCollection[i],
                                        settings, currentRecursionLevel, itemNamespace, true);
                                    if (!itemResult.Item1)
                                    {
                                        if (itemResult.Item2.NullSafeCount() > 0)
                                        {
                                            inequalProperties.AddRange(itemResult.Item2);
                                        }
                                        else
                                        {
                                            // edge case, but the items themselves could be inequal without having any properties
                                            inequalProperties.Add(new InequalProperty(itemNamespace.TrimEnd('.'),
                                                expectedCollection[i], actualCollection[i]));
                                        }
                                        ignoredProperties.AddRange(itemResult.Item3);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var result = CompareProperties(expectedValue, actualValue, settings, currentRecursionLevel, propertyNamespace + property.Name + ".");

                            if (!result.Item1)
                            {
                                inequalProperties.AddRange(result.Item2);
                            }
                        }
                    }
                    else if (!expectedValue.Equals(actualValue)) // can't use == as that will only do a reference comparison as the values are objects.
                    {
                        propertyAsInequal.InequalityReason = "Comparison of objects with .Equals() returned false";
                        inequalProperties.Add(propertyAsInequal);
                    }
                }
            }

            return Tuple.Create(!inequalProperties.Any(), inequalProperties, ignoredProperties);
        }

        // ReSharper restore CompareNonConstrainedGenericWithNull

        private static FilteredProperties FilterProperties(IEnumerable<PropertyInfo> properties, string propertyNamespace, ComparisonOptions settings)
        {
            var namespaceWithoutIndexes = _collectionIndexPattern.Replace(propertyNamespace, "[]");
            if (settings.ComparisonType == ComparisonType.Inclusive)
            {
                var toIgnore = settings.PropertiesToIgnore.NullSafe().ToList();
                var splitProperties = properties.ToLookup(p => toIgnore.Contains(namespaceWithoutIndexes + p.Name));

                return new FilteredProperties
                {
                    PropertiesToCompare = splitProperties[false].NullSafe().ToList(),
                    IgnoredPropertyNames = splitProperties[true].NullSafe().Select(p => propertyNamespace + p.Name).ToList()
                };
            }
            else
            {
                var toInclude = settings.PropertiesToCompare.NullSafe().ToList();
                var splitProperties = properties.ToLookup(p => toInclude.Any(i => i.NullSafe().StartsWith(namespaceWithoutIndexes + p.Name)));

                return new FilteredProperties
                {
                    PropertiesToCompare = splitProperties[true].NullSafe().ToList(),
                    IgnoredPropertyNames = splitProperties[false].NullSafe().Select(p => propertyNamespace + p.Name).ToList()
                };
            }
        }

        //<Some extension method copied from Clerent.Extensions.Core so we don't have the dependencies>
          
        private static IEnumerable<T> NullSafe<T>(this IEnumerable<T> collection)
        {
            return collection ?? Enumerable.Empty<T>();
        }

        private static int NullSafeCount<T>(this IEnumerable<T> collection)
        {
            return collection != null ? collection.Count() : 0;
        }

        private static TResult NullSafe<TInput, TResult>(this TInput value, Func<TInput, TResult> evaluator, TResult failureValue)
            where TInput : class
        {
            return (value != null) ? evaluator(value) : failureValue;
        }

        private static string NullSafe(this string input)
        {
            return input ?? string.Empty;
        }

        //</>

        private static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToArray();
        }

        private static bool ShouldCompareAsValueType(Type type)
        {
            // we don't want to compare KeyValuePair<T1,T2> as a value type even though it is one, as that would
            // ignore it's Key and Value
            return type.IsValueType && !IsKeyValuePair(type);
        }

        private static bool IsKeyValuePair(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
        }

        private static bool ShouldCompareReferences(PropertyInfo property, string propertyNamespace, ComparisonOptions settings)
        {
            return settings.PropertiesToCompareByReference.NullSafe().Any(r => propertyNamespace + property.Name == r);
        }

        private static bool IsComparable(Type type)
        {
            return typeof(IComparable).IsAssignableFrom(type);
        }

        private static bool IsEnumerable(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        private static bool HasOverriddenEquals(Type type)
        {
            var equalsMethod = type.GetMethods()
                .FirstOrDefault(m => m.Name == "Equals" && m.GetBaseDefinition().DeclaringType.Equals(typeof(object)));

            return equalsMethod != null && equalsMethod.DeclaringType != typeof(object);
        }

        private static void LogInequalProperties(object expected, object actual, Tuple<bool, List<InequalProperty>, List<string>> comparisonResults)
        {
            WriteLine("Items unequal due to having unequal properties.");
            WriteLine("Unequal properties:");
            WriteLine();
            foreach (var item in comparisonResults.Item2.NullSafe())
            {
                WriteLine(item.PropertyName);
                WriteLine("---------------");
                WriteLine("Reason: ");
                WriteLine(item.InequalityReason);
                WriteLine("Expected Value: ");
                WriteLine(item.ExpectedPropertyValue.NullSafe(SerializePropertyValue, "null"));
                WriteLine("Actual Value:");
                WriteLine(item.ActualPropertyValue.NullSafe(SerializePropertyValue, "null"));
                WriteLine();
            }

            var ignored = comparisonResults.Item3.NullSafe().ToArray();

            if (ignored.Any())
            {
                WriteLine();
                WriteLine("Ignored properties:");
                foreach (var item in ignored)
                {
                    WriteLine(item);
                }
            }

            WriteLine();
            LogItemValues("Expected", expected);
            LogItemValues("Actual", actual);
        }

        private static string SerializePropertyValue(object value)
        {
            var type = value.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                var kvpKey = type.GetProperty("Key").GetValue(value, null);
                var kvpValue = type.GetProperty("Value").GetValue(value, null);
                return JsonConvert.SerializeObject(new { Key = kvpKey, Value = kvpValue }, Formatting.Indented);
            }
            if (type.IsEnum)
            {
                return value.ToString();
            }

            return JsonConvert.SerializeObject(value);
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
        private static void WriteLine(string message = "")
        {
            try
            {
                if (_testOutputHelper == null)
                {
                    Console.WriteLine(message);
                }
                else
                {
                    _testOutputHelper.WriteLine(message);
                }
            }
            catch (Exception ex) // Test helper error out on build server sometimes
            {
            }
        }

        /// <summary>
        /// Indicates the type of Comparison that should be performed
        /// </summary>
        public enum ComparisonType
        {
            /// <summary>
            /// Inclusive - specifies that all properties should be compared, except for those excluded 
            /// in the list of PropertiesToIgnore
            /// </summary>
            Inclusive,

            /// <summary>
            /// Exclusive - specifies that only properties that are specified in the list of PropertiesToCompare
            /// should be compared
            /// </summary>
            Exclusive
        }

        /// <summary>
        /// Settings that specify how an object's properties should be compared.
        /// </summary>
        public class ComparisonOptions
        {
            /// <summary>
            /// Gets or sets a value that indicates whether the subproperties of a property should be compared,
            /// if there is no other way to compare the properties (e.g., if the property type does not implement 
            /// IComparible or override Equals). If a property implements IEnumerable, the properties of the items it contains will be
            /// compared.
            /// </summary>
            public bool ShouldRecursivelyCompareSubProperties { get; set; }

            /// <summary>
            /// Gets or sets the type of comparison that should be done. Defaults to Inclusive.
            /// </summary>
            public ComparisonType ComparisonType { get; set; }

            /// <summary>
            /// Gets or sets a collection of property names that should be ignored. Not used for Exclusive comparisons.
            /// Properties should be specified in the following manner: "PropertyName.SubPropertyName.SubSubPropertyName". 
            /// To specify a property of a collection item, use []: "PropertyName.IEnumerablePropertyName[].ItemPropertyName". 
            /// </summary>
            public IEnumerable<string> PropertiesToIgnore { get; set; }

            /// <summary>
            /// Gets or sets a collection of property names that should be compared when performing an Exclusive comparison.
            /// Properties should be specified in the following manner: "PropertyName.SubPropertyName.SubSubPropertyName". 
            /// To specify a property of a collection item, use []: "PropertyName.IEnumerablePropertyName[].ItemPropertyName". 
            /// </summary>
            public IEnumerable<string> PropertiesToCompare { get; set; }

            /// <summary>
            /// Gets or sets a collection of property names that should be compared using .ReferenceEquals(), even if Should
            /// ShouldRecursivelyCompareSubProperties has been set to true.
            /// Properties should be specified in the following manner: "PropertyName.SubPropertyName.SubSubPropertyName". 
            /// To specify a property of a collection item, use []: "PropertyName.IEnumerablePropertyName[].ItemPropertyName". 
            /// </summary>
            public IEnumerable<string> PropertiesToCompareByReference { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="ComparisonOptions"/> class.
            /// </summary>
            public ComparisonOptions()
            {
                ComparisonType = ComparisonType.Inclusive;
            }
        }

        private class InequalProperty
        {
            public string PropertyName { get; private set; }

            public object ExpectedPropertyValue { get; private set; }

            public object ActualPropertyValue { get; private set; }

            public string InequalityReason { get; set; }

            public InequalProperty(string name, object expectedValue, object actualValue)
            {
                PropertyName = name;
                ExpectedPropertyValue = expectedValue;
                ActualPropertyValue = actualValue;
            }
        }

        private class FilteredProperties
        {
            public List<PropertyInfo> PropertiesToCompare { get; set; }

            public List<string> IgnoredPropertyNames { get; set; }
        }       
    }
}
