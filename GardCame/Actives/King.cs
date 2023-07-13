using GardCame.Environment;
using GardCame.Interfaces;
using GardCame.Models;

namespace GardCame.Actives
{
    public class King : IActive
    {
        public void Active(Card card)
        {
            Board.Ammount = 99;
        }
    }
}
