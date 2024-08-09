namespace CardGame
{
    public class Card
    {
        public string Value { get; }
        public string Suit { get; }

        public Card(string value, string suit)
        {
            Value = value;
            Suit = suit;
        }
    }
}