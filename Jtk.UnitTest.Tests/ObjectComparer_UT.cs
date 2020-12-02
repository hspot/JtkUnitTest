using System;
using Xunit;
using Jtk.UnitTest;
using Xunit.Abstractions;

namespace Jtk.UnitTest.Tests
{
    public class ObjectComparer_UT
    {

        public ObjectComparer_UT(ITestOutputHelper testOutputHelper)
        {
            ObjectComparer.SetTestOutputHelper(testOutputHelper);
        }

        [Fact]
        public void ArePropertiesEqual_One_Property_Is_Not_Equal()
        {
            var expected = new MyTestObject
            {
                Id = 1,
                Description = "Description"
            };

            var actual = new MyTestObject
            {
                Id = 1,
                Description = "Description Not Equal"
            };

            var result = ObjectComparer.ArePropertiesEqual(expected, actual);

            Assert.False(result);
        }

        [Fact]
        public void ArePropertiesEqual_All_Propertes_Is_Equal()
        {
            var expected = new MyTestObject
            {
                Id = 1,
                Description = "Description Equal"
            };

            var actual = new MyTestObject
            {
                Id = 1,
                Description = "Description Equal"
            };

            var result = ObjectComparer.ArePropertiesEqual(expected, actual);

            Assert.True(result);
        }

        [Fact]
        public void ArePropertiesEqual_All_Propertes_Is_Equal_InnerObjects_Not_Considerred_Equal_By_Default()
        {
            var expected = new MyTestObject
            {
                Id = 1,
                Description = "Description Equal",
                InnerObject = new MyTestInnerObject
                {
                    InnerId = 1
                }
            };

            var actual = new MyTestObject
            {
                Id = 1,
                Description = "Description Equal",
                InnerObject = new MyTestInnerObject
                {
                    InnerId = 1
                }
            };

            var result = ObjectComparer.ArePropertiesEqual(expected, actual);

            Assert.False(result);
        }

        [Fact]
        public void ArePropertiesEqual_All_Propertes_Is_Equal_InnerObjects_Are_Equal_If_Requested_Consideration()
        {
            var expected = new MyTestObject
            {
                Id = 1,
                Description = "Description Equal",
                InnerObject = new MyTestInnerObject
                {
                    InnerId = 1
                }
            };

            var actual = new MyTestObject
            {
                Id = 1,
                Description = "Description Equal",
                InnerObject = new MyTestInnerObject
                {
                    InnerId = 1
                }
            };

            var compareOption = new ObjectComparer.ComparisonOptions { ShouldRecursivelyCompareSubProperties = true };
            var result = ObjectComparer.ArePropertiesEqual(expected, actual, compareOption);

            Assert.True(result);
        }

        private class MyTestObject
        {
            public int Id { get; set; }

            public string Description { get; set; }

            public MyTestInnerObject InnerObject { get; set; }
        }

        private class MyTestInnerObject
        {
            public int InnerId { get; set; }

            public string InnerDescription { get; set; }
        }
    }
}
