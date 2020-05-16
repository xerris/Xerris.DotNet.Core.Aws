using System;
using Amazon.DynamoDBv2.Model;
using Xerris.DotNet.Core.Extensions;

namespace Xerris.DotNet.Core.Aws.DynamoDb
{
    public static class AttributeValueExtensions
    {
        public static T Convert<T>(this AttributeValue value, Func<AttributeValue, T> converter)
        {
            return converter(value);
        }

        public static DateTime ToDateTime(this AttributeValue value)
        {
            return value.Convert(x => DateTime.Parse(x.S));
        }

        public static T ToEnum<T>(this AttributeValue value) where T : struct, IConvertible
        {
            return value.Convert(x => x.N.ToString().ToEnum<T>());
        }
    }
}