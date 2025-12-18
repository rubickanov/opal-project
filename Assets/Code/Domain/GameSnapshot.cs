using System;
using System.Collections.Generic;

namespace Rubickanov.Opal.Domain
{
    [Serializable]
    public class GameSnapshot
    {
        public int Rows;
        public int Columns;
        public List<CardSnapshot> Cards;
        public int Moves;
        public int Score;
        public int MatchedPairs;
        public int FirstRevealedCardId;
        public bool HasFirstRevealedCard;

        public GameSnapshot(
            int rows,
            int columns,
            List<CardSnapshot> cards,
            int moves,
            int score,
            int matchedPairs,
            int? firstRevealedCardId)
        {
            Rows = rows;
            Columns = columns;
            Cards = cards;
            Moves = moves;
            Score = score;
            MatchedPairs = matchedPairs;
            FirstRevealedCardId = firstRevealedCardId ?? -1;
            HasFirstRevealedCard = firstRevealedCardId.HasValue;
        }

        public int? GetFirstRevealedCardId()
        {
            return HasFirstRevealedCard ? FirstRevealedCardId : null;
        }
    }
}