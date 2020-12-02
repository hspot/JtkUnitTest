# Jtk.UnitTest
- Jtk.Unit is a set of common extension and unit test helper methods

## Any Moq Helpers
Wrapper to make it simpler to use Moq It.IsAny methods

### Any.Bool, Any. Int, etc
Instead of using `It.IsAny<bool>` or `It.IsAny<Decimal>` or `It.IsAny<Double>` or `It.IsAny<DateTime>` or `It.IsAny<int>` or `It.IsAny<string>`, use the following to produce more concise and readable code:
```
Any.Bool
Any.Decimal
Any.Double
Any.DateTime
Any.Int
Any.String
```
### Any.InstanceOf<T>()
Instead of using `It.IsAny<MyClass>()`, use the following to produce more concise and readable code:
```
Any.InstanceOf<MyClass>()
```

### Any.Array<T>(), Any.Dictionary<T1,T2>(), etc
Instead of using `It.IsAny<MyClass[]>()` or `It.IsAny<IEnumerable<MyClass>>()` or `It.IsAny<List<MyClass>>()` or `It.IsAny<Dictionary<string, MyClass>>()`, or `It.Any<KeyValuePair<string, MyClass>>()` or `It.Any<Tuple<string, MyClass>>()`, use the following to produce more concise and readable code:
```
Any.Array<MyClass>()
Any.IEnumerable<MyClass>()
Any.List<MyClass>()
Any.Dictionary<string, MyClass>()
Any.KeyValuePair<string, MyClass>()
Any.Tuple<string, MyClass>()
```

## General Moq Extensions
Wrapper to make it simpler to use Moq Moq Verify methods

### VerifyOneCallTo(), VerifyNoCallsTo(), etc
Instead of using `.Verify(expression, Times.Exactly(callCount))` or `.Verify(expression, Times.Exactly(callCount))` or `.Verify(express, Times.Once)` or `.Verify(express, Times.Once)`, `.Verify(expression, Times.AtLeastOnce)` or `.Verify(expression, Times.AtLeastOnce)` or `.Verify(expression, Times.AtMostOnce)` or `.Verify(expression, Times.AtMostOnce)` or `.Verify(expression, Times.Never())` or `.Verify(expression, Times.Never())` or `.VerifyGet(expression, Times.Exactly(getCount))` or `.VerifyGet(expression, Times.Once)` or `.VerifyGet(expression, Times.AtLeastOnce)` or `.VerifyNoGets(expression, Times.Never` or `.VerifySet(expression, Times.Exactly(setCount))` or `.VerifySet(expression, Times.Once)` or `.VerifySet(expression, Times.AtLeastOnce)` or `.VerifySet(expression, Time.Never())`, use the following to produce more concise and readable code:
```
.VerifyCallCount(callCount, expression)
.VerifyCallCount(callCount, expression = 12)
.VerifyOneCallTo(expression)
.VerifyOneCallTo(expression = 12)
.VerifyAtLeastOneCallTo(expression)
.VerifyAtLeastOneCallTo(expression = 12)
.VerifyAtMostOneCallTo(expression)
.VerifyAtMostOneCallTo(expression = 12)
.VerifyNoCallsTo(expression)
.VerifyNoCallsTo(expression = 12)
.VerifyGetCount(getCount, expression)
.VerifyOneGet(expression)
.VerifyAtLeastOneGet(expression)
.VerifyNoGets(expression)
.VerifySetCount(setCount, expression)
.VerifyOneSet(expression)
.VerifyAtLeastOneSet(expression)
.VerifyNoSet(expression)
```

## Object Comparer
Helper class for comparing the objects by automatically comparing all of object's properties. This eliminates the need of explicitly compare many object properties or writing many IComparable implementations. Without Object Comparer, A typical example of a test could be :
```
    Assert.Equal(111, myCompanyActualResult.Id);
    Assert.Equal("Description1", myCompanyActualResult.Description);
    Assert.Equal(CompanyType.Type1, myCompanyActualResult.Type);
    Assert.Equal(12232.12, myCompanyActualResult.Revenue);
    Assert.Equal("63141", myCompanyActualResult.Address.Zip);
    Assert.Equal("MyStreet St", myCompanyActualResult.Address.Street);
    ...
```

