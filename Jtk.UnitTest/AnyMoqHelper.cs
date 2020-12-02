using Moq;
using System;
using System.Collections.Generic;

namespace Jtk.UnitTest
{
    /// <summary>
    /// Wrapper to make it simpler to use Moq It.IsAny methods
    /// Ex: Instead of "It.IsAny<string>()", do "Any.String"
    /// Ex: Instad of "It.IsAny<BussinessObject>()", do "Any.InstanceOf<BussinessObject>"
    /// </summary>
    public static class Any
    {
        /// <summary>
        /// <see cref="It.IsAny{bool}()"/> wrapper.
        /// </summary>
        public static bool Bool
        {
            get { return It.IsAny<bool>(); }
        }

        /// <summary>
        /// <see cref="It.IsAny{decimal}()"/> wrapper.
        /// </summary>
        public static decimal Decimal
        {
            get { return It.IsAny<decimal>(); }
        }

        /// <summary>
        /// <see cref="It.IsAny{double}()"/> wrapper.
        /// </summary>
        public static double Double
        {
            get { return It.IsAny<double>(); }
        }

        /// <summary>
        /// <see cref="It.IsAny{long}()"/> wrapper.
        /// </summary>
        public static long Long
        {
            get { return It.IsAny<long>(); }
        }

        /// <summary>
        /// <see cref="It.IsAny{DateTime}()"/> wrapper.
        /// </summary>
        public static DateTime DateTime
        {
            get { return It.IsAny<DateTime>(); }
        }

        /// <summary>
        /// <see cref="It.IsAny{int}()"/> wrapper.
        /// </summary>
        public static int Int
        {
            get { return It.IsAny<int>(); }
        }

        /// <summary>
        /// <see cref="It.IsAny{string}()"/> wrapper.
        /// </summary>
        public static string String
        {
            get { return It.IsAny<string>(); }
        }

        /// <summary>
        /// Wrapper for <see cref="It.IsAny{T}()"/>.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>Instance of the specified type.</returns>
        public static T InstanceOf<T>()
        {
            return It.IsAny<T>();
        }

        /// <summary>
        /// <see cref="It.IsAny{Array{T}}"/> wrapper.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>Array of the specified type.</returns>
        public static T[] Array<T>()
        {
            return It.IsAny<T[]>();
        }

        /// <summary>
        /// <see cref="It.IsAny{IEnumerable{T}}"/> wrapper.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>Array of the specified type.</returns>
        public static IEnumerable<T> IEnumerable<T>()
        {
            return It.IsAny<IEnumerable<T>>();
        }

        /// <summary>
        /// <see cref="It.IsAny{List{T}}"/> wrapper.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>Array of the specified type.</returns>
        public static List<T> List<T>()
        {
            return It.IsAny<List<T>>();
        }

        /// <summary>
        /// Wrapper for <see cref="It.IsAny{Dictionary{T1, T2}}()"/>.
        /// </summary>
        public static Dictionary<T1, T2> Dictionary<T1, T2>()
        {
            return It.IsAny<Dictionary<T1, T2>>();
        }

        /// <summary>
        /// Wrapper for <see cref="It.IsAny{KeyValuePair{T1, T2}}()"/>.
        /// </summary>
        public static KeyValuePair<T1, T2> KeyValuePair<T1, T2>()
        {
            return It.IsAny<KeyValuePair<T1, T2>>();
        }

        /// <summary>
        /// Wrapper for <see cref="It.IsAny{Tuple{T1, T2}}()"/>.
        /// </summary>
        public static Tuple<T1, T2> Tuple<T1, T2>()
        {
            return It.IsAny<Tuple<T1, T2>>();
        }
    }
}
