using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;

namespace Xerris.DotNet.Core.Aws.Repositories.DynamoDb
{
    public class TableProxy : ITable
    {
        private readonly Table table;

        private TableProxy(Table table)
        {
            this.table = table;
            TableName = table.TableName;
        }

        public async Task PutItemAsync(Document toAdd)
        {
            await table.PutItemAsync(toAdd);
        }

        public async Task DeleteItemAsync(Document toDelete)
        {
            await table.DeleteItemAsync(toDelete);
        }

        public string TableName { get; set; }

        public static ITable Create(IAmazonDynamoDB client, string tableName)
        {
            return new TableProxy(Table.LoadTable(client, tableName));
        }
    }
}