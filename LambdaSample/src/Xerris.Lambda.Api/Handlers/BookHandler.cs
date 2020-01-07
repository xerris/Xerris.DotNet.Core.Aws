using System.Collections.Generic;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Xerris.DotNet.Core.Aws.Api;
using Xerris.DotNet.Core.Extensions;
using Xerris.Lambda.Api.Request; //using System.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Xerris.Lambda.Api.Handlers
{
    public class BookHandler
    {
        public APIGatewayProxyResponse GetBooks(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var books = new List<Book>{
                new Book{
                    ISBN = "abc-123",
                    Title = "Slaughter House Five",
                    Author = "Kurt Vonnegut",
                    Pages = 200
                },
                new Book{
                    ISBN = "abc-456",
                    Title = "Bluebeard",
                    Author = "Kurt Vonnegut",
                    Pages = 150
                }
            };
            return books.Ok();
        }
    }
}
