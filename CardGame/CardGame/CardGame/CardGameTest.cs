using CardGameApp;

namespace CardGameTest
{
    public class CardTest
    {
        [Fact]
        public void Card_Should_Return_Correct_Value_And_Suit()
        {
            // Arrange
            var card = new Card(Suit.Hearts, Rank.Ace);

            // Assert
            Assert.Equal(Suit.Hearts, card.Suit);
            Assert.Equal(Rank.Ace, card.Rank);
        }
    }

    public class DeckTests
    {
        [Fact]
        public void Deck_Should_Contain52_Cards_After_Initialization()
        {
            // Arrange & Act
            var deck = new Deck();

            // Assert
            Assert.Equal(52, deck.CardsRemaining);
        }

        [Fact]
        public void Deck_Shuffle_Should_Random_Card_Order()
        {
            // Arrange
            var deck = new Deck();
            var originalOrder = new List<Card>(deck.Cards);

            // Act
            deck.Shuffle();
            var shuffledOrder = deck.Cards;

            // Assert
            Assert.NotEqual(originalOrder, shuffledOrder);
        }

        [Fact]
        public void Deck_Deal_Should_Reduce_Cards_Remaining()
        {
            // Arrange
            var deck = new Deck();
            int initialCount = deck.CardsRemaining;

            // Act
            var dealtCards = deck.Deal(5);
            int finalCount = deck.CardsRemaining;

            // Assert
            Assert.Equal(5, dealtCards.Count);
            Assert.Equal(initialCount - 5, finalCount);
        }

        [Fact]
        public void Deck_Deal_Should_Return_Null_If_Not_Enough_Cards()
        {
            // Arrange
            var deck = new Deck();

            // Act
            var dealtCards = deck.Deal(53);

            // Assert
            Assert.Null(dealtCards);
        }
    }

}