### ArePropertiesEqual
Use the following to compares the properties of two objects to determine if they are equal. Properties that implement IComparable are compared  with .CompareTo(), and properties that do not are compared with .Equals().

By default, ObjectComparer only compare first-level properties of the referenced object. To ask ObjectComparer to traverse and compare properties of sub-objects of the first level object, use the `ShouldRecursivelyCompareSubProperties` ComparisionOption.
```
var expectedCompany = new Company 
{
    Id = 111,
    Description = "Description1",
    Type = CompanyType.Type1,
    Revenue = 12232.12
}

var actualCompany = companyService.GetCompany(id);

Assert.True(ObjectComparer.ArePropertiesEqual(expectedCompany, actualCompany))
OR
Assert.True(ObjectComparer.ArePropertiesEqual(expectedCompany, actualCompany, 
    new ComparisonOptions { ShouldRecursivelyCompareSubProperties = true }))
```

### AssertPropertiesAreEqual
Use the following to assert that the properties of two objects to determine if they are equal. Properties that implement IComparable are compared  with .CompareTo(), and properties that do not are compared with .Equals(). 

By default, ObjectComparer only compare first-level properties of the referenced object. To ask ObjectComparer to traverse and compare properties of sub-objects of the first level object, use the `ShouldRecursivelyCompareSubProperties` ComparisionOption.
```
ObjectComparer.AssertPropertiesAreEqual(expectedCompany, actualCompany);

ObjectComparer.AssertPropertiesAreEqual(expectedCompany, actualCompany, 
    new ComparisonOptions { ShouldRecursivelyCompareSubProperties = true });
```

### MatchPropertyValues
Returns a Moq Match<T> that will match an object with the same property values as a given object. Properties that implement IComparable are compared  with .CompareTo(), and properties that do not are compared with .Equals()

By default, ObjectComparer only compare first-level properties of the referenced object. For the example below, ObjectComparer will compare the three properties of `expectedMyCompanyObject`.
```
var expectedMyCompanyObject = new MyCompany
{
    Name = name,
    Number = "434732428",
    AnotherNumber = "434732428"
};

_myService.Verify(s => s.Process(name, 
    ObjectComparer.MatchPropertyValues(expectedMyCompanyObject)));
```

To ask ObjectComparer to traverse and compare properties of sub-objects of the first level object, use the `ShouldRecursivelyCompareSubProperties` ComparisionOption. For the example below, properties of `Person` are also compared:
```
IPerson Person = new Person { BusinessLegalName = "Legal name 1" };
var personWithSensitiveData = new PersonWithSensitiveData { Person = Person,    FullTIN = string.Empty };

 var compareOptions = new ComparisonOptions { ShouldRecursivelyCompareSubProperties = true };

_mockMyService.VerifyOneCallTo(a => a.PopulateMerchantApplication(merchant, salesProfile, 
    ObjectComparer.MatchPropertyValues(personWithSensitiveData, compareOptions)));
```

### ObjectComparer.ComparisonOptions

**ShouldRecursivelyCompareSubProperties**: The value that indicates whether the subproperties of a property should be compared, if there is no other way to compare the properties (e.g., if the property type does not implement IComparible or override Equals). If a property implements IEnumerable, the properties of the items it contains will be compared.

**ComparisonType:** The type of comparison that should be done. Defaults to Inclusive

**PropertiesToIgnore:** A collection of property names that should be ignored. Not used for Exclusive comparisons. Properties should be specified in the following manner: "PropertyName.SubPropertyName.SubSubPropertyName". To specify a property of a collection item, use []: "PropertyName.IEnumerablePropertyName[].ItemPropertyName". 

**PropertiesToCompare:** A collection of property names that should be compared when performing an Exclusive comparison. Properties should be specified in the following manner: "PropertyName.SubPropertyName.SubSubPropertyName". To specify a property of a collection item, use []: "PropertyName.IEnumerablePropertyName[].ItemPropertyName". 

