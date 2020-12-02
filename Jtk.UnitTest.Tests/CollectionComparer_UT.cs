using System;
using System.Collections.Generic;
using Xunit;

namespace Jtk.UnitTest.Tests
{
    public class CollectionComparer_UT
    {
        [Fact]
        public void AreCollectionsEquivalentByItemProperties_One_Item_Is_Not_Equal()
        {
            var list1 = new List<MyTestObject>
            {
                new MyTestObject
                {
                    Id = 1,
                    Description = "Description1"
                },
                new MyTestObject
                {
                    Id = 2,
                    Description = "Description2"
                }
            };

            var list2 = new List<MyTestObject>
            {
                new MyTestObject
                {
                    Id = 1,
                    Description = "Description1"
                },
                new MyTestObject
                {
                    Id = 3, // not 2 like in list 1
                    Description = "Description2"
                }
            };

            var result = CollectionComparer.AreCollectionsEqualByItemProperties(list1, list2);

            Assert.False(result);
        }

        [Fact]
        public void AreCollectionsEquivalentByItemProperties_All_Items_Is_Equal()
        {
            var list1 = new List<MyTestObject>
            {
                new MyTestObject
                {
                    Id = 1,
                    Description = "Description1"
                },
                new MyTestObject
                {
                    Id = 2,
                    Description = "Description2"
                }
            };

            var list2 = new List<MyTestObject> // data matching list1
            {
                new MyTestObject
                {
                    Id = 1,
                    Description = "Description1"
                },
                new MyTestObject
                {
                    Id = 2,
                    Description = "Description2"
                }
            };

            var result = CollectionComparer.AreCollectionsEqualByItemProperties(list1, list2);

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
