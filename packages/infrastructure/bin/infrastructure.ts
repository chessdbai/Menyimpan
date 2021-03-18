#!/usr/bin/env node
import 'source-map-support/register';
import * as cdk from '@aws-cdk/core';
import * as pipelines from '@aws-cdk/pipelines';
import * as iam from '@aws-cdk/aws-iam';
import * as codepipeline from '@aws-cdk/aws-codepipeline';
import * as codepipeline_actions from '@aws-cdk/aws-codepipeline-actions';
import { MenyimpanStack } from '../lib/menyimpan-stack';
import AccountManager, { ZeusServiceAccount, ZeusCorpAccount } from '@chessdb.biz/zeus-accounts';

const betaAccount = AccountManager.getAccounts({
  stages: [ 'Beta' ],
  tag: 'Service'
})[0] as ZeusServiceAccount;
const prodAccount = AccountManager.getAccounts({
  stages: [ 'Prod' ],
  tag: 'Service'
})[0] as ZeusServiceAccount;
const deployAccount = AccountManager.getAccounts({
  tag: 'Deployment'
})[0] as ZeusCorpAccount;

class Menyimpan extends cdk.Stage {
  constructor(scope: cdk.Construct, id: string, account: ZeusServiceAccount) {
    super(scope, id, {
      env: account.environment
    });
  
    new MenyimpanStack(this, 'Menyimpan', account);
  }
}

/**
 * Stack to hold the pipeline
 */
 class DeployStack extends cdk.Stack {
  constructor(scope: cdk.Construct, id: string, props?: cdk.StackProps) {
    super(scope, id, props);

    const sourceArtifact = new codepipeline.Artifact();
    const cloudAssemblyArtifact = new codepipeline.Artifact();

    const pipeline = new pipelines.CdkPipeline(this, 'Pipeline', {
      pipelineName: 'Menyimpan',
      cloudAssemblyArtifact,
      sourceAction: new codepipeline_actions.GitHubSourceAction({
        actionName: 'GitHub',
        output: sourceArtifact,
        oauthToken: cdk.SecretValue.secretsManager('corp/Deploy/GitHub'),
        // Replace these with your actual GitHub project name
        owner: 'chessdbai',
        repo: 'Menyimpan',
        branch: 'master', // default: 'master'
      }),

      synthAction: pipelines.SimpleSynthAction.standardNpmSynth({
        sourceArtifact,
        cloudAssemblyArtifact,
        buildCommand: 'npm run release',
        rolePolicyStatements: [
          new iam.PolicyStatement({
            actions: [
              "sts:GetServiceBearerToken",
              "codeartifact:GetPackageVersionReadme",
              "codeartifact:GetAuthorizationToken",
              "codeartifact:DescribeRepository",
              "codeartifact:ReadFromRepository",
              "codeartifact:GetRepositoryEndpoint",
              "codeartifact:DescribeDomain",
              "codeartifact:DescribePackageVersion",
              "codeartifact:GetPackageVersionAsset",
              "codeartifact:GetRepositoryPermissionsPolicy",
              "codeartifact:GetDomainPermissionsPolicy"
            ],
            resources: ['*']
          })
        ]
      }),
    });

    pipeline.addApplicationStage(new Menyimpan(this, 'Beta', betaAccount), {
      
    });
    pipeline.addApplicationStage(new Menyimpan(this, 'Prod', prodAccount));
  }
}

const app = new cdk.App();
new DeployStack(app, 'DeployStack', {
  env: {
    account: deployAccount.accountId,
    region: deployAccount.region
  }
});