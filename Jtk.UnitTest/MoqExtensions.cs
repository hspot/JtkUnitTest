using Moq;
using System;
using System.Linq.Expressions;

namespace Jtk.UnitTest
{
    public static class MoqExtensions
    {
        // public static Expression<Func<T, TResult>> DefineExpression<T, TResult>(
        // 	this Mock<T> mock, Expression<Func<T, TResult>> expression) where T : class
        // {
        // 	return expression;
        // }

        /// <summary>
        /// Verifies that a specific invocation matching the given expression was performed on the mock exactly
        ///</summary>		
        public static void VerifyCallCount<T>(this Mock<T> mock, int callCount, Expression<Action<T>> expression)
            where T : class
        {
            mock.Verify(expression, Times.Exactly(callCount));
        }

        /// <summary>
        /// Verifies that a specific invocation matching the given expression was performed on the mock exactly
        ///</summary>	
        public static void VerifyCallCount<T, TResult>(this Mock<T> mock, int callCount, Expression<Func<T, TResult>> expression)
            where T : class
        {
            mock.Verify(expression, Times.Exactly(callCount));
        }

        /// <summary>
        /// Verifies that a specific invocation matching the given expression was performed on the mock exactly once.
        /// </summary>	
        public static void VerifyOneCallTo<T>(this Mock<T> mock, Expression<Action<T>> expression) where T : class
        {
            mock.Verify(expression, Times.Once);
        }

        /// <summary>
        /// Verifies that a specific invocation matching the given expression was performed on the mock exactly once.
        /// </summary>		
        public static void VerifyOneCallTo<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> expression) where T : class
        {
            mock.Verify(expression, Times.Once);
        }

        /// <summary>
        /// Verifies that a specific invocation matching the given expression was performed at least once.
        /// </summary>
        public static void VerifyAtLeastOneCallTo<T>(this Mock<T> mock, Expression<Action<T>> expression) where T : class
        {
            mock.Verify(expression, Times.AtLeastOnce);
        }

        /// <summary>
        /// Verifies that a specific invocation matching the given expression was performed at least once.
        /// </summary>	
        public static void VerifyAtLeastOneCallTo<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> expression) where T : class
        {
            mock.Verify(expression, Times.AtLeastOnce);
        }

        /// <summary>
        /// Verifies that a specific invocation matching the given expression was performed at most once.
        /// </summary>		
        public static void VerifyAtMostOneCallTo<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> expression) where T : class
        {
            mock.Verify(expression, Times.AtMostOnce);
        }

        /// <summary>
        /// Verifies that a specific invocation matching the given expression was performed at most once.
        /// </summary>	
        public static void VerifyAtMostOneCallTo<T>(this Mock<T> mock, Expression<Action<T>> expression) where T : class
        {
            mock.Verify(expression, Times.AtMostOnce);
        }

        /// <summary>
        /// Verifies that a specific invocation matching the given expression was never called.
        /// </summary>
        public static void VerifyNoCallsTo<T>(this Mock<T> mock, Expression<Action<T>> expression) where T : class
        {
            mock.Verify(expression, Times.Never());
        }

        /// <summary>
        /// Verifies that a specific invocation matching the given expression was never called.
        /// </summary>		
        public static void VerifyNoCallsTo<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> expression) where T : class
        {
            mock.Verify(expression, Times.Never);
        }

        /// <summary>
        /// Verifies that a property was read on the mock exactly <paramref name="getCount"/> times.
        /// </summary>	
        public static void VerifyGetCount<T, TProperty>(
            this Mock<T> mock, int getCount, Expression<Func<T, TProperty>> expression) where T : class
        {
            mock.VerifyGet(expression, Times.Exactly(getCount));
        }

        /// <summary>
        /// Verifies that a property was read on the mock once.
        /// </summary>		
        public static void VerifyOneGet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression) where T : class
        {
            mock.VerifyGet(expression, Times.Once);
        }

        /// <summary>
        /// Verifies that a property was read on the mock at least once.
        /// </summary>		
        public static void VerifyAtLeastOneGet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression) where T : class
        {
            mock.VerifyGet(expression, Times.AtLeastOnce);
        }

        /// <summary>
        /// Verifies Verifies that a property was never read on the mock.
        /// </summary>
        public static void VerifyNoGets<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression) where T : class
        {
            mock.VerifyGet(expression, Times.Never);
        }

        /// <summary>
        /// Verifies that a property was set on the mock exactly <paramref name="setCount"/> times.
        /// </summary>
        public static void VerifySetCount<T>(this Mock<T> mock, int setCount, Action<T> setterExpression) where T : class
        {
            mock.VerifySet(setterExpression, Times.Exactly(setCount));
        }

        /// <summary>
        /// Verifies that a property was set on the mock once.
        /// </summary>	
        public static void VerifyOneSet<T>(this Mock<T> mock, Action<T> setterExpression) where T : class
        {
            mock.VerifySet(setterExpression, Times.Once);
        }

        /// <summary>
        /// Verifies that a property was set on the mock at least once.
        /// </summary>
        public static void VerifyAtLeastOneSet<T>(this Mock<T> mock, Action<T> setterExpression) where T : class
        {
            mock.VerifySet(setterExpression, Times.AtLeastOnce);
        }

        /// <summary>
        /// Verifies Verifies that a property was never set on the mock.
        /// </summary>
        public static void VerifyNoSet<T>(this Mock<T> mock, Action<T> setterExpression) where T : class
        {
            mock.VerifySet(setterExpression, Times.Never);
        }
    }
}