**PropertiesToCompareByReference:** A collection of property names that should be compared using .ReferenceEquals(), even if Should ShouldRecursivelyCompareSubProperties has been set to true. Properties should be specified in the following manner: "PropertyName.SubPropertyName.SubSubPropertyName". To specify a property of a collection item, use []: "PropertyName.IEnumerablePropertyName[].ItemPropertyName". 

### ITestOutputHelper
To get logs about properties that were equal or not equal, provide the ITestOutputHelper as follow:
```
public ObjectComparer_UT(ITestOutputHelper testOutputHelper)
{
    ObjectComparer.SetTestOutputHelper(testOutputHelper);
}
```

## Collection Comparer
Comparison Helper class for determining if collections of complex objects are equal

### AreCollectionsEqual
Determines whether two collections are equal by checking if they contain the same items, in the same order. _Note that this overload determines if items are equal by calling Equals(), so two items of a reference type with the same property values will not be considered equal (unless Equals() and .GetHashCode() have been overridden for the type). This is something that unit test framework like xUnit already supports._
```
Assert.True(CollectionComparer.AreCollectionsEqual(collection1, collection2));
```

### AreCollectionsEquivalent
Determines whether two collections are equivalent by checking if they contain the same items, in the any order. _Note that this overload determines if items are equal by calling Equals(), so two items of a reference type with the same property values will not be considered equal (unless Equals() and .GetHashCode() have been overridden for the type)._
```
Assert.False(CollectionComparer.AreCollectionsEquivalent(collection1, collection2));
```

### AreCollectionsEqualByItemProperties
Determines whether two collections are equal by checking if they contain the same items, in the same order. Items equality is determined by comparing each item's properties.
```
var expectedResult = new List<MyCompanyData>
{
    new MyCompanyData // 1
    {
        ResponseId = 1,
        RiskCodes = new List<string>
        {
            "20","00","00"
        },
        CompanyIndex = "50",
        ContactId = "1",
        CompanyRiskCodes = new List<string>
        {
            "25","62"
        },
        PersonIndex = "10",        
    },
    new MyCompanyData // 2
    {
        ResponseId = 1,
        RiskCodes = new List<string>
        {
            "20","00","00"
        },
        CompanyIndex = "50",
        ContactId = "2",                      
        CompanyRiskCodes = new List<string>
        {
            "81","80"
        },
        PersonIndex = "20",
    }    
};

var result = await _testObject.Process(MerchantNumber, _businessContacts);

var comparisonOptions = new ComparisonOptions
{
    ShouldRecursivelyCompareSubProperties = true,
};
Assert.True(CollectionComparer.AreCollectionsEqualByItemProperties(expectedResult, result, comparisonOptions));
```

### AreCollectionsEquivalentByItemProperties
Determines whether two collections are equal by checking if they contain the same items, any order. Items equality is determined by comparing each item's properties.

See example for `AreCollectionsEqualByItemProperties` above with the understanding that the **order of expected results does not matter.**

### MatchEqualIEnumerable
Returns a Moq Match IEnumerable<T> that can be used with .Setup, .Verify, etc to match an 'Equal' collection. _Note that this overload determines if items are equal by calling Equals(), so two items of a reference type with the same property values will not be considered equal (unless Equals() and .GetHashCode() have been overridden for the type)_
```
_mockMyService.VerifyOneCallTo(a => a.Process
    (CollectionComparer.MatchEqualIEnumerable(personWithSensitiveData, null)));
```

### MatchEquivalentList
Returns a Moq Match List<T> that can be used with .Setup, .Verify, etc to match an 'Equivalent' collection. _Note that this overload determines if items are equal by calling Equals(), so two items of a reference type with the same property values will not be considered equal (unless Equals() and .GetHashCode() have been overridden for the type)_
```
_mockMyService.VerifyOneCallTo(a => a.Process(
    CollectionComparer.MatchEquivalentList(personWithSensitiveData, null)));
```

### MatchEqualIEnumerableByItemProperties
Returns a Moq Match IEnumerable<T> that can be used with .Setup, .Verify, etc to match an 'Equal' collection. _**Note that this overload determines if items are equal by calling using the ObjectComparer to match item property values.**_

