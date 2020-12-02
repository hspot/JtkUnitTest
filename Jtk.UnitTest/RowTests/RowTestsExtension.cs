using System;
using System.Collections.Generic;
using Moq;

namespace Jtk.UnitTest.RowTests
{
	/// <summary>
	/// Extension methods to assist with RowTests
	/// </summary>
	public static class RowTestsExtension
	{	
		public static RowTests<TTestCase> ExecuteRowTests<TTestCase>(
			this IEnumerable<TTestCase> enumerable, Action<TTestCase> executeTestCase)
		{
			return new RowTests<TTestCase>(enumerable).ExecuteRowTests(executeTestCase);
		}
		
		public static RowTests<TTestCase> WithMockResetCallsFor<TTestCase>(
			this IEnumerable<TTestCase> enumerable,
			params Mock[] mocksToResetCalls)
		{
			return new RowTests<TTestCase>(enumerable, mocksToResetCalls); 
		}
	}
}