using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using Serilog;
using Xerris.AWS.Services.Services;
using Xerris.DotNet.Core;
using Xerris.DotNet.Core.Aws.Api;
using Xerris.DotNet.Core.Validations;

namespace Xerris.AWS.Hello.Handlers
{
    public class HelloHandler : BaseHandler
    {
        private readonly IHelloService service;

        public HelloHandler() : this(IoC.Resolve<IHelloService>())
        {
        }
        
        public HelloHandler(IHelloService service) 
        {
            this.service = service;
        }

        [LambdaSerializer(typeof(JsonSerializer))]
        public async Task<APIGatewayProxyResponse> GetHello(APIGatewayProxyRequest request, ILambdaContext context)
        {
            if (request.IsKeepWarm()) return this.Warmed();
            return "ok".Ok();
        }

        [LambdaSerializer(typeof(JsonSerializer))]
        public async Task<APIGatewayProxyResponse> SaveHello(APIGatewayProxyRequest request, ILambdaContext context)
        {
            Log.Debug("Calling GET /SaveHello");
            if (request.IsKeepWarm()) this.Warmed();

            return "ok".Ok();
        }

        private static async Task<APIGatewayProxyResponse> ExecuteAsync(Func<Task<APIGatewayProxyResponse>> func,
            [CallerMemberName] string callingMethod = null)
        {
            try
            {
                return await func();
            }
            catch (ValidationException ve)
            {
                return ve.Message.BadRequest();
            }
            catch (Exception e)
            {
                Log.Error(e,$"Unexpected error encountered in {callingMethod}", callingMethod);
                return e.Message.Error();
            }
        }
    }
}