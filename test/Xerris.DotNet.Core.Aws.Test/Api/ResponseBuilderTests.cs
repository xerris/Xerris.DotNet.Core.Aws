using System.Net;
using Xerris.DotNet.Core.Aws.Api;
using Xerris.DotNet.Core.Aws.Test.Model;
using Xerris.DotNet.Core.Extensions;
using Xerris.DotNet.Core.Validations;
using Xunit;

namespace Xerris.DotNet.Core.Aws.Test.Api
{
    public class ResponseBuilderTests
    {
        private readonly Foo subject;

        public ResponseBuilderTests()
        {
            subject = new Foo {FirstName = "First", LastName = "Last"};
        }
        
        [Fact]
        public void Ok_Dto()
        {
            var response = subject.Ok();
            
            Validate.Begin()
                .IsNotNull(response, "response")
                .Check()
                .IsEqual(response.Body, subject.ToJson(), "foo as Json")
                .IsEqual(response.StatusCode, (int) HttpStatusCode.OK, "ok")
                .Check();
        }
        
        [Fact]
        public void Ok()
        {
            var response = "ok".Ok();
            
            Validate.Begin()
                .IsNotNull(response, "response")
                .Check()
                .IsEqual(response.Body, "ok", "ok as Json")
                .IsEqual(response.StatusCode, (int) HttpStatusCode.OK, "ok")
                .Check();
        }

        [Fact]
        public void Created()
        {
            var response = subject.Created();
            
            Validate.Begin()
                .IsNotNull(response, "response")
                .Check()
                .IsEqual(response.Body, subject.ToJson(), "foo as Json")
                .IsEqual(response.StatusCode, (int) HttpStatusCode.Created, "ok")
                .Check();
        }

        [Fact]
        public void Accepted()
        {
            var response = subject.Accepted();
            
            Validate.Begin()
                .IsNotNull(response, "response")
                .Check()
                .IsEqual(response.Body, subject.ToJson(), "foo as Json")
                .IsEqual(response.StatusCode, (int) HttpStatusCode.Accepted, "ok")
                .Check();
        }

        [Fact]
        public void Error_String()
        {
            var response = "error".Error();
            
            Validate.Begin()
                .IsNotNull(response, "response")
                .Check()
                .IsEqual(response.Body, "error", "error as Json")
                .IsEqual(response.StatusCode, (int) HttpStatusCode.InternalServerError, "ok")
                .Check();
        }

        [Fact]
        public void Error_Subject()
        {
            var response = subject.Error();
            
            Validate.Begin()
                .IsNotNull(response, "response")
                .Check()
                .IsEqual(response.Body, subject.ToJson(), "error as Json")
                .IsEqual(response.StatusCode, (int) HttpStatusCode.InternalServerError, "ok")
                .Check();
        }

        [Fact]
        public void BadRequest_Dto()
        {
            var response = subject.BadRequest();
            
            Validate.Begin()
                .IsNotNull(response, "response")
                .Check()
                .IsEqual(response.Body, subject.ToJson(), "foo as Json")
                .IsEqual(response.StatusCode, (int) HttpStatusCode.BadRequest, "ok")
                .Check();
        }

        [Fact]
        public void BadRequest()
        {
            var response = "badRequest".BadRequest();
            
            Validate.Begin()
                .IsNotNull(response, "response")
                .Check()
                .IsEqual(response.Body, "badRequest", "badrequest as Json")
                .IsEqual(response.StatusCode, (int) HttpStatusCode.BadRequest, "ok")
                .Check();
        }
        
        [Fact]
        public void Unauthorized()
        {
            var response = "ok".UnAuthorized();
            
            Validate.Begin()
                .IsNotNull(response, "response")
                .Check()
                .IsNotNull(response.Body, "body is null")
                .IsEqual(response.StatusCode, (int) HttpStatusCode.Unauthorized, "ok")
                .Check();
        }
    }
}