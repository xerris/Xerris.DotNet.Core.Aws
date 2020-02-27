using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xerris.DotNet.Core.Extensions;
using DynamoTable=Amazon.DynamoDBv2.DocumentModel;

namespace Xerris.DotNet.Core.Aws.Repositories.DynamoDb
{
    public interface IBaseRepository<in T> where T : class
    {
        Task SaveAsync(T toUpdate);
        Task DeleteAsync(T toDelete);
        Task<IEnumerable<TU>> FindAllAsync<TU>(IEnumerable<ScanCondition> where);
        Task<IEnumerable<TU>> FindAllAsync<TU>();
        Task<TU> FindOneAsync<TU>(ScanCondition where, bool allowNull = true);
    }
    
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private static readonly JsonSerializerSettings DynamoDbJsonSerializationSettings =
                          new JsonSerializerSettings
                          {
                              ContractResolver = new DefaultContractResolver()
                          };
        
        private readonly IAmazonDynamoDB client;
        private ITable Table { get; }
        
        
        protected BaseRepository(IAmazonDynamoDB client, string tableName) : this(client, TableProxy.Create(client, tableName))
        {
        }

        protected BaseRepository(IAmazonDynamoDB client, ITable table)
        {
            this.client = client;
            Table = table;
        }
        
        public async Task<TU> FindOneAsync<TU>(ScanCondition where, bool allowNull = true)
        {
            return await FindOneAsync<TU>(new[] {where}, allowNull);
        }

        protected async Task<TU> FindOneAsync<TU>(IEnumerable<ScanCondition> where, bool allowNull = true)
        {
            using (var context = new DynamoDBContext(client))
            {
                var search = context.ScanAsync<TU>(where, CreateOperationConfig());

                while (!search.IsDone)
                {
                    var entities = await search.GetNextSetAsync();
                    if (entities.Any()) return entities.FirstOrDefault();
                }
                if (allowNull) return default;
            }
            throw new NotFoundException<T>(where);
        }

        public async Task<IEnumerable<TU>> FindAllAsync<TU>(IEnumerable<ScanCondition> where)
        {
            using var context = new DynamoDBContext(client);
            var results = new List<TU>();
            var search = context.ScanAsync<TU>(where, CreateOperationConfig());

            while (!search.IsDone)
            {
                var entities = await search.GetNextSetAsync();
                if (entities.Any())
                    results.AddRange(entities);
            }

            return results;
        }

        public async Task<IEnumerable<TU>> FindAllAsync<TU>(ScanCondition where)
        {
            return await FindAllAsync<TU>(new[] {where});
        }

        public async Task<IEnumerable<TU>> FindAllAsync<TU>()
        {
            return await FindAllAsync<TU>(Enumerable.Empty<ScanCondition>());
        }

        public async Task SaveAsync(T toUpdate)
        {
            var item = Document.FromJson(toUpdate.ToJson(DynamoDbJsonSerializationSettings));
            await Table.PutItemAsync(item);
        }

        public async Task DeleteAsync(T toDelete)
        {
            await Table.DeleteItemAsync(Document.FromJson(toDelete.ToJson(DynamoDbJsonSerializationSettings)));
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