using System;
using System.Collections.Generic;

namespace Rubickanov.Opal.Domain
{
    public static class CardFactory
    {
        public static List<Card> CreateShuffledDeck(int rows, int columns)
        {
            int totalCards = rows * columns;

            if (totalCards % 2 != 0)
            {
                throw new ArgumentException("Total number of cards must be even (rows * columns)");
            }

            int pairCount = totalCards / 2;
            var cards = new List<Card>(totalCards);

            for (int i = 0; i < pairCount; i++)
            {
                cards.Add(new Card(i * 2, i));
                cards.Add(new Card(i * 2 + 1, i));
            }

            Shuffle(cards);
            return cards;
        }

        public static List<Card> FromSnapshots(List<CardSnapshot> snapshots)
        {
            var cards = new List<Card>(snapshots.Count);

            foreach (var snapshot in snapshots)
            {
                cards.Add(new Card(snapshot.Id, snapshot.Value, snapshot.State));
            }

            return cards;
        }

        private static void Shuffle(List<Card> cards)
        {
            var random = new Random();

            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (cards[i], cards[j]) = (cards[j], cards[i]);
            }
        }
    }
}
