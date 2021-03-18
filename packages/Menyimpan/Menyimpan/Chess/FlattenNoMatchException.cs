// -----------------------------------------------------------------------
// <copyright file="FlattenNoMatchException.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
// -----------------------------------------------------------------------

namespace Menyimpan.Chess
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// An exception thrown when the SAN move from the game
    /// cannot be matched with any legal move in the current position.
    /// </summary>
    [Serializable]
    public class FlattenNoMatchException : FlattenException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlattenNoMatchException"/> class.
        /// </summary>
        public FlattenNoMatchException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlattenNoMatchException"/> class.
        /// </summary>
        /// <param name="message">The custom error message.</param>
        public FlattenNoMatchException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlattenNoMatchException"/> class.
        /// </summary>
        /// <param name="message">The custom error message.</param>
        /// <param name="inner">The custom inner exception.</param>
        public FlattenNoMatchException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlattenNoMatchException"/> class.
        /// </summary>
        /// <param name="info">Information about the serialization attempt.</param>
        /// <param name="context">Context about the data stream.</param>
        protected FlattenNoMatchException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}