using GardCame.Environment;
using GardCame.Interfaces;
using GardCame.Models;
using System.Diagnostics;

namespace GardCame.Actives
{
    public class Minus : IActive
    {
        public void Active(Card card)
        {
            if (Board.Ammount - 10 >= 0)
                Board.Ammount -= 10;
            else
                Board.Ammount = 0;   
        }
    }
}
