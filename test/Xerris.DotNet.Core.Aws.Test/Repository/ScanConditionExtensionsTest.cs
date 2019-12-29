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
            AssertEquals(eq,  nameof(Foo.FirstName), ScanOperator.Equal,angelina.FirstName);
        }
        
        [Fact]
        public void NotEqual()
        {
            var eq = this.NotEqual<Foo>(x => x.LastName, angelina.LastName);
            AssertEquals(eq,  nameof(Foo.LastName), ScanOperator.NotEqual,angelina.LastName);
        }
        
        [Fact]
        public void Contains()
        {
            var eq = this.Contains<Foo>(x => x.LastName, angelina.LastName);
            AssertEquals(eq,  nameof(Foo.LastName), ScanOperator.Contains,angelina.LastName);
        }
        
        [Fact]
        public void NotContains()
        {
            var eq = this.NotContains<Foo>(x => x.LastName, angelina.LastName);
            AssertEquals(eq,  nameof(Foo.LastName), ScanOperator.NotContains,angelina.LastName);
        }
        
        [Fact]
        public void GreaterThan()
        {
            var eq = this.GreaterThan<Foo>(x => x.Age, angelina.Age);
            AssertEquals(eq,  nameof(Foo.Age), ScanOperator.GreaterThan,angelina.Age);
        }
        
        [Fact]
        public void GreaterThanOrEqual()
        {
            var eq = this.GreaterThanOrEqual<Foo>(x => x.Age, angelina.Age);
            AssertEquals(eq,  nameof(Foo.Age), ScanOperator.GreaterThanOrEqual,angelina.Age);
        }
        
        [Fact]
        public void LessThan()
        {
            var eq = this.LessThan<Foo>(x => x.Age, angelina.Age);
            AssertEquals(eq,  nameof(Foo.Age), ScanOperator.LessThan,angelina.Age);
        }
        
        [Fact]
        public void LessThanOrEqual()
        {
            var eq = this.LessThanOrEqual<Foo>(x => x.Age, angelina.Age);
            AssertEquals(eq,  nameof(Foo.Age), ScanOperator.LessThanOrEqual,angelina.Age);
        }

        [Fact]
        public void TwoScanConditions()
        {
            var condition = this.Equals<Foo>(x => x.FirstName, angelina.FirstName)
                .And(this.GreaterThan<Foo>(x => x.Age, angelina.Age))
                .And(this.NotEqual<Foo>(x => x.LastName, "Pitt")).ToList();
            
            Validate.Begin()
                .IsNotNull(condition, "condition").Check()
                .IsNotEmpty(condition, "conditions not empty").Check()
                .IsEqual(condition.Count(), 3, "found 3 conditions");

            
            AssertEquals(condition[0], nameof(Foo.FirstName), ScanOperator.Equal, angelina.FirstName);
            AssertEquals(condition[1], nameof(Foo.Age), ScanOperator.GreaterThan, angelina.Age);
            AssertEquals(condition[2], nameof(Foo.LastName), ScanOperator.NotEqual, "Pitt");
        }

        private static void AssertEquals(ScanCondition actual, string propertyName, ScanOperator oper, object value)
        {
            Validate.Begin()
                .IsNotNull(actual, "actual").Check()
                .IsNotNull(actual.Values, "value").Check()
                .IsNotEmpty(actual.Values, "values not empty").Check()
                .IsEqual(actual.Values.Length, 1, "values count").Check()
                //check the value of the scan condition
                .IsEqual(actual.Operator, oper, "operator")
                .IsEqual(actual.PropertyName, propertyName, "property name")
                .IsEqual(actual.Values.First(), value, "actual value")
                .Check();
        }
    }
}