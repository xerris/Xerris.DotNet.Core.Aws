#!/usr/bin/env node
import 'source-map-support/register';
import cdk = require('@aws-cdk/core');
import { InfrastructureStack } from './infrastructure-stack';

const app = new cdk.App();
new InfrastructureStack(app, 'Xerris-Lambda-Api-Dev');
