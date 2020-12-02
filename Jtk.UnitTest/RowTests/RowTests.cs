using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Xunit;

namespace Jtk.UnitTest.RowTests
{
    //TODO: #UnitTestExt Howard: Need to add unit tests for this and set up to not upload unit tests project to Nuget
    //TODO: #UnitTestExt Howard: Try not to use Newtonsoft and it complicates usage in app that also use Newtonsoft
    public class RowTests<TTestCase>
	{
		private readonly IEnumerable<TTestCase> _testCases;
		
		private readonly IEnumerable<Mock> _mocksToResetCalls;

		private readonly Regex _typeNameSuffixRegex
			= new Regex(@", [^\]]*, Version=[^\]]*, Culture=[^\]]*, PublicKeyToken=[^\]]*",
			RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

		private readonly Regex _genericVersionRegex = new Regex(@"`\d+",
			RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

		private int _testCaseCount;
		private int _testCaseIndex;

		public RowTests(IEnumerable<TTestCase> testCases, params Mock[] mocksToResetCalls)
		{
			_mocksToResetCalls = mocksToResetCalls;
			_testCases = testCases;
		}

		public RowTests<TTestCase> ExecuteRowTests(Action<TTestCase> executeTestCase)
		{
			_testCaseCount = _testCases.Count();
			_testCaseIndex = -1;
			var caughtExceptions = new List<TestException>();

			foreach (TTestCase testCase in _testCases)
			{
				_testCaseIndex++;

				try
				{
					foreach (Mock mock in _mocksToResetCalls)
					{
						mock.ResetCalls();
					}

					executeTestCase(testCase);
				}
				catch (Exception ex)
				{
					SaveCaughtException(ex, caughtExceptions);
				}
			}

			HandleCaughtExceptions(caughtExceptions);

			return this;
		}

		private void SaveCaughtException(Exception ex, List<TestException> list)
		{
			list.Add(new TestException(
				ex.GetType(),
				string.Format("{0} - {1}{2}", GetTestCaseDescription(), ex.Message, GetTestCaseJson())));
		}

		private static void HandleCaughtExceptions(List<TestException> caughtExceptions)
		{
			if (caughtExceptions.Any())
			{
				string message = string.Join("\n", caughtExceptions.Select(x => x.Message).ToArray());

				if (caughtExceptions.Any())
				{                                  
                    Assert.True(false, message);
				}
			}
		}

		private string GetTestCaseDescription()
		{
			return string.Format("TestCase[{0}] ({1} of {2})", _testCaseIndex, _testCaseIndex + 1, _testCaseCount);
		}

		private string GetTestCaseJson()
		{
			TTestCase testCase = _testCases.ElementAt(_testCaseIndex);
			string jsonString;

			try
			{
				jsonString = JsonConvert.SerializeObject(testCase, 
					new IsoDateTimeConverter { DateTimeFormat = "MM/dd/yyyy hh:mm:ss" },
					new StringEnumConverter());
			}
			catch (Exception)
			{                
				jsonString = string.Empty;
			}

			if (string.IsNullOrWhiteSpace(jsonString) || jsonString == "{}")
			{
				jsonString = string.Empty;
			}
			else
			{
				string typeName = testCase != null
					? testCase.GetType().FullName
					: "null";

				if (typeName.Contains("_Anonymous"))
				{
					typeName = "anonymousType";
				}

				typeName = _typeNameSuffixRegex.Replace(typeName, string.Empty);
				typeName = _genericVersionRegex.Replace(typeName, string.Empty);

				jsonString = string.Format("\n\nTestCase data - {0}: {1}\n", typeName, jsonString);
			}

			return jsonString;
		}

		/// <summary>
		/// Exception to denote the result of the test case
		/// </summary>
		private class TestException
		{
			/// <summary>
			/// Type of exception
			/// </summary>
			public Type ExceptionType { get; set; }

			/// <summary>
			/// Exception description message.
			/// </summary>
			public string Message { get; set; }
			
			public TestException(Type exceptionType, string message)
			{
				ExceptionType = exceptionType;
				Message = message;
			}
		}
	}
}