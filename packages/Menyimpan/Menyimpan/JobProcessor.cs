// -----------------------------------------------------------------------
// <copyright file="JobProcessor.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
// -----------------------------------------------------------------------

namespace Menyimpan
{
    using System.IO;
    using System.Threading.Tasks;
    using Aletheia.Pgn;
    using Amazon.S3.Transfer;
    using Eunomia;
    using Menyimpan.Chess;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// A JobProcessor class.
    /// </summary>
    public class JobProcessor
    {
        private static readonly IGameFlattener Flattener = new RudzFlattener();

        private readonly ILogger<JobProcessor> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobProcessor"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public JobProcessor(ILogger<JobProcessor> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Processes the job.
        /// </summary>
        /// <param name="job">The job to process.</param>
        /// <returns>An awaitable task.</returns>
        public async Task ProcessAsync(ProcessJob job)
        {
            var util = new TransferUtility(job.S3);
            string tmpPath = Path.GetTempFileName();
            await util.DownloadAsync(new TransferUtilityDownloadRequest()
            {
                BucketName = job.SourceBucketName,
                Key = job.SourceKey,
                FilePath = tmpPath,
            });
            using var stream = File.OpenRead(tmpPath);
            using var pgnGameStream = new PgnGameStream(stream);
            while (!pgnGameStream.EndOfStream)
            {
                var nextGame = pgnGameStream.ParseNextGame();
                var (game, rows) = Flattener.FlattenPgnGame(nextGame, job.DatasetId);
            }
        }
    }
}