import * as cdk from '@aws-cdk/core';
import * as ec2 from '@aws-cdk/aws-ec2';
import * as batch from '@aws-cdk/aws-batch';
import * as ecs from '@aws-cdk/aws-ecs';

interface BatchProcessorProps {
  vpc: ec2.IVpc,
  image: ecs.ContainerImage
}

export class BatchProcessor extends cdk.Construct {

  readonly computeEnvironment : batch.IComputeEnvironment;
  readonly securityGroup : ec2.ISecurityGroup;
  readonly jobDefinition : batch.IJobDefinition;
  readonly jobQueue : batch.IJobQueue;

  constructor(parent: cdk.Construct, name: string, props: BatchProcessorProps) {
    super(parent, name);

    const computeSecurityGroup = new ec2.SecurityGroup(this, 'SecurityGroup', {
      allowAllOutbound: true,
      vpc: props.vpc
    });

    const computeEnv = new batch.ComputeEnvironment(this, 'ComputeEnv', {
      computeEnvironmentName: 'Placeholder',
      enabled: true,
      computeResources: {
        type: batch.ComputeResourceType.SPOT,
        vpc: props.vpc,
        vpcSubnets: {
          subnetType: ec2.SubnetType.PRIVATE
        },
        securityGroups: [ computeSecurityGroup ]
      }
    });

    const jobQueue = new batch.JobQueue(this, 'JobQueue', {
      jobQueueName: 'Placeholder',
      computeEnvironments: [ {
        computeEnvironment: computeEnv,
        order: 1
      } ]
    });

    const jobDefinition = new batch.JobDefinition(this, 'JobDef', {
      container: {
        image: props.image,
        
      }
    });

    this.securityGroup = computeSecurityGroup;
    this.computeEnvironment = this.computeEnvironment;
    this.jobQueue = jobQueue;
    this.jobDefinition = jobDefinition;
  }
}