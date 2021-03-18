// -----------------------------------------------------------------------
// <copyright file="PgnIdUtility.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
// -----------------------------------------------------------------------

namespace Menyimpan.Chess
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using Aletheia.Pgn.Model;
    using Eunomia;
    using HashidsNet;

    /// <summary>
    /// A static class to generate a unique ID for a PGN game.
    /// </summary>
    public static class PgnIdUtility
    {
        /// <summary>
        /// The seven required tags in a PGN game.
        /// </summary>
        internal static readonly string[] SevenTagRoster = new[]
        {
            Event, Site, Date, Round, White, Black, Result,
        };

        private const string Event = nameof(Event);
        private const string Site = nameof(Site);
        private const string Date = nameof(Date);
        private const string Round = nameof(Round);
        private const string White = nameof(White);
        private const string Black = nameof(Black);
        private const string Result = nameof(Result);

        /// <summary>
        /// Generate a unique ID for a pgn game.
        /// </summary>
        /// <param name="pgnGame">The game to generate the ID for.</param>
        /// <returns>The unique id.</returns>
        public static GameId GenerateGameId(PgnGame pgnGame)
        {
            var tagString = CanonicalizeTags(pgnGame);
            var mainLineString = CanonicalizeMainLine(pgnGame);
            var ids = Sha256AsIntArray(tagString + mainLineString);

            var hashIds = new Hashids();
            var uniqueId = Sha256AsHex(pgnGame.OriginalPgnText);
            var canonicalizedId = Sha256AsHex(tagString + mainLineString);
            var publicId = hashIds.Encode(ids);

            return new GameId()
            {
                UniqueId = uniqueId,
                CanonicalizedId = canonicalizedId,
                PublicId = publicId,
            };
        }

        /// <summary>
        /// Return a canonicalized string with the values of the seven tags.
        /// This string is formed by putting each tag of the seven-tag-roster
        /// on a line with the format:
        /// <code>Name=url-encoded-value</code>
        /// The string will end in a trailing newline.
        /// </summary>
        /// <param name="pgnGame">The PGN game to create the canonicalized stream for.</param>
        /// <returns>The canonicalized tag string.</returns>
        internal static string CanonicalizeTags(PgnGame pgnGame)
        {
            var builder = new StringBuilder();
            foreach (var tagName in SevenTagRoster)
            {
                var tagValue = pgnGame.AllTags[tagName].Value;
                builder.AppendLine($"{tagName}={HttpUtility.UrlEncode(tagValue)}");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Return a canonicalized string with each main-line move added,
        /// with any symbols removed (no checks, no mates). Each ply is
        /// added on its own line, with a trailing newline character.
        /// </summary>
        /// <param name="pgnGame">The PGN game to create the canonicalized stream for.</param>
        /// <returns>The canonicalized tag string.</returns>
        internal static string CanonicalizeMainLine(PgnGame pgnGame)
        {
            var builder = new StringBuilder();
            foreach (var ply in pgnGame.MainLineAsList)
            {
                string plyText = ply.San;
                plyText = plyText
                    .Replace("+", string.Empty)
                    .Replace("#", string.Empty);
                builder.AppendLine(plyText);
            }

            return builder.ToString();
        }

        private static string Sha256AsHex(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using (var hash = SHA256.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 64, because 512 bits / 8 bits in byte * 2 symbols for byte
                var hashedInputStringBuilder = new StringBuilder(64);
                foreach (var b in hashedInputBytes)
                {
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                }

                return hashedInputStringBuilder.ToString();
            }
        }

        private static int[] Sha256AsIntArray(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using (var hash = SHA256.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                var ints = new List<int>();
                for (int i = 0; i < hashedInputBytes.Length; i += 4)
                {
                    var byteArray = new[]
                    {
                        hashedInputBytes[i],
                        hashedInputBytes[i + 1],
                        hashedInputBytes[i + 2],
                        hashedInputBytes[i + 3],
                    };
                    ints.Add(BitConverter.ToInt32(byteArray));
                }

                return ints.ToArray();
            }
        }
    }
}