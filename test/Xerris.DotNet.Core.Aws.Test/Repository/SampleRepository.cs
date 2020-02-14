using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Xerris.DotNet.Core.Aws.Repositories.DynamoDb;
using Xerris.DotNet.Core.Aws.Test.Model;

namespace Xerris.DotNet.Core.Aws.Test.Repository
{
    public class SampleRepository : BaseRepository<Foo>
    {
        public SampleRepository(IAmazonDynamoDB client, ITable table) : base(client, table)
        {
        }

        public async Task<Foo> FindForAsync(Foo subject)
        {
            var where= this.Equals<Foo>(x => x.FirstName, subject)
                          .And(this.Equals<Foo>(x => x.LastName, subject));
            return await base.FindOneAsync<Foo>(where);
        }
    }
}