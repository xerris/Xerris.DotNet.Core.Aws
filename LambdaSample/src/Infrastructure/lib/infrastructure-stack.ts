import cdk = require('@aws-cdk/core');
import lambda = require('@aws-cdk/aws-lambda');
import apigateway = require('@aws-cdk/aws-apigateway');
import iam = require("@aws-cdk/aws-iam");
import * as path from "path";

export class InfrastructureStack extends cdk.Stack {
    zipFileLocation : string;
    
  constructor(scope: cdk.Construct, id: string, props?: cdk.StackProps) {
    super(scope, id, props);

      this.zipFileLocation = path.join(__dirname, '../../../dist/LambdaPackage.zip'); 

      const getBooks = this.createLambda('getBooks', 
          'Xerris.Lambda.Api::Xerris.Lambda.Api.Handlers.BookHandler::GetBooks');
      
      const api = new apigateway.RestApi(this, 'xerris-api-sample', {
          restApiName: 'xerris-api-sample',
          deployOptions: {
              stageName: 'dev'
          }
      });

      const libraryResource = api.root.addResource('library');

      const postIntegration = new apigateway.LambdaIntegration(getBooks);

      libraryResource.addMethod('GET', postIntegration);
      addCorsOptions(libraryResource);
  }

    private createLambda(id : string, handler: string) {
        return new lambda.Function(this, id, {
            runtime: lambda.Runtime.DOTNET_CORE_2_1,
            handler: handler,
            code: lambda.Code.fromAsset(this.zipFileLocation),
            timeout: cdk.Duration.seconds(20)
        });
    }
  
}

export function addCorsOptions(apiResource: apigateway.IResource) {
    apiResource.addMethod('OPTIONS', new apigateway.MockIntegration({
        integrationResponses: [{
            statusCode: '200',
            responseParameters: {
                'method.response.header.Access-Control-Allow-Headers': "'Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token,X-Amz-User-Agent'",
                'method.response.header.Access-Control-Allow-Origin': "'*'",
                'method.response.header.Access-Control-Allow-Credentials': "'false'",
                'method.response.header.Access-Control-Allow-Methods': "'OPTIONS,GET,PUT,POST,DELETE'",
            },
        }],
        passthroughBehavior: apigateway.PassthroughBehavior.NEVER,
        requestTemplates: {
            "application/json": "{\"statusCode\": 200}"
        },
    }), {
        methodResponses: [{
            statusCode: '200',
            responseParameters: {
                'method.response.header.Access-Control-Allow-Headers': true,
                'method.response.header.Access-Control-Allow-Methods': true,
                'method.response.header.Access-Control-Allow-Credentials': true,
                'method.response.header.Access-Control-Allow-Origin': true,
            },
        }]
    })
}

