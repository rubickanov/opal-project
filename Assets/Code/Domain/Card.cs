namespace Rubickanov.Opal.Domain
{
    public class Card
    {
        public int Id { get; }
        public int Value { get; }
        public CardState State { get; private set; }

        public Card(int id, int value, CardState state = CardState.Hidden)
        {
            Id = id;
            Value = value;
            State = state;
        }

        public void Hide() => State = CardState.Hidden;
        public void Reveal() => State = CardState.Revealed;
        public void Match() => State = CardState.Matched;
        public void MarkPendingHide() => State = CardState.PendingHide;
    }

    public enum CardState
    {
        Hidden,
        Revealed,
        Matched,
        PendingHide
    }
}
