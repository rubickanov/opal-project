using System;

namespace Rubickanov.Opal.Domain
{
    [Serializable]
    public class CardSnapshot
    {
        public int Id;
        public int Value;
        public CardState State;

        public CardSnapshot(int id, int value, CardState state)
        {
            Id = id;
            Value = value;
            State = state;
        }
    }
}