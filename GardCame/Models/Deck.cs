using GardCame.Utils;

namespace GardCame.Models
{
    public class Deck
    {
        protected Stack<Card> deck { get; set; } = new Stack<Card>();

        public virtual Stack<Card> GetCards()
        {
            return deck;
        }

        public virtual Stack<Card> AddCard(Card card)
        {
            deck.Push(card);

            return deck;
        }

        public Card RemoveCard()
        {
            if(deck.Count() > 0)
                return deck.Pop();
            return null;
        }

        public Stack<Card> Shuffle()
        {
            deck.Shuffle();
            return deck;
        }
    }
}
