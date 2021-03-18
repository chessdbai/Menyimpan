import * as cdk from '@aws-cdk/core';

import * as ecs from '@aws-cdk/aws-ecs';
import * as ec2 from '@aws-cdk/aws-ec2';
import { ZeusServiceAccount } from '@chessdb.biz/zeus-accounts';
import { BatchProcessor } from './batch-processor';
import * as path from 'path';

export class MenyimpanStack extends cdk.Stack {
  constructor(scope: cdk.Construct, id: string, account: ZeusServiceAccount) {
    super(scope, id, {
      env: account.environment
    });

    const vpc = ec2.Vpc.fromLookup(this, 'ImportedVpc', {
      vpcId: account.topology.vpcId
    });
    
    const dir = path.resolve(path.join(__dirname, '../../Menyimpan'));
    const image = ecs.ContainerImage.fromAsset(dir, {
      file: 'Dockerfile'
    });
    new BatchProcessor(this, 'BatchProcessor', {
      vpc: vpc,
      image: image
    });

    // The code that defines your stack goes here
  }
}
