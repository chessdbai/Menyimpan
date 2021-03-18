// -----------------------------------------------------------------------
// <copyright file="FlattenException.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
// -----------------------------------------------------------------------

namespace Menyimpan.Chess
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// A top-level exception for any errors occurring during
    /// the flattening process.
    /// </summary>
    [Serializable]
    public class FlattenException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlattenException"/> class.
        /// </summary>
        public FlattenException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlattenException"/> class.
        /// </summary>
        /// <param name="message">The custom error message.</param>
        public FlattenException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlattenException"/> class.
        /// </summary>
        /// <param name="message">The custom error message.</param>
        /// <param name="inner">The custom inner exception.</param>
        public FlattenException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlattenException"/> class.
        /// </summary>
        /// <param name="info">Information about the serialization attempt.</param>
        /// <param name="context">Context about the data stream.</param>
        protected FlattenException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets or sets the position where the exception occurred.
        /// </summary>
        public string Fen { get; set; }

        /// <summary>
        /// Gets or sets the SAN move from the PGN game.
        /// </summary>
        public string SanToMatch { get; set; }

        /// <summary>
        /// Gets or sets the SAN move from the PGN game.
        /// </summary>
        public List<string> GeneratedMoves { get; set; }
    }
}