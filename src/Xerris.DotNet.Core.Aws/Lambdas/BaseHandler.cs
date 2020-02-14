using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Serilog;
using Xerris.DotNet.Core.Aws.Api;
using Xerris.DotNet.Core.Validations;

namespace Xerris.DotNet.Core.Aws.Lambdas
{

    public abstract class BaseHandler
    {
        protected static async Task<APIGatewayProxyResponse> SendAsync<T,TEx>(Func<Task<T>> action, [CallerMemberName]string path=null, Action<TEx> exceptionHandler = null)
            where TEx : Exception
        {
            try
            {
                var response = await action();
                return response.Ok();
            }
            catch (ValidationException e)
            {
                Log.Error(e, $"Validation error in {path}");
                return e.Message.BadRequest();
            }
            catch (TEx ex)
            {
                Log.Error(ex, $"Unexpected error encountered {path}");
                exceptionHandler?.Invoke(ex);
                return ex.Message.Error();
            }
            catch (Exception e)
            {
                Log.Error(e, $"Unexpected error encountered {path}");
                return e.Message.Error();
            }
        }
        
        protected static async Task<APIGatewayProxyResponse> SendAsync<T>(Func<Task<T>> action, [CallerMemberName]string path=null)
        {
            try
            {
                var response = await action();
                return response.Ok();
            }
            catch (ValidationException e)
            {
                Log.Error(e, $"Validation error in {path}");
                return e.Message.BadRequest();
            }
            catch (Exception e)
            {
                Log.Error(e, $"Unexpected error encountered {path}");
                return e.Message.Error();
            }
        }
        
        protected static string GetIdentity(APIGatewayProxyRequest request)
        {
            Log.Debug("Checking Identity...");
            var identity = request.GetAuthorization();
            Validate.Begin()
                .IsNotNull(identity, "identity is null. Ensure the Authorization header is set.").Check()
                .IsNotEmpty(identity, "identity is null").Check();
            return identity;
        }
    }
}