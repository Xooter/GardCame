using GardCame.Environment;
using GardCame.Interfaces;
using GardCame.Models;

namespace GardCame.Actives
{
    public class Add : IActive
    {
        public void Active(Card card)
        {
            Board.Ammount += card.Value;
        }
    }
}
