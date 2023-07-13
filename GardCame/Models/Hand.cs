using GardCame.Environment;
using GardCame.Models;
using GardCame.Utils;

namespace GardCame.Models
{
    public class Hand
    {

        protected List<Card> deck { get; set; } = new List<Card>();

        public virtual List<Card> GetCards()
        {
            return deck;
        }

        public virtual List<Card> AddCard(Card card)
        {
            if (deck.Count() >= 5) return deck;
            deck.Add(card);

            return deck;
        }

        public List<Card> RemoveCard(Card card)
        {
            deck.Remove(card);
            return deck;
        }

        public List<Card> ReplaceBlankCard(Card card)
        {
            int index = deck.FindIndex(x => x is BlankCard);
            if (index != -1)
                deck[index] = card;
            return deck;
        }

        public bool HasBlankCard()
        {
            return deck.Any(x => x is BlankCard);
        }

        public List<Card> Shuffle()
        {
            deck.Shuffle();
            return deck;
        }

        public override string ToString()
        {
            throw new NotImplementedException();
            string cardList = "";
            foreach (Card card in deck)
            {
                //cardList += $"{card.Name}${card.Description}${card.MaxHealth}#";
            }
            return cardList;
        }
       
    }
}
