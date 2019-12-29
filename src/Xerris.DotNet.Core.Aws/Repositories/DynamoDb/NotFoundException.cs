using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;

namespace Xerris.DotNet.Core.Aws.Repositories.DynamoDb
{
    public class NotFoundException<T> : Exception
    {
        public NotFoundException(ScanCondition condition) : base($"{typeof(T)} not found where {condition}")
        {
        }
        
        public NotFoundException(IEnumerable<ScanCondition> where) : base($"{typeof(T)} not found where {where}")
        {
        }
    }
}