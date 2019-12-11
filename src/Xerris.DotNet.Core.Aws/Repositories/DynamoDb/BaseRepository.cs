using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Xerris.DotNet.Core.Extensions;

namespace Xerris.DotNet.Core.Aws.Repositories.DynamoDb
{
    public interface IBaseRepository<in T> where T : class
    {
        Task SaveAsync(T toUpdate);
        Task DeleteAsync(T toDelete);
    }
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly IAmazonDynamoDB client;

        protected Table Table { get; }
        
        protected BaseRepository(IAmazonDynamoDB client, string tableName)
        {
            this.client = client;
            Table = Table.LoadTable(client, tableName);
        }

        protected async Task<TU> FindOneAsync<TU>(ScanCondition where, bool allowNull = true)
        {
            using (var context = new DynamoDBContext(client))
            {
                var search = context.ScanAsync<TU>(new[] {where}, CreateOperationConfig());

                while (!search.IsDone)
                {
                    var entities = await search.GetNextSetAsync();
                    if (entities.Any()) return entities.FirstOrDefault();
                }
                if (allowNull) return default;
            }
            throw new NotFoundException<T>(where);
        }

        public async Task SaveAsync(T toUpdate)
        {
            var item = Document.FromJson(toUpdate.ToJson(JsonExtensions.DefaultCaseSettings));
            await Table.PutItemAsync(item);
        }

        public async Task DeleteAsync(T toDelete)
        {
            await Table.DeleteItemAsync(Document.FromJson(toDelete.ToJson(JsonExtensions.DefaultCaseSettings)));
        }

        private DynamoDBOperationConfig CreateOperationConfig()
        {
            return new DynamoDBOperationConfig { OverrideTableName = Table.TableName };
        }

        protected ScanCondition WhereEquals(string field, string value)
        {
            return new ScanCondition(field, ScanOperator.Equal, value);
        }
    }
}