using System.Collections.Generic;

namespace Rubickanov.Opal.Domain
{
    public class Game
    {
        public IReadOnlyList<Card> Cards => _cards;
        public int Rows => _rows;
        public int Columns => _columns;
        public int Moves => _moves;
        public int Score => _score;
        public bool IsFinished => _matchedPairs * 2 == _cards.Count;

        private readonly List<Card> _cards;
        private readonly int _rows;
        private readonly int _columns;

        private int _moves;
        private int _score;
        private int _matchedPairs;
        private int _consecutiveMatches;

        private Card? _firstRevealed;

        public Game(int rows, int columns)
        {
            _rows = rows;
            _columns = columns;
            _cards = CardFactory.CreateShuffledDeck(rows, columns);
        }

        private Game(int rows, int columns, List<Card> cards, int moves, int score, int matchedPairs, int? firstRevealedCardId)
        {
            _rows = rows;
            _columns = columns;
            _cards = cards;
            _moves = moves;
            _score = score;
            _matchedPairs = matchedPairs;

            if (firstRevealedCardId.HasValue)
            {
                _firstRevealed = _cards.Find(c => c.Id == firstRevealedCardId.Value);
            }
        }

        public RevealResult RevealCard(Card card)
        {
            if (IsFinished)
            {
                return RevealResult.GameFinished;
            }

            if (card.State != CardState.Hidden)
            {
                return RevealResult.InvalidCard;
            }

            HidePendingCards();

            card.Reveal();

            if (_firstRevealed == null)
            {
                _firstRevealed = card;
                return RevealResult.FirstCard;
            }

            var first = _firstRevealed;
            _firstRevealed = null;
            _moves++;

            if (first.Value == card.Value)
            {
                first.Match();
                card.Match();
                _matchedPairs++;
                _consecutiveMatches++;
                _score += CalculateMatchScore();

                return IsFinished ? RevealResult.MatchAndFinish : RevealResult.Match;
            }

            first.MarkPendingHide();
            card.MarkPendingHide();
            _consecutiveMatches = 0;

            return RevealResult.NoMatch;
        }

        private void HidePendingCards()
        {
            foreach (var card in _cards)
            {
                if (card.State == CardState.PendingHide)
                {
                    card.Hide();
                }
            }
        }

        private int CalculateMatchScore()
        {
            const int baseScore = 100;
            int comboBonus = (_consecutiveMatches - 1) * 25;
            return baseScore + comboBonus;
        }

        public GameSnapshot CreateSnapshot()
        {
            var cardSnapshots = new List<CardSnapshot>(_cards.Count);

            foreach (var card in _cards)
            {
                cardSnapshots.Add(new CardSnapshot(card.Id, card.Value, card.State));
            }

            return new GameSnapshot(
                _rows,
                _columns,
                cardSnapshots,
                _moves,
                _score,
                _matchedPairs,
                _firstRevealed?.Id
            );
        }

        public static Game FromSnapshot(GameSnapshot snapshot)
        {
            var cards = CardFactory.FromSnapshots(snapshot.Cards);

            return new Game(
                snapshot.Rows,
                snapshot.Columns,
                cards,
                snapshot.Moves,
                snapshot.Score,
                snapshot.MatchedPairs,
                snapshot.FirstRevealedCardId
            );
        }
    }
}
