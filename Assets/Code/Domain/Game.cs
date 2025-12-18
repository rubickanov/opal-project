using System;
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
        private Card? _pendingCard1;
        private Card? _pendingCard2;

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

            foreach (var card in _cards)
            {
                if (card.State == CardState.PendingHide)
                {
                    if (_pendingCard1 == null) _pendingCard1 = card;
                    else _pendingCard2 = card;
                }
            }
        }

        public RevealData RevealCard(Card card)
        {
            var changedCards = new List<Card>(4);

            if (IsFinished)
            {
                return new RevealData(RevealResult.GameFinished, changedCards);
            }

            if (card.State == CardState.PendingHide)
            {
                HideOtherPendingCards(card, changedCards);
                card.Reveal();
                changedCards.Add(card);
                _firstRevealed = card;
                return new RevealData(RevealResult.FirstCard, changedCards);
            }

            if (card.State != CardState.Hidden)
            {
                return new RevealData(RevealResult.InvalidCard, changedCards);
            }

            CollectAndHidePendingCards(changedCards);

            card.Reveal();
            changedCards.Add(card);

            if (_firstRevealed == null)
            {
                _firstRevealed = card;
                return new RevealData(RevealResult.FirstCard, changedCards);
            }

            var first = _firstRevealed;
            _firstRevealed = null;
            _moves++;

            if (first.Value == card.Value)
            {
                first.Match();
                card.Match();
                changedCards.Add(first);
                _matchedPairs++;
                _consecutiveMatches++;
                _score += CalculateMatchScore();

                var result = IsFinished ? RevealResult.MatchAndFinish : RevealResult.Match;
                return new RevealData(result, changedCards);
            }

            first.MarkPendingHide();
            card.MarkPendingHide();
            _pendingCard1 = first;
            _pendingCard2 = card;
            changedCards.Add(first);
            _consecutiveMatches = 0;

            return new RevealData(RevealResult.NoMatch, changedCards);
        }

        private void CollectAndHidePendingCards(List<Card> changedCards)
        {
            if (_pendingCard1 != null)
            {
                _pendingCard1.Hide();
                changedCards.Add(_pendingCard1);
                _pendingCard1 = null;
            }

            if (_pendingCard2 != null)
            {
                _pendingCard2.Hide();
                changedCards.Add(_pendingCard2);
                _pendingCard2 = null;
            }
        }

        private void HideOtherPendingCards(Card except, List<Card> changedCards)
        {
            if (_pendingCard1 != null && _pendingCard1 != except)
            {
                _pendingCard1.Hide();
                changedCards.Add(_pendingCard1);
            }

            if (_pendingCard2 != null && _pendingCard2 != except)
            {
                _pendingCard2.Hide();
                changedCards.Add(_pendingCard2);
            }

            _pendingCard1 = null;
            _pendingCard2 = null;
        }

        /// <summary>
        /// Calculates score for a successful match using the following formula:
        ///
        /// Score = BaseScore * ComboMultiplier * DifficultyMultiplier + PerfectBonus
        ///
        /// Where:
        /// - BaseScore = 100 points
        /// - ComboMultiplier = 1.5 ^ (consecutiveMatches - 1), rewards streaks exponentially
        /// - DifficultyMultiplier = totalPairs / 8, harder boards give more points
        /// - PerfectBonus = 1000 points if player matched all pairs without mistakes
        ///
        /// Examples (4x4 grid = 8 pairs):
        ///   1st match:        100 * 1.0  * 1.0 = 100
        ///   2nd consecutive:  100 * 1.5  * 1.0 = 150
        ///   3rd consecutive:  100 * 2.25 * 1.0 = 225
        ///   Perfect game:     adds +1000 bonus
        ///
        /// Examples (6x6 grid = 18 pairs):
        ///   1st match:        100 * 1.0 * 2.25 = 225
        /// </summary>
        private int CalculateMatchScore()
        {
            const int baseScore = 100;
            const double comboExponent = 1.5;
            const int perfectBonus = 1000;

            int totalPairs = _cards.Count / 2;

            double comboMultiplier = Math.Pow(comboExponent, _consecutiveMatches - 1);
            double difficultyMultiplier = totalPairs / 8.0;

            int score = (int)(baseScore * comboMultiplier * difficultyMultiplier);

            bool isPerfectGame = IsFinished && _moves == totalPairs;
            if (isPerfectGame)
            {
                score += perfectBonus;
            }

            return score;
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
