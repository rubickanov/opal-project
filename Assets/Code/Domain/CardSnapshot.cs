namespace Rubickanov.Opal.Domain
{
    public class CardSnapshot
    {
        public readonly int Id;

        public readonly int Value;

        public readonly CardState State;
        public CardSnapshot(int id, int value, CardState state)
        {
            Id = id;
            Value = value;
            State = state;
        }
    }
}