`ObjectComparer.ComparisonOptions` parameter can be used to control how objects inside the collection are compared.
```
var comparisonOptions = new ComparisonOptions
{
    ShouldRecursivelyCompareSubProperties = true,
};

_mockMyService.VerifyOneCallTo(a => a.Process
    (CollectionComparer.MatchEqualIEnumerableByItemProperties(personWithSensitiveData, comparisonOptions)));
```

### MatchEquivalentIEnumerableByItemProperties
Returns a Moq Match IEnumerable<T> that can be used with .Setup, .Verify, etc to match an 'Equivalent' collection. _**Note that this overload determines if items are equal by calling using the ObjectComparer to match item property values.**_

`ObjectComparer.ComparisonOptions` parameter can be used to control how objects inside the collection are compared.

```
_mockMyService.VerifyOneCallTo(a => a.Process
    (CollectionComparer.MatchEqualIEnumerableByItemProperties(personWithSensitiveData, null)));
```

### ITestOutputHelper
To get logs about properties that were equal or not equal, provide the ITestOutputHelper as follow:
```
public CollectionComparer_UT(ITestOutputHelper testOutputHelper)
{
    CollectionComparer.SetTestOutputHelper(testOutputHelper);
}
```

## RowTest
RowTest allow to cleaner and more direct way to test multiple tests each with its own set of test data. RowTest is especially helpful when the test data include reference types.

### ExecuteRowTests
Instead of creating separate methods to return test data and expected result with reference types as below:
```
private class TestDataObject{
    List<ApplicationType> TestData {get; set;}
    ApplicationPermissions ExpectedResult {get; set;}
}

public static IEnumerable<object[]> TestData
{
    get 
    {
        var testData = new List<TestDataObject>
        {
            new TestDataObject 
            {
                TestData = new List<ApplicationType>
                {
                    ApplicationType.Type1
                },
                ExpectedResult = new ApplicationPermissions
                {
                    ...       
                }
            },
            ...
        }

        //Convert to list of objects
    }

    [Theory, MemberData(nameof(TestData))]
    public Test1(ApplicationPermissions testObject)
    {
        ...
    }
}

```

Use the following as a cleaner and more direct way to set up the multiple tests with reference data objects:
```
new[]
{
    new // Nothing is true
    {
        PermissionList = new List<ApplicationType>() {
        },
        ExpectedPermissionObject = new ApplicationPermissions {
                CanEditBankAccounts = false, CanEditSocialSecurityNumber = false, CanEditPerson = false
        }                    
    },
    new // Only CanEditBankAccounts = true
    {
        PermissionList = new List<ApplicationType>() {
            ApplicationType.EditBankAccounts
        },
        ExpectedPermissionObject = new ApplicationPermissions {
                CanEditBankAccounts = true, CanEditSocialSecurityNumber = false, CanEditPerson = false
        }
    }    
}
.WithMockResetCallsFor(_repositoryMock)
.ExecuteRowTests(row =>
{
    var guid = new Guid();

    _repositoryMock.Setup(a => a.GetApplicationPermissions(guid)).Returns(row.PermissionList);
    var actualPermObject = _applicationTokenManager.GetApplicationPermissions(guid);

    ObjectComparer.AssertPropertiesAreEqual(row.ExpectedPermissionObject, actualPermObject);
});
```

### WithMockResetCallsFor
Reset usage, count, etc for mocks before each test run.
```
See example above
```

## HttpClientStub
HttpClient stub to dynamically set the values to return for HttpResponseMessage, etc

### Usage
By having a test object using _httpClientStub.HttpClient dependency, we can have the HttpClient returns the desired _responseText response text.
```
_httpClientStub = new HttpClientStub();

_testObject = new TransUnionService(_configuration, _logger.Object, _httpClientStub.HttpClient);

var response = new HttpResponseMessage
{
    Content = new StringContent(_responseText),
    StatusCode = System.Net.HttpStatusCode.OK
};
_httpClientStub.Response = response;
```