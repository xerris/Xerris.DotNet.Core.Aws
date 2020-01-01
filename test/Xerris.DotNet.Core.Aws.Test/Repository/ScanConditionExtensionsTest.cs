using System.Linq;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Xerris.DotNet.Core.Aws.Repositories.DynamoDb;
using Xerris.DotNet.Core.Aws.Test.Model;
using Xerris.DotNet.Core.Validations;
using Xunit;

namespace Xerris.DotNet.Core.Aws.Test.Repository
{
    public class ScanConditionExtensionsTest
    {
        private readonly Foo angelina;
        
        public ScanConditionExtensionsTest()
        {
            angelina = new Foo("Angelina", "Jolie");
        }
        
        [Fact]
        public void Equal()
        {
            var eq = angelina.Equals(x => x.FirstName, angelina);
            Validate.Begin()
                .ComparesTo<ScanCondition>(eq, new ScanCondition(nameof(Foo.FirstName), ScanOperator.Equal,angelina.FirstName), AssertEquals)
                .Check();
        }
        
        [Fact]
        public void NotEqual()
        {
            var ne = angelina.NotEqual(x => x.LastName, angelina);
            Validate.Begin()
                .ComparesTo<ScanCondition>(ne, new ScanCondition(nameof(Foo.LastName), ScanOperator.NotEqual, angelina.LastName), AssertEquals)
                .Check();
            
            ne = angelina.NotEqual<Foo>(x => x.LastName, angelina.LastName);
            Validate.Begin()
                .ComparesTo<ScanCondition>(ne, new ScanCondition(nameof(Foo.LastName), ScanOperator.NotEqual, angelina.LastName), AssertEquals)
                .Check();
        }
        
        [Fact]
        public void Contains()
        {
            var eq = angelina.Contains(x => x.LastName, angelina);
            Validate.Begin()
                .ComparesTo<ScanCondition>(eq, new ScanCondition(nameof(Foo.LastName), ScanOperator.Contains, angelina.LastName), AssertEquals)
                .Check();
        }
        
        [Fact]
        public void NotContains()
        {
            var eq = angelina.NotContains(x => x.LastName, angelina);
            Validate.Begin()
                .ComparesTo<ScanCondition>(eq, new ScanCondition(nameof(Foo.LastName), ScanOperator.NotContains, angelina.LastName), AssertEquals)
                .Check();
        }
        
        [Fact]
        public void GreaterThan()
        {
            var eq = this.GreaterThan(x => x.Age, angelina);
            Validate.Begin()
                .ComparesTo<ScanCondition>(eq, new ScanCondition(nameof(Foo.Age), ScanOperator.GreaterThan, angelina.Age), AssertEquals)
                .Check();
        }
        
        [Fact]
        public void GreaterThanOrEqual()
        {
            var eq = this.GreaterThanOrEqual(x => x.Age, angelina);
            Validate.Begin()
                .ComparesTo<ScanCondition>(eq, new ScanCondition(nameof(Foo.Age), ScanOperator.GreaterThanOrEqual, angelina.Age), AssertEquals)
                .Check();
        }
        
        [Fact]
        public void LessThan()
        {
            var eq = this.LessThan(x => x.Age, angelina);
            Validate.Begin()
                .ComparesTo<ScanCondition>(eq, new ScanCondition(nameof(Foo.Age), ScanOperator.LessThan, angelina.Age), AssertEquals)
                .Check();
        }
        
        [Fact]
        public void LessThanOrEqual()
        {
            var eq = this.LessThanOrEqual(x => x.Age, angelina);
            Validate.Begin()
                .ComparesTo<ScanCondition>(eq, new ScanCondition(nameof(Foo.Age), ScanOperator.LessThanOrEqual, angelina.Age), AssertEquals)
                .Check();
        }

        [Fact]
        public void MultipleScanConditions()
        {
            var condition = this.Equals(x => x.FirstName, angelina)
                .And(this.GreaterThan(x => x.Age, angelina))
                .And(this.NotEqual<Foo>(x => x.LastName, "Pitt")).ToList();

            Validate.Begin()
                .IsNotNull(condition, "condition").Check()
                .IsNotEmpty(condition, "conditions not empty").Check()
                .IsEqual(condition.Count(), 3, "found 3 conditions")
                .ComparesTo<ScanCondition>(condition[0], new ScanCondition(nameof(Foo.FirstName), ScanOperator.Equal, angelina.FirstName), AssertEquals)
                .ComparesTo<ScanCondition>(condition[1], new ScanCondition(nameof(Foo.Age), ScanOperator.GreaterThan, angelina.Age), AssertEquals)
                .ComparesTo<ScanCondition>(condition[2], new ScanCondition(nameof(Foo.LastName), ScanOperator.NotEqual, "Pitt"), AssertEquals)
                .Check();
        }

        private static void AssertEquals(Validation validation, ScanCondition actual, ScanCondition expected)
        {
            validation.IsNotNull(actual, "actual").Check()
                .IsNotNull(actual.Values, "value").Check()
                .IsNotEmpty(actual.Values, "values not empty").Check()
                .IsEqual(actual.Values.Length, 1, "values count").Check()
                //check the value of the scan condition
                .IsEqual(actual.Operator, expected.Operator, "operator")
                .IsEqual(actual.PropertyName, expected.PropertyName, "property name")
                .IsEqual(actual.Values.First(), expected.Values.First(), "actual value")
                .Check();
        }
    }
}