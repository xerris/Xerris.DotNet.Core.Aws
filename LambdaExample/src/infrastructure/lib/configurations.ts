import {Environment} from "@aws-cdk/core";

export interface Configuration {
    readonly stageName : string,
    readonly vpcId: string;
    readonly securityGroupId: string;
    readonly providerArn: string;
    readonly env?: Environment;
}