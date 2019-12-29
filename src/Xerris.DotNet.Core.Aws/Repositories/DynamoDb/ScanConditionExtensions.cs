using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Xerris.DotNet.Core.Extensions;

namespace Xerris.DotNet.Core.Aws.Repositories.DynamoDb
{
    public static class ScanConditionExtensions
    {
        public static ScanCondition Equals<TModel>(this object obj, Expression<Func<TModel, object>> sourceProperty, object value)
        {
            var property = sourceProperty.GetProperty();
            return new ScanCondition(property.Name, ScanOperator.Equal, value);
        }
        
        public static ScanCondition NotEqual<TModel>(this object obj, Expression<Func<TModel, object>> sourceProperty, object value)
        {
            var property = sourceProperty.GetProperty();
            return new ScanCondition(property.Name, ScanOperator.NotEqual, value);
        }
        
        public static ScanCondition Contains<TModel>(this object obj, Expression<Func<TModel, object>> sourceProperty, object value)
        {
            var property = sourceProperty.GetProperty();
            return new ScanCondition(property.Name, ScanOperator.Contains, value);
        }
        
        public static ScanCondition NotContains<TModel>(this object obj, Expression<Func<TModel, object>> sourceProperty, object value)
        {
            var property = sourceProperty.GetProperty();
            return new ScanCondition(property.Name, ScanOperator.NotContains, value);
        }
        
        public static ScanCondition GreaterThan<TModel>(this object obj, Expression<Func<TModel, object>> sourceProperty, object value)
        {
            var property = sourceProperty.GetProperty();
            return new ScanCondition(property.Name, ScanOperator.GreaterThan, value);
        }
        
        public static ScanCondition GreaterThanOrEqual<TModel>(this object obj, Expression<Func<TModel, object>> sourceProperty, object value)
        {
            var property = sourceProperty.GetProperty();
            return new ScanCondition(property.Name, ScanOperator.GreaterThanOrEqual, value);
        }
        
        public static ScanCondition LessThan<TModel>(this object obj, Expression<Func<TModel, object>> sourceProperty, object value)
        {
            var property = sourceProperty.GetProperty();
            return new ScanCondition(property.Name, ScanOperator.LessThan, value);
        }
        
        public static ScanCondition LessThanOrEqual<TModel>(this object obj, Expression<Func<TModel, object>> sourceProperty, object value)
        {
            var property = sourceProperty.GetProperty();
            return new ScanCondition(property.Name, ScanOperator.LessThanOrEqual, value);
        }

        public static IEnumerable<ScanCondition> And(this ScanCondition where, ScanCondition toAdd)
        {
            return new List<ScanCondition> {where, toAdd};
        }

        public static IEnumerable<ScanCondition> And(this IEnumerable<ScanCondition> where, ScanCondition toAdd)
        { 
            return where.Append(toAdd);
        }
    }
}