// -----------------------------------------------------------------------
// <copyright file="IGameFlattener.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
// -----------------------------------------------------------------------

namespace Menyimpan.Chess
{
    using System.Collections.Generic;
    using Aletheia.Pgn.Model;
    using Eunomia;

    /// <summary>
    /// An interface defining the required methods to be a game flattener.
    /// </summary>
    public interface IGameFlattener
    {
        /// <summary>
        /// Flattens a parsed <see cref="FlattenPgnGame"/> into a game and list of positions.
        /// </summary>
        /// <param name="pgnGame">The parsed PGN game.</param>
        /// <param name="datasetId">The dataset id.</param>
        /// <returns>A tuple containing the game and the list of flattened positions.</returns>
        (IndexedGame Game, List<GamePositionRow> Positions) FlattenPgnGame(
            PgnGame pgnGame,
            DatasetId datasetId);
    }
}