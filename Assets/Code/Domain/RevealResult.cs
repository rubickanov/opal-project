using System.Collections.Generic;

namespace Rubickanov.Opal.Domain
{
    public enum RevealResult
    {
        InvalidCard,
        GameFinished,
        FirstCard,
        Match,
        NoMatch,
        MatchAndFinish
    }

    public struct RevealData
    {
        public readonly RevealResult Result;
        public readonly List<Card> ChangedCards;

        public RevealData(RevealResult result, List<Card> changedCards)
        {
            Result = result;
            ChangedCards = changedCards;
        }
    }
}
