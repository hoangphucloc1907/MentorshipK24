namespace CardGameApp
{
    public class Deck
    {
        private List<Card> _cards;
        public List<Card> Cards => new List<Card>(_cards);

        public int CardsRemaining => _cards.Count;

        public Deck()
        {
            _cards = new List<Card>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    _cards.Add(new Card(suit, rank));
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