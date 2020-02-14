using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Moq;
using Xerris.DotNet.Core.Aws.Repositories.DynamoDb;
using Xerris.DotNet.Core.Aws.Test.Model;
using Xerris.DotNet.Core.Validations;
using Xunit;

namespace Xerris.DotNet.Core.Aws.Test.Repository
{
    public class BaseRepositoryTests : IDisposable
    {
        private readonly MockRepository mocks;
        private readonly Mock<ITable> table;
        private readonly SampleRepository subject;
        private readonly Foo angelina;

        public BaseRepositoryTests()
        {
            mocks = new MockRepository(MockBehavior.Strict);
            table = mocks.Create<ITable>();
            var client = mocks.Create<IAmazonDynamoDB>();
            
            subject = new SampleRepository(client.Object, table.Object);
            angelina = new Foo("Angelina", "Jolie");
        }

        [Fact]
        public async Task PutItemAsync()
        {
            table.Setup(x => x.PutItemAsync(It.Is<Document>(a => Matches(a, angelina))))
                 .Returns(Task.CompletedTask);

            await subject.SaveAsync(angelina);
        }

        [Fact]
        public async Task DeleteItemAsync()
        {
            table.Setup(x => x.DeleteItemAsync(It.Is<Document>(a => Matches(a, angelina))))
                 .Returns(Task.CompletedTask);

            await subject.DeleteAsync(angelina);
        }

        private bool Matches(Document document, Foo expected)
        {
            Validate.Begin()
                .IsNotNull(document, "document")
                .IsNotNull(expected, "expected").Check()
                .IsEqual(document.Keys.Count, 3, "has 3 keys")
                .Check()
                .IsEqual(document[nameof(Foo.FirstName)].AsString(), expected.FirstName, "firstName")
                .IsEqual(document[nameof(Foo.LastName)].AsString(), expected.LastName, "lastName")
                .IsEqual(document[nameof(Foo.Age)].AsInt(), expected.Age, "age")
                .Check();
            return true;
        }

        public void Dispose()
        {
            mocks.VerifyAll();
        }
    }
}