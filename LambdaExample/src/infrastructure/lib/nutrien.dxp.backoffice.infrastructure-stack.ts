import cdk = require("@aws-cdk/core");
import lambda = require("@aws-cdk/aws-lambda");
import apigateway = require("@aws-cdk/aws-apigateway");
import iam = require("@aws-cdk/aws-iam");
import path = require("path");
import events = require('@aws-cdk/aws-events');
import targets = require('@aws-cdk/aws-events-targets');
import ec2 = require("@aws-cdk/aws-ec2");
import {Effect, PolicyStatement} from "@aws-cdk/aws-iam";
import {AuthorizationType, CfnAuthorizer, Cors} from "@aws-cdk/aws-apigateway";
import {Configuration} from "./configurations"
import {FilterPattern, LogGroup, RetentionDays} from "@aws-cdk/aws-logs";
import {RuleTargetInput} from "@aws-cdk/aws-events";

const corsOptions = {
    allowOrigins: Cors.ALL_ORIGINS,
    allowMethods: Cors.ALL_METHODS,
    allowCredentials: true,
    allowHeaders: Cors.DEFAULT_HEADERS,
    statusCode: 200
};

class HelloBaseStack extends cdk.Stack {

    protected zipFileLocation: string;
    protected config: Configuration;
    protected secretsPolicy : iam.PolicyStatement;
    protected keepWarmRule: events.Rule;
    protected keepWarmCount: number = 0;
    protected api :  apigateway.RestApi;
    protected name : string;
    
    constructor(scope: cdk.Construct, id: string, apiName: string, config : Configuration) {
        super(scope, id, {env: config.env});

        this.config = config;
        this.zipFileLocation = path.join(__dirname, "../../../dist/LambdaPackage.zip");
        this.secretsPolicy = this.createPolicyStatementForSecrets();
        this.api = this.createApiGateway(apiName);
    }
    
    protected createApiGateway(name : string) {
        const api = new apigateway.RestApi(this, name, {
            restApiName: name,
            deployOptions: {
                stageName: this.config.stageName,
            },
            defaultCorsPreflightOptions : corsOptions
        });
        return api;
    }

    protected createLambda(id: string, handler: string) {
        return new lambda.Function(this, id, {
            runtime: lambda.Runtime.DOTNET_CORE_2_1,
            handler: handler,
            functionName: `xerris-aws-${id}`,
            code: lambda.Code.fromAsset(this.zipFileLocation),
            timeout: cdk.Duration.seconds(60),
            environment: {stageName: this.config.stageName},
            memorySize: 1024
        });
    }
    
    protected createPolicyStatementForSecrets() {
        // see https://docs.aws.amazon.com/IAM/latest/UserGuide/list_awssecretsmanager.html
        const secretsPolicy = new iam.PolicyStatement({
            effect: iam.Effect.ALLOW
        });
        secretsPolicy.addActions("secretsmanager:GetSecretValue");
        secretsPolicy.addResources("*");
        return secretsPolicy;
    }

    protected buildKeepWarmInput(method: string, path : string) : any {
        return {
            "path": path,
            "httpMethod": method.toUpperCase(),
            "isBase64Encoded": false,
            "headers": {
                "x-keep-warm": true,
            }
        }
    }

    protected keepWarm(target : lambda.Function, method : string, path : string)
    {
        if(this.keepWarmCount % 5 == 0){
            const part = this.keepWarmCount / 5 + 1;
            this.keepWarmRule = new events.Rule(this, `${this.name}-warmer-part${part}`, {
                description: `Scheduled request to keep dxp-backoffice ${this.name} Lambdas from shutting down`,
                ruleName: `${this.name}-warmer-part${part}`,
                schedule: events.Schedule.rate(cdk.Duration.minutes(10))
            });
        }

        this.keepWarmRule.addTarget(new targets.LambdaFunction(target,
            {event: RuleTargetInput.fromObject(this.buildKeepWarmInput(method, path))}));

        this.keepWarmCount++;
    }
    
    protected integrate(method : string, apiResource : apigateway.IResource,
                      lambdaFunc : lambda.Function)    {

        const integration = new apigateway.LambdaIntegration(lambdaFunc);
        apiResource.addMethod(method, integration, {
            authorizationType: AuthorizationType.COGNITO,
        });
    }
}

export class XerrisHelloStack extends HelloBaseStack {
    
    constructor(scope: cdk.Construct, id: string, config : Configuration)  {
        super(scope, id, "xerris-hello", config);

        this.name = "hello";
        this.setUpHelloLambdas(this.api);
    } 

    private setUpHelloLambdas(api: apigateway.RestApi) {
        const productApi = api.root.addResource("hello");
        
        const getHelloLambda = this.createLambda("getHello",
            "Xerris.AWS.Hello::Xerris.AWS.Hello.Handlers.CustomerHandler::GetHello");
        this.integrate("GET", productApi.addResource("getHello"), getHelloLambda);
        getHelloLambda.addToRolePolicy(this.secretsPolicy);
        this.keepWarm(getHelloLambda, "GET", "/hello/getHello")
    }
}


