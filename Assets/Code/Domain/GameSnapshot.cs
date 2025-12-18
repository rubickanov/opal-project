using System.Collections.Generic;
namespace Rubickanov.Opal.Domain
{
    public class GameSnapshot
    {
        public readonly int Rows;

        public readonly int Columns;

        public readonly List<CardSnapshot> Cards;

        public readonly int Moves;

        public readonly int Score;

        public readonly int MatchedPairs;

        public readonly int? FirstRevealedCardId;

        public GameSnapshot(
            int rows,
            int columns,
            List<CardSnapshot> cards,
            int moves,
            int score,
            int matchedPairs,
            int? firstRevealedCardId
        )
        {
            Rows = rows;
            Columns = columns;
            Cards = cards;
            Moves = moves;
            Score = score;
            MatchedPairs = matchedPairs;
            FirstRevealedCardId = firstRevealedCardId;

        }
    }
}