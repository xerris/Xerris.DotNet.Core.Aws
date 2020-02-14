using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;

namespace Xerris.DotNet.Core.Aws.Repositories.DynamoDb
{
    public interface ITable
    {
        Task PutItemAsync(Document toAdd);
        Task DeleteItemAsync(Document fromJson);
        string TableName { get; set; }
    }
}