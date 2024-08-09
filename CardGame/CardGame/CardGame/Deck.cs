namespace CardGame
{
    public class Deck
    {
        private List<Card> _cards;
        public List<Card> Cards => new List<Card>(_cards);

        public int CardsRemaining => _cards.Count;

        public Deck()
        {
            _cards = new List<Card>();
            var suits = new[] { "Hearts", "Diamonds", "Clubs", "Spades" };
            var values = new[] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

            foreach (var suit in suits)
            {
                foreach (var value in values)
                {
                    _cards.Add(new Card(value, suit));
                }
            }
        }

        public void Shuffle()
        {
            Random rand = new Random();
            _cards = _cards.OrderBy(c => rand.Next()).ToList();
        }

        public List<Card> Deal(int numberOfCards)
        {
            if (numberOfCards > _cards.Count)
            {
                return null;
            }

            var dealtCards = _cards.Take(numberOfCards).ToList();
            _cards.RemoveRange(0, numberOfCards);

            return dealtCards;
        }
    }
}