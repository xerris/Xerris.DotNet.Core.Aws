using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Xerris.DotNet.Core.Aws.IoC;

namespace Xerris.DotNet.Core.Aws.Repositories.DynamoDb
{
    public class TableProxy : ITable
    {
        private readonly ILazyProvider<IAmazonDynamoDB> client;
        private Table table;

        private TableProxy(ILazyProvider<IAmazonDynamoDB> client, string tableName)
        {
            this.client = client;
            this.TableName = tableName;
        }

        private Table Table
        {
            get { return table ??= Table.LoadTable(client.Create(), TableName); }
        }

        public async Task PutItemAsync(Document toAdd)
        {
            await Table.PutItemAsync(toAdd);
        }

        public async Task DeleteItemAsync(Document toDelete)
        {
            await Table.DeleteItemAsync(toDelete);
        }

        public string TableName { get; }

        public static ITable Create(ILazyProvider<IAmazonDynamoDB> client, string tableName)
        {
            return new TableProxy(client, tableName);
        }
    }
}