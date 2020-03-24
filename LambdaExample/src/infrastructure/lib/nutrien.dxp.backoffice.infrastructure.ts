#!/usr/bin/env node
import "source-map-support/register";
import cdk = require("@aws-cdk/core");
import { NutrienDxpBackofficeInfrastructureStack, TemporaryPricingInfrastructureStack } from "./nutrien.dxp.backoffice.infrastructure-stack";
import {Configuration} from "./configurations"

const configurations : { [key: string]: Configuration } = {
    ["dev"] :  {
        stageName: "dev",
        env :  { account: process.env.CDK_DEFAULT_ACCOUNT, region: process.env.CDK_DEFAULT_REGION }
    },
    ["staging"] : {
        stageName: "staging",
        env :  { account: process.env.CDK_DEFAULT_ACCOUNT, region: process.env.CDK_DEFAULT_REGION }
    },
    ["prod"] : {
        stageName: "prod",
        env :  { account: process.env.CDK_DEFAULT_ACCOUNT, region: process.env.CDK_DEFAULT_REGION }
    }
}

const app = new cdk.App();
const buildEnvironment = (app.node.tryGetContext('env') || "dev").trim().toLowerCase();
new TemporaryPricingInfrastructureStack(app, "Nutrien-Dxp-Temporary-Pricing", configurations[buildEnvironment]);


