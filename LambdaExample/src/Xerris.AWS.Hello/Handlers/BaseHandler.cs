using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Serilog;
using Xerris.DotNet.Core.Aws.Api;
using Xerris.DotNet.Core.Serialization;
using Xerris.DotNet.Core.Validations;

namespace Xerris.AWS.Hello.Handlers
{
    public abstract class BaseHandler
    {
        public static readonly JsonSerializerSettings LongNameSerializerSettings = 
            new JsonSerializerSettings
            {
                ContractResolver = new LongNameContractResolver()
            };

 

        protected static string GetIdentity(APIGatewayProxyRequest request)
        {
            Log.Debug("Checking Identity...");
            var identity = request.GetAuthorization();
            Validate.Begin()
                .IsNotNull(identity, "identity is null. Ensure the Authorization header is set.").Check()
                .IsNotEmpty(identity, "identity is null").Check();
            return identity;
        }

        protected static Validation ValidateIsNumeric(string temporaryPriceId)
        {            
            return Validate.Begin()
                .IsNumeric(temporaryPriceId, "should be numeric")
                .ContinueIfValid(v => v.GreaterThan(int.Parse(temporaryPriceId), 0, "greater than 0"));
        }
    }
}