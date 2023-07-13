using GardCame.Environment;
using GardCame.Interfaces;
using GardCame.Models;
using System.Diagnostics;

namespace GardCame.Actives
{
    public class As : IActive
    {
        public void Active(Card card)
        {
            if(Board.Ammount - 1 >= 0)
                Board.Ammount -= 1;
        }
    }
}
