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
        public static ScanCondition Equals<TModel>(this object obj, Expression<Func<TModel, object>> sourceProperty, TModel subject)
        {
            var property = sourceProperty.GetProperty();
            return new ScanCondition(property.Name, ScanOperator.Equal, property.GetValue(subject));
        }
        
        public static ScanCondition NotEqual<TModel>(this object obj, Expression<Func<TModel, object>> sourceProperty, TModel subject)
        {
            var property = sourceProperty.GetProperty();
            return new ScanCondition(property.Name, ScanOperator.NotEqual, property.GetValue(subject));
        }
        
        public static ScanCondition NotEqual<TModel>(this object obj, Expression<Func<TModel, object>> sourceProperty, object subject)
        {
            var property = sourceProperty.GetProperty();
            return new ScanCondition(property.Name, ScanOperator.NotEqual, subject);
        }
        
        public static ScanCondition Contains<TModel>(this object obj, Expression<Func<TModel, object>> sourceProperty, TModel subject)
        {
            var property = sourceProperty.GetProperty();
            return new ScanCondition(property.Name, ScanOperator.Contains, property.GetValue(subject));
        }
        
        public static ScanCondition NotContains<TModel>(this object obj, Expression<Func<TModel, object>> sourceProperty, TModel subject)
        {
            var property = sourceProperty.GetProperty();
            return new ScanCondition(property.Name, ScanOperator.NotContains, property.GetValue(subject));
        }
        
        public static ScanCondition GreaterThan<TModel>(this object obj, Expression<Func<TModel, object>> sourceProperty, TModel subject)
        {
            var property = sourceProperty.GetProperty();
            return new ScanCondition(property.Name, ScanOperator.GreaterThan, property.GetValue(subject));
        }
        
        public static ScanCondition GreaterThanOrEqual<TModel>(this object obj, Expression<Func<TModel, object>> sourceProperty, TModel subject)
        {
            var property = sourceProperty.GetProperty();
            return new ScanCondition(property.Name, ScanOperator.GreaterThanOrEqual, property.GetValue(subject));
        }
        
        public static ScanCondition LessThan<TModel>(this object obj, Expression<Func<TModel, object>> sourceProperty, TModel subject)
        {
            var property = sourceProperty.GetProperty();
            return new ScanCondition(property.Name, ScanOperator.LessThan, property.GetValue(subject));
        }
        
        public static ScanCondition LessThanOrEqual<TModel>(this object obj, Expression<Func<TModel, object>> sourceProperty, TModel subject)
        {
            var property = sourceProperty.GetProperty();
            return new ScanCondition(property.Name, ScanOperator.LessThanOrEqual, property.GetValue(subject));
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