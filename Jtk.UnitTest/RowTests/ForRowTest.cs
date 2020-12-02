namespace Jtk.UnitTest.RowTests
{	
	public static class ForRowTest
	{       
        /// Usage
        /// new [] 
        /// {
        ///     new {Section = 1, ShouldDisplay = true },
        ///     new {Section = 2, ShouldDisplay = false },
        /// }.ExecuteRowTests (testCase => {___})
        public static T[] Items<T>(params T[] items)
		{
			return items;
		}	
	}
}