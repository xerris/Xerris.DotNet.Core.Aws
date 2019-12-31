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
            var eq = this.Equals<Foo>(x => x.FirstName, angelina.FirstName);
            Validate.Begin()
                .ComparesTo<ScanCondition>(eq, new ScanCondition(nameof(Foo.FirstName), ScanOperator.Equal,angelina.FirstName), AssertEquals)
                .Check();
        }
        
        [Fact]
        public void NotEqual()
        {
            var eq = this.NotEqual<Foo>(x => x.LastName, angelina.LastName);
            Validate.Begin()
                .ComparesTo<ScanCondition>(eq, new ScanCondition(nameof(Foo.LastName), ScanOperator.NotEqual, angelina.LastName), AssertEquals)
                .Check();
        }
        
        [Fact]
        public void Contains()
        {
            var eq = this.Contains<Foo>(x => x.LastName, angelina.LastName);
            Validate.Begin()
                .ComparesTo<ScanCondition>(eq, new ScanCondition(nameof(Foo.LastName), ScanOperator.Contains, angelina.LastName), AssertEquals)
                .Check();
        }
        
        [Fact]
        public void NotContains()
        {
            var eq = this.NotContains<Foo>(x => x.LastName, angelina.LastName);
            Validate.Begin()
                .ComparesTo<ScanCondition>(eq, new ScanCondition(nameof(Foo.LastName), ScanOperator.NotContains, angelina.LastName), AssertEquals)
                .Check();
        }
        
        [Fact]
        public void GreaterThan()
        {
            var eq = this.GreaterThan<Foo>(x => x.Age, angelina.Age);
            Validate.Begin()
                .ComparesTo<ScanCondition>(eq, new ScanCondition(nameof(Foo.Age), ScanOperator.GreaterThan, angelina.Age), AssertEquals)
                .Check();
        }
        
        [Fact]
        public void GreaterThanOrEqual()
        {
            var eq = this.GreaterThanOrEqual<Foo>(x => x.Age, angelina.Age);
            Validate.Begin()
                .ComparesTo<ScanCondition>(eq, new ScanCondition(nameof(Foo.Age), ScanOperator.GreaterThanOrEqual, angelina.Age), AssertEquals)
                .Check();
        }
        
        [Fact]
        public void LessThan()
        {
            var eq = this.LessThan<Foo>(x => x.Age, angelina.Age);
            Validate.Begin()
                .ComparesTo<ScanCondition>(eq, new ScanCondition(nameof(Foo.Age), ScanOperator.LessThan, angelina.Age), AssertEquals)
                .Check();
        }
        
        [Fact]
        public void LessThanOrEqual()
        {
            var eq = this.LessThanOrEqual<Foo>(x => x.Age, angelina.Age);
            Validate.Begin()
                .ComparesTo<ScanCondition>(eq, new ScanCondition(nameof(Foo.Age), ScanOperator.LessThanOrEqual, angelina.Age), AssertEquals)
                .Check();
        }

        [Fact]
        public void MultipleScanConditions()
        {
            var condition = this.Equals<Foo>(x => x.FirstName, angelina.FirstName)
                .And(this.GreaterThan<Foo>(x => x.Age, angelina.Age))
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