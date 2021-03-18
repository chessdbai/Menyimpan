// -----------------------------------------------------------------------
// <copyright file="RudzFlattener.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
// -----------------------------------------------------------------------

namespace Menyimpan.Chess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aletheia.Pgn.Model;
    using ChessBot.Logic.Eco;
    using Eunomia;
    using Rudz.Chess;
    using Rudz.Chess.Enums;
    using Rudz.Chess.Factories;
    using Rudz.Chess.Types;

    /// <summary>
    /// A flattener that uses the Rudz ChessLib library to flatten games.
    /// </summary>
    public class RudzFlattener : IGameFlattener
    {
        /// <summary>
        /// Flatten a game into its indexed form and a list of the positions.
        /// </summary>
        /// <param name="pgnGame">The parsed PGN game object to flatten.</param>
        /// <param name="datasetId">The dataset for this import job.</param>
        /// <returns>A 2-tuple with the indexed game and a list of positions.</returns>
        public (IndexedGame Game, List<GamePositionRow> Positions) FlattenPgnGame(
            PgnGame pgnGame,
            DatasetId datasetId)
        {
            var indexedGame = this.CreateGameFromPgnGame(pgnGame, datasetId);

            var board = new Board();
            var pieceValue = new PieceValue();
            var pos = new Position(board, pieceValue);
            var game = GameFactory.Create(pos);
            if (pgnGame.AllTags.ContainsKey("FEN"))
            {
                var fen = pgnGame.AllTags["FEN"].Value;
                game.NewGame(fen);
            }
            else
            {
                game.NewGame();
            }

            var positions = new List<GamePositionRow>();
            var pgnPlies = pgnGame.MainLineAsList;

            int plyCount = 0;
            GamePly prevPly = null;
            var pliesSoFar = new List<GamePly>();
            string gameId = Guid.NewGuid().ToString();
            OpeningPosition openingPosition = null;

            foreach (var ply in pgnPlies)
            {
                var position = new GamePositionRow();
                position.GameUniqueId = gameId;
                position.GamePlyNumber = plyCount;
                position.GameMoveNumber = (plyCount / 2) + 1;

                var fen = game.GetFen().ToString();
                var fenParts = fen.Split(' ');

                if (!ply!.SanIsNullMove)
                {
                    Rudz.Chess.Types.Move move;
                    try
                    {
                        move = this.PlyToChessMove(game, ply);
                    }
                    catch (FlattenNoMatchException e)
                    {
                        string posFen = game.Pos.GenerateFen().ToString();
                        Console.WriteLine(e);
                        move = this.PlyToChessMove(game, ply);
                        throw;
                    }

                    var movePrinter = new MoveAmbiguity(game.Pos);
                    position.NextMoveSan = ply!.San;
                    position.NextMoveUci = movePrinter.ToNotation(move, MoveNotations.Uci);

                    var state = new State();
                    game.Pos.MakeMove(move, state);
                }
                else
                {
                    game.Pos.MakeNullMove(new State());
                    position.NextMoveSan = ply!.San;
                    position.NextMoveUci = "0000";
                }

                position.PreviousMoveSan = prevPly?.San;
                position.PreviousMoveUci = positions.Count > 0 ? positions.Last().NextMoveUci : "0000";

                position.FenPosition = fen;

                var openingFen = string.Join(" ", fenParts.Take(3));
                openingPosition = OpeningPositions.GetOpeningOrNull(openingFen) ?? openingPosition;
                position.EcoCode = openingPosition?.ECO;

                pliesSoFar.Add(ply);
                plyCount++;
                prevPly = ply;
                positions.Add(position);
            }

            return (indexedGame, positions);
        }

        private Move PlyToChessMove(Rudz.Chess.IGame game, GamePly ply)
        {
            var movePrinter = new MoveAmbiguity(game.Pos);
            var moves = game.Pos.GenerateMoves()
                .Select(m => m.Move)
                .ToList();

            string san = ply.San;
            var promoParts = san.Split('=');
            List<Move> filteredMoves;
            if (promoParts.Length == 2)
            {
                var nonPromoPart = promoParts[0];
                var promoPieceChar = promoParts[1].Trim('=', '#').ToLower()[0];
                PieceTypes? pieceTypes = null;
                switch (promoPieceChar)
                {
                    case 'n':
                        pieceTypes = PieceTypes.Knight;
                        break;
                    case 'b':
                        pieceTypes = PieceTypes.Bishop;
                        break;
                    case 'r':
                        pieceTypes = PieceTypes.Rook;
                        break;
                    case 'q':
                        pieceTypes = PieceTypes.Queen;
                        break;
                }

                // This is a promotion
                if (pieceTypes == null)
                {
                    throw new ArgumentException($"Unknown promotion piece type '{promoPieceChar}'.");
                }

                filteredMoves = moves.Where(
                    m =>
                        nonPromoPart.Contains(m.GetToSquare().ToString()) &&
                        nonPromoPart.Contains(m.GetFromSquare().FileChar) &&
                        m.IsPromotionMove() &&
                        m.GetPromotedPieceType() == pieceTypes)
                    .ToList();
            }
            else
            {
                var moveText = promoParts[0].TrimEnd('#', '+');
                if (moveText == "O-O-O" || moveText == "O-O")
                {
                    filteredMoves = moves.Where(
                            m =>
                                m.ToString() == moveText.Replace('O', '0'))
                        .ToList();
                }
                else
                {
                    moveText = moveText.Replace("x", string.Empty);
                    var firstChar = moveText[0];
                    IEnumerable<Move> tempFilter = moves;
                    PieceTypes? movingPiece = null;
                    if (char.IsUpper(firstChar))
                    {
                        moveText = moveText.Remove(0, 1);
                        switch (firstChar)
                        {
                            case 'N':
                                movingPiece = PieceTypes.Knight;
                                break;
                            case 'Q':
                                movingPiece = PieceTypes.Queen;
                                break;
                            case 'K':
                                movingPiece = PieceTypes.King;
                                break;
                            case 'B':
                                movingPiece = PieceTypes.Bishop;
                                break;
                            case 'R':
                                movingPiece = PieceTypes.Rook;
                                break;
                            default:
                                throw new ArgumentException($"Unknown moving piece type '{firstChar}'.");
                        }
                    }
                    else
                    {
                        movingPiece = PieceTypes.Pawn;
                    }

                    tempFilter = tempFilter.Where(m =>
                        game.Pos.GetPiece(m.GetFromSquare()).Type() == movingPiece);

                    var destSquare = moveText.Substring(moveText.Length - 2, 2);
                    moveText = moveText.Replace(destSquare, string.Empty);
                    tempFilter = tempFilter.Where(m =>
                        m.GetToSquare().ToString() == destSquare);

                    if (moveText.Length == 2)
                    {
                        tempFilter = tempFilter.Where(m =>
                            m.GetFromSquare().ToString() == moveText);
                    }
                    else if (moveText.Length == 1)
                    {
                        char l = moveText[0];
                        if (char.IsNumber(l))
                        {
                            tempFilter = tempFilter.Where(m =>
                                m.GetFromSquare().Rank.Char == l);
                        }
                        else
                        {
                            tempFilter = tempFilter.Where(m =>
                                m.GetFromSquare().FileChar == l);
                        }
                    }

                    filteredMoves = tempFilter.ToList();
                }
            }

            if (filteredMoves.Count == 0)
            {
                var ex = new FlattenNoMatchException($"No moves found that match san '{ply.San}'.");
                ex.Fen = game.Pos.GenerateFen().ToString();
                ex.SanToMatch = ply.San;
                ex.GeneratedMoves = moves.Select(m => movePrinter.ToNotation(m, MoveNotations.San).TrimEnd('#', '+')).ToList();
                throw ex;
            }
            else if (filteredMoves.Count > 1)
            {
                throw new FlattenAmbiguityException($"Multiple moves found that match san '{ply.San}'.");
            }

            return filteredMoves[0];
        }

        private IndexedGame CreateGameFromPgnGame(PgnGame game, DatasetId datasetId)
        {
            var indexedGame = new IndexedGame()
            {
                White = game.WhitePlayer.Name,
                Black = game.BlackPlayer.Name,
                Site = game.AllTags["Site"].Value,
                Round = game.AllTags["Round"].Value,
                Event = game.AllTags["Event"].Value,
                Date = game.AllTags["Date"].AsDateValue() ?? DateTime.UnixEpoch,
                Result = game.AllTags["Round"].Value,
                GamePgn = game.OriginalPgnText,
            };

            var otherTags = new Dictionary<string, string>();
            foreach (var kvp in game.AllTags)
            {
                if (!PgnIdUtility.SevenTagRoster.Contains(kvp.Key))
                {
                    otherTags.Add(kvp.Key, kvp.Value.Value);
                }
            }

            indexedGame.OtherTags = otherTags;

            indexedGame.Id = PgnIdUtility.GenerateGameId(game);
            indexedGame.Dataset = datasetId;
            indexedGame.ImportTimestamp = DateTime.UtcNow;

            return indexedGame;
        }
    }
}