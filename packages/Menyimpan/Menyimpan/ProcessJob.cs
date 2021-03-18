// -----------------------------------------------------------------------
// <copyright file="ProcessJob.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
// -----------------------------------------------------------------------

namespace Menyimpan
{
    using Amazon.S3;
    using Eunomia;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// A ProcessJob class.
    /// </summary>
    public record ProcessJob
    {
        /// <summary>
        /// Gets the S3 client.
        /// </summary>
        public IAmazonS3 S3 { get; init; }

        /// <summary>
        /// Gets the bucket name.
        /// </summary>
        public string SourceBucketName { get; init; }

        /// <summary>
        /// Gets the S3 key.
        /// </summary>
        public string SourceKey { get; init; }

        /// <summary>
        /// Gets the destination bucket.
        /// </summary>
        public string DestinationBucket { get; init; }

        /// <summary>
        /// Gets the destination prefix.
        /// </summary>
        public string DestinationPrefix { get; init; }

        /// <summary>
        /// Gets the dataset id.
        /// </summary>
        public DatasetId DatasetId { get; init; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger<JobProcessor> Logger { get; init; }
    }
}