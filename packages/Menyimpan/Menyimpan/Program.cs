// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
// -----------------------------------------------------------------------

namespace Menyimpan
{
    using System;
    using Amazon;
    using Amazon.Runtime;
    using Amazon.Runtime.CredentialManagement;
    using Amazon.S3;
    using Eunomia;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Entry point class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point method.
        /// </summary>
        /// <param name="bucket">The S3 bucket.</param>
        /// <param name="key">The S3 key.</param>
        /// <param name="dataset">The dataset name.</param>
        /// <param name="author">The dataset author.</param>
        /// <param name="awsProfile">The AWS credential profile name.</param>
        public static void Main(
            string bucket,
            string key,
            string dataset,
            string author,
            string awsProfile = null)
        {
            var loggerFactory = CreateLoggerFactory();
            (var creds, var region) = GetAWSConfig();
            var s3 = new AmazonS3Client(creds, region);

            var job = new ProcessJob()
            {
                SourceBucketName = bucket,
                SourceKey = key,
                DestinationBucket = Environment.GetEnvironmentVariable("CHESSDB_DST_BUCKET"),
                DestinationPrefix = Environment.GetEnvironmentVariable("CHESSDB_DST_PREFIX"),
                DatasetId = new DatasetId()
                {
                    Author = author,
                    Name = dataset,
                },
                S3 = s3,
            };
            var processor = new JobProcessor(loggerFactory.CreateLogger<JobProcessor>());
        }

        private static ILoggerFactory CreateLoggerFactory() =>
            LoggerFactory.Create(builder =>
            {
                builder.AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("Placeholder.Program", LogLevel.Debug)
                    .AddConsole();
            });

        private static (AWSCredentials Creds, RegionEndpoint Region) GetAWSConfig(string profileName = null)
        {
            if (!string.IsNullOrEmpty(profileName))
            {
                var chain = new CredentialProfileStoreChain();
                AWSCredentials creds;
                RegionEndpoint region;
                if (chain.TryGetProfile(profileName, out var profile))
                {
                    region = profile.Region;
                }
                else
                {
                    throw new ArgumentException($"No region profile with the name '{profileName}' was found.");
                }

                if (chain.TryGetAWSCredentials(profileName, out var credentials))
                {
                    creds = credentials;
                }
                else
                {
                    throw new ArgumentException($"No credential profile with credentials found with the name '{profileName}'.");
                }
            }

            return (FallbackCredentialsFactory.GetCredentials(), FallbackRegionFactory.GetRegionEndpoint());
        }
    }